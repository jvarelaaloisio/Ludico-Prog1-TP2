using TMPro;
using UnityEngine;
using VarelaAloisio.Core;
using VarelaAloisio.Core.Attributes;
using VarelaAloisio.Core.Extensions;

namespace Views
{
    public class HealthLabel : MacacoBehaviour
    {
        [AutoMap(How.GetComponent, When.Reset | When.Awake, OnError.Ignore)]
        [SerializeField] private TMP_Text label;

        public void HandleOnDamage(int before, int after)
            => label?.SetText(after.ToString().Colored(after > 0 ? Color.green : Color.red));
    }
}
