using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VarelaAloisio.Core.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using VarelaAloisio.Core.Runtime;
using VarelaAloisio.Core.Utils;
using ThreadPriority = UnityEngine.ThreadPriority;

namespace VarelaAloisio.Scenes
{
    [Service(typeof(ILevelService))]
    public class LevelService : MacacoBehaviour, ILevelService
    {
    #region Log Strings

        private static readonly string Activating = "Activating ".Colored(C.Yellow);
        private static readonly string Activated = "Activated ".Colored(C.Green);
        private static readonly string Loading = "Loading ".Colored(C.Yellow);
        private static readonly string Loaded = "Loaded ".Colored(C.Green);
        private static readonly string Unloading = "Unloading ".Colored(C.Red);
        private static readonly string Unloaded = "Unloaded ".Colored(C.Green);
        private static readonly string Waiting = "Waiting ".Colored(C.Yellow);

    #endregion
        private class LoadingState
        {
            public readonly List<SceneAsyncOperation> LoadingOperations = new();
            public readonly List<SceneAsyncOperation> UnloadingOperations = new ();

            /// <summary/>Unloads all scenes that were in the middle of loading when this coroutine started
            public IEnumerator Reset(Coroutine before, Action onFinish = null, UnityEngine.Object owner = null)
            {
                if (before != null)
                    yield return before;
                Debug.Log("Resetting Loading state. ".Colored(Color.yellow), owner);
                foreach (SceneAsyncOperation sceneOperation in LoadingOperations)
                {
                    if (HasUnloadOperation(sceneOperation))
                    {
                        yield return WaitForUnloadToFinish(sceneOperation);
                        continue;
                    }
                    
                    if (sceneOperation.AsyncOperation != null)
                        yield return WaitForLoadToFinish(sceneOperation);

                    Debug.Log(Unloading + sceneOperation.Scene.name, owner);
                    var unloadOperation = SceneManager.UnloadSceneAsync(sceneOperation.Scene);
                    if (unloadOperation != null)
                        yield return new WaitUntil(() => ProgressIsOver90Percent(unloadOperation));
                    else
                        Debug.Log($"Unload operation for {sceneOperation.Scene.name} failed.", owner);
                    Debug.Log(Unloaded + sceneOperation.Scene.name, owner);
                }

                Debug.Log("Reset successful. ".Colored(Color.green) + "Clearing loading state", owner);
                Clear();
                onFinish?.Invoke();

                yield break;

                bool HasUnloadOperation(SceneAsyncOperation sceneOperation)
                    => UnloadingOperations
                                    .Exists(op => op.Scene == sceneOperation.Scene);

                IEnumerator WaitForUnloadToFinish(SceneAsyncOperation sceneOperation)
                {
                    var unloadOperation = UnloadingOperations.Find(op => op.Scene == sceneOperation.Scene);
                    Debug.Log(Waiting + $"for ({unloadOperation.Scene.name}) to finish unloading", owner);
                    yield return unloadOperation.AsyncOperation;
                }

                IEnumerator WaitForLoadToFinish(SceneAsyncOperation sceneOperation)
                {
                    sceneOperation.AsyncOperation.allowSceneActivation = true;
                    Debug.Log(Waiting + $"for ({sceneOperation.Scene.name}) to finish loading"
                             + $"\nCurrent progress: {sceneOperation.AsyncOperation.progress}", owner);
                    yield return new WaitUntil(()=> ProgressIsOver90Percent(sceneOperation.AsyncOperation));
                }
            }
            public void Clear()
            {
                LoadingOperations.Clear();
                UnloadingOperations.Clear();
            }
        }

        //TODO: Refactor to not use UI directly
        [SerializeField] private Ref<IView> loadingScreen;
        [SerializeField] private Ref<IProgress<float>> progressModelView;
        [Header("Delays")]
        [SerializeField] private float delayBeforeHidingLoadScreen = .5f;
        [SerializeField] private float delayBetweenBatches = 1f;
        [SerializeField] private float delayBeforeLoadingNextScene = 3f;
        [SerializeField] private float delayBeforeActivatingScene = .5f;

