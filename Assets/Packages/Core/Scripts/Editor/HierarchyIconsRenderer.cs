using System.Reflection;
using UnityEditor;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using VarelaAloisio.Core.Extensions;
using VarelaAloisio.Core.Runtime;
using Debug = UnityEngine.Debug;

public static class HierarchyIconsRenderer
{
    private static readonly Texture2D Icon = AssetDatabase
        .LoadAssetAtPath<Texture2D>("Assets/Packages/Core/Scripts/References/Editor/service-bell_black.png");

    [InitializeOnLoadMethod]
    private static void HandleInitialization()
        => EditorApplication.hierarchyWindowItemOnGUI += RenderIcons;

    private static void RenderIcons(int instanceID, Rect selectionRect)
    {
        if (instanceID < 0)
            return;
        Object entity = EditorUtility.EntityIdToObject(instanceID);
        if (entity is not GameObject gameObject)
            return;

        if (gameObject.TryGetComponent(out MacacoBehaviour macacoBehaviour))
            DrawIconOnGameObject(selectionRect, macacoBehaviour);
        else if(gameObject.TryGetComponent(out RegisterComponentAsService registerBehaviour))
            DrawIconOnGameObject(selectionRect, registerBehaviour);
    }

    private static void DrawIconOnGameObject(Rect selectionRect, object target)
    {
        var serviceAttribute = target.GetType().GetCustomAttribute<ServiceAttribute>();
        if (serviceAttribute is null)
            return;
        float size = selectionRect.height;
        var iconRect = new Rect(selectionRect.width, selectionRect.y, size, size);
        GUI.Label(iconRect, new GUIContent(Icon));
    }
}
