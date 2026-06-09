using Core.UI;
using UnityEngine;
using UnityEngine.Events;
using VarelaAloisio.Core;

namespace UI
{
    public class Menu : MacacoBehaviour, IMenu
    {
        [field: SerializeField] public Transform ButtonsParent { get; set; }
        [SerializeField] private MenuConfiguration configuration;
        [SerializeField] private UnityEvent onOpen;
        [SerializeField] private UnityEvent onClose;

        public IMenuConfiguration Configuration => configuration;

        public void Open()
        {
            ButtonsParent.gameObject.SetActive(true);
            onOpen.Invoke();
        }

        public void Close()
        {
            ButtonsParent.gameObject.SetActive(false);
            onClose.Invoke();
        }

        protected override void Reset()
        {
            base.Reset();
            ButtonsParent = transform;
        }
    }
}