        private ThreadPriority _defaultBackgroundLoadingPriority;
        private float _totalSceneProgress;
        private ILevel _currentLevel;
        private Coroutine _currentLoadCoroutine;
        private Coroutine _resetLoadingStateCoroutine;
        private bool _isLoading = false;
        private CancellationTokenSource _loadTokenSource = new();
        private readonly LoadingState _loadingState = new();
        private readonly List<string> _currentLoadedScenePaths = new(20);

        protected override void Awake()
        {
            base.Awake();
            _defaultBackgroundLoadingPriority = Application.backgroundLoadingPriority;
        }

        /// <summary /> Unloads the previous level and loads the given one.
        /// <param name="newLevel">The level to change to</param>
        public void LoadLevel(ILevel newLevel)
        {
            Log(Loading + $"Level [{newLevel.name}]");
            TokenUtils.Recreate(ref _loadTokenSource);

            if (!_isLoading)
            {
                StartCoroutine(DoLoadLevel(_currentLevel, newLevel));
            }
            _currentLevel = newLevel;
        }

        private IEnumerator DoLoadLevel(ILevel oldLevel, ILevel newLevel)
        {
            _isLoading = true;
            loadingScreen.Value?.Show();
            yield return _loadingState.Reset(_currentLoadCoroutine, owner:this);
            yield return DoChangeLevel(oldLevel, newLevel, _loadTokenSource.Token);
            _isLoading = false;
        }

        //TODO: Refactor into an async task and change the delay before hiding loading screen into a list of setup tasks to await
        private IEnumerator DoChangeLevel(ILevel oldLevel,
                                          ILevel newLevel,
                                          CancellationToken token = default)
        {
            var scenesLoadedCount = 0;
            var scenesToLoadCount = newLevel.ImmediateBatch.Length
                                       + (oldLevel?.TotalSceneCount ?? 0);
            
            if (oldLevel is not null)
                yield return UnloadOldLevel();

            var loadReport = Loaded + $"level {newLevel.name}. Report:";
            Application.backgroundLoadingPriority = ThreadPriority.High;
            yield return LoadImmediate();

            Log(loadReport);

            Application.backgroundLoadingPriority = ThreadPriority.Low;
            //THOUGHT: I think I added this delay because the player started in the air and Cinemachine needed to place itself.
            //We would probably benefit from adding a "Setup" system for the level
            yield return new WaitForSeconds(delayBeforeHidingLoadScreen);
            SetupActiveScene(newLevel);

            loadingScreen.Value?.Hide();

            yield return LoadBatches();

            Application.backgroundLoadingPriority = _defaultBackgroundLoadingPriority;
            
            yield break;

            IEnumerator UnloadOldLevel()
            {
                _currentLoadedScenePaths.Clear();
                var unloadReport = new StringBuilder(Unloaded + $"level {oldLevel.name}. Report:");

                foreach (var unloadOperation in oldLevel.Unload())
                {
                    double duration = Time.realtimeSinceStartupAsDouble;
                    
                    _loadingState.UnloadingOperations.Add(unloadOperation);
                    if (token.IsCancellationRequested)
                    {
                        Log($"Unload cancelled".Colored(C.Red));
                        yield break;
                    }
                    yield return UpdateLevelLoadProgress(unloadOperation, scenesLoadedCount, scenesToLoadCount);
                    _loadingState.UnloadingOperations.Remove(unloadOperation);
                    
                    duration = Time.realtimeSinceStartupAsDouble - duration;
                    unloadReport.AppendLine($"{unloadOperation.Path} ({duration * 1000:F} ms)");
                    scenesLoadedCount++;
                }
                Log(unloadReport.ToString());
            }

            IEnumerator LoadImmediate()
            {
                foreach (var loadOperation in newLevel.LoadImmediate())
                {
                    _currentLoadedScenePaths.Add(loadOperation.Path);
                    double duration = Time.realtimeSinceStartupAsDouble;
                
                    _loadingState.LoadingOperations.Add(loadOperation);
                    if (token.IsCancellationRequested)
                    {
                        Log($"Immediate Load cancelled".Colored(C.Red));
                        yield break;
                    }
                    yield return UpdateLevelLoadProgress(loadOperation, scenesLoadedCount, scenesToLoadCount);
                    _loadingState.LoadingOperations.Remove(loadOperation);
                
                    duration = Time.realtimeSinceStartupAsDouble - duration;
                    loadReport += $"\n{loadOperation.Path} ({duration * 1000:F} ms)";
                    scenesLoadedCount++;
                }
            }

            IEnumerator LoadBatches()
            {
                foreach (var levelBatch in newLevel.LoadDeferredBatches)
                {
                    foreach (var loadOperation in levelBatch.Load())
                    {
                        var asyncOperation = loadOperation.AsyncOperation;
                        asyncOperation.allowSceneActivation = false;
                        //TODO: Try to make this whole block more readable. Maybe add a report, like the ones above?
                        string sceneName = loadOperation.Scene.name;
                        Log(Loading + sceneName);

                        double loadStartTime = Time.realtimeSinceStartupAsDouble;
                    
                        _loadingState.LoadingOperations.Add(loadOperation);
                        if (token.IsCancellationRequested)
                        {
                            Log($"Batch Load cancelled".Colored(C.Red));
                            yield break;
                        }
                        yield return new WaitUntil(() => ProgressIsOver90Percent(asyncOperation));
                        _loadingState.LoadingOperations.Remove(loadOperation);

                        //TODO: Use Stopwatch in the same way as in the InterfaceRef inspector.
                        int loadMilliseconds = (int)(Time.realtimeSinceStartupAsDouble - loadStartTime) * 1000;
                        logger.Log(logTag,
                                   Loaded + $"{sceneName} in {loadMilliseconds.Colored(C.Red)}ms"
                                          + $"\nWaiting {delayBeforeActivatingScene} seconds before activation.".Colored(C.Black),
                                   this);

                        if (token.IsCancellationRequested)
                        {
                            Log($"Batch Load cancelled".Colored(C.Red));
                            yield break;
                        }

                        //TODO: Handle this with ThreadPriority instead of arbitrary delays
                        //THOUGHT: Is this really necessary?
                        yield return new WaitForSeconds(delayBeforeActivatingScene);

                        double activationStartTime = Time.realtimeSinceStartupAsDouble;
                        Log(Activating + sceneName);
                        asyncOperation.allowSceneActivation = true;

                        int activationMilliseconds = (int)(Time.realtimeSinceStartupAsDouble - activationStartTime) * 1000;
                        Log(Activated + $"{sceneName} in {activationMilliseconds.Colored(C.Red)}ms"
                                      + $"\nWaiting {delayBetweenBatches * 1000}ms.".Colored(C.Black));
                        
                        //THOUGHT: Is this really necessary? Maybe just yield return null?
                        yield return new WaitForSeconds(delayBeforeLoadingNextScene);
                    }

                    //THOUGHT: Again, is this really necessary? I think we should test changing all of these random delays to wait just a frame.
                    //We should also research how to slow down loading so it doesn't freeze the game at all.
                    yield return new WaitForSeconds(delayBetweenBatches);
                }
            }
        }

