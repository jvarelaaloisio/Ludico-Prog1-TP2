using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
using VarelaAloisio.Core;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace UI
{
    public class InputIconsHandler : MacacoBehaviour
    {
        [SerializeField] private GameObject pcVersion;
        [SerializeField] private GameObject playStationVersion;
        [SerializeField] private GameObject xboxVersion;

        public void SetVersion(InputDevice device)
        {
            switch (device.name)
            {
                case "Keyboard":
                case "Mouse":
                    pcVersion.SetActive(true);
                    playStationVersion.SetActive(false);
                    xboxVersion.SetActive(false);
                    break;
                case "DualSenseGamepadHID":
                    pcVersion.SetActive(false);
                    playStationVersion.SetActive(true);
                    xboxVersion.SetActive(false);
                    break;
                default:
                    pcVersion.SetActive(false);
                    playStationVersion.SetActive(false);
                    xboxVersion.SetActive(true);
                break;
            }

            // switch (device)
            // {
            //     case Keyboard:
            //     case Mouse:
            //         pcVersion.SetActive(true);
            //         playStationVersion.SetActive(false);
            //         xboxVersion.SetActive(false);
            //         break;
            //     case DualShockGamepad:
            //         pcVersion.SetActive(false);
            //         playStationVersion.SetActive(true);
            //         xboxVersion.SetActive(false);
            //         break;
            //     case XInputController:
            //         pcVersion.SetActive(false);
            //         playStationVersion.SetActive(false);
            //         xboxVersion.SetActive(true);
            //         break;
            // }
        }
    }
}
