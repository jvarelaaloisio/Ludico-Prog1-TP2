using UnityEngine;

namespace VarelaAloisio.Core.Debugging
{
    public interface IGizmoDrawer
    {
        void DrawLine(string tag, Vector3 start, Vector3 end);
        void DrawLine(string tag, Vector3 start, Vector3 end, Color color);
        void DrawLine(string tag, Vector3 start, Vector3 end, Color color, float duration);
        void DrawRay(string tag, Vector3 start, Vector3 dir);
        void DrawRay(string tag, Vector3 start, Vector3 dir, Color color);
        void DrawRay(string tag, Vector3 start, Vector3 dir, Color color, float duration);
    }
}