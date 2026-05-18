using System;
using Core.UI;
using UnityEngine;
using VarelaAloisio.Core;

namespace UI
{
    public class Menu : MacacoBehaviour, IMenu
    {
        [field: SerializeField] public Transform ButtonsParent { get; set; }
        [SerializeField] private MenuConfiguration configuration;

        public IMenuConfiguration Configuration => configuration;

        protected override void Reset()
        {
            base.Reset();
            ButtonsParent = transform;
        }
    }
}