        private static bool ProgressIsOver90Percent(AsyncOperation asyncOperation)
            => asyncOperation.progress >= 0.89f;

        private void SetupActiveScene(ILevel level)
        {
            int activeSceneBuildIndex = level.ActiveScene.BuildIndex;
            if (activeSceneBuildIndex == -1)
            {
                Log($"Active scene in level ({level.ActiveScene}[{activeSceneBuildIndex}]) is not found in build settings!");
                return;
            }

            var activeScene = SceneManager.GetSceneByBuildIndex(activeSceneBuildIndex);
            if (!activeScene.IsValid())
            {
                Log($"Active Scene ({activeScene.name}) is not valid!");
                return;
            }

            SceneManager.SetActiveScene(activeScene);
            Log($"Setting {activeScene.name} as active scene.");
        }

        private IEnumerator UpdateLevelLoadProgress(SceneAsyncOperation loadOperation, int scenesAlreadyLoadedQty,
                                                    int totalScenesToLoadQty)
        {
            while (!loadOperation.AsyncOperation.isDone)
            {
                float loadOperationProgress = loadOperation.AsyncOperation.progress + scenesAlreadyLoadedQty;
                UpdateLoadingBarProgress(loadOperationProgress, totalScenesToLoadQty);
                yield return null;
            }
        }

        private void UpdateLoadingBarProgress(float current, float total)
            => progressModelView.Value?.Report(current / total);
    }
}