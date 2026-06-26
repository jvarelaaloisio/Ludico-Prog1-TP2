using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;

namespace Views
{
    public class DialogueView : MacacoBehaviour, ITimeControl
    {
        [AutoMap(How.GetComponent, When.Reset, OnError.Ignore)]
        [SerializeField] private TMP_Text dialogue;
        [SerializeField] private float duration;
        /// <inheritdoc />
        public void SetTime(double time)
        {
            float lerp = (float)time / duration;
            int maxCharacters = Mathf.CeilToInt(dialogue.text.Length * lerp);
            dialogue.maxVisibleCharacters = maxCharacters;
        }

        /// <inheritdoc />
        public void OnControlTimeStart()
            => dialogue.gameObject.SetActive(true);

        /// <inheritdoc />
        public void OnControlTimeStop()
            => dialogue.gameObject.SetActive(false);
    }
}