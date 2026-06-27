using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    public class ImageWithRectUV : Image
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);

            Rect r = rectTransform.rect;

            UIVertex v = new UIVertex();

            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref v, i);

                float u = Mathf.InverseLerp(r.xMin, r.xMax, v.position.x);
                float y = Mathf.InverseLerp(r.yMin, r.yMax, v.position.y);

                v.uv1 = new Vector2(u, y);

                vh.SetUIVertex(v, i);
            }
        }
    }
}