using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [AddComponentMenu("Layout/Canvas Scaler (auto match)")]
    public class AutoMatchCanvasScaler : CanvasScaler
    {
        [SerializeField] private bool enableDebug;

        protected override void OnEnable()
        {
            base.OnEnable();
            CalculateMatch();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            CalculateMatch();
        }

        private void CalculateMatch()
        {
            float aspectRatio = (float)Screen.width / Screen.height;
            bool isHorizontal = aspectRatio > 1;
            m_MatchWidthOrHeight = isHorizontal ? 1 : 0;
            if (enableDebug)
                Debug.Log($"Aspect Ratio: {aspectRatio}, Match: {m_MatchWidthOrHeight}");
        }
    }
}