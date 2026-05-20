using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VarelaAloisio.Core;

namespace UI
{
    public class LoadingBar : MonoBehaviour, IProgress<float>, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image fill;
        [SerializeField] private Gradient colors;

        public void Report(float value)
        {
            fill.fillAmount = value;
            fill.color = colors.Evaluate(value);
        }

        public Task Show()
        {
            canvas.enabled = true;
            return Task.CompletedTask;
        }

        public Task Hide()
        {
            canvas.enabled = false;
            return Task.CompletedTask;
        }
    }
}
