using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFramePro.UI
{
    [AddComponentMenu("UI/Effects/Gradient")]
    public class GradientPro : BaseMeshEffect
    {
        public Color32 topColor = Color.white;
        public Color32 bottomColor = Color.black;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!this.IsActive())
                return;

            List<UIVertex> vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            ModifyVertices(vertexList);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }

        public void ModifyVertices(List<UIVertex> vertexList)
        {
            if (!IsActive())
            {
                return;
            }

            int count = vertexList.Count;
            if (count > 0)
            {
                float bottomY = vertexList[0].position.y;
                float topY = vertexList[0].position.y;

                for (int i = 1; i < count; i++)
                {
                    float y = vertexList[i].position.y;
                    if (y > topY)
                    {
                        topY = y;
                    }
                    else if (y < bottomY)
                    {
                        bottomY = y;
                    }
                }

                float uiElementHeight = topY - bottomY;

                for (int i = 0; i < count; i++)
                {
                    UIVertex uiVertex = vertexList[i];
                    uiVertex.color = Color32.Lerp(bottomColor, topColor, (uiVertex.position.y - bottomY) / uiElementHeight);
                    vertexList[i] = uiVertex;
                }
            }
        }

        public override void ModifyMesh(Mesh mesh)
        {
            if (!IsActive())
            {
                return;
            }

            Vector3[] vertexList = mesh.vertices;
            int count = mesh.vertexCount;
            if (count > 0)
            {
                float bottomY = vertexList[0].y;
                float topY = vertexList[0].y;

                for (int i = 1; i < count; i++)
                {
                    float y = vertexList[i].y;
                    if (y > topY)
                    {
                        topY = y;
                    }
                    else if (y < bottomY)
                    {
                        bottomY = y;
                    }
                }
                List<Color32> colors = new List<Color32>();
                float uiElementHeight = topY - bottomY;
                for (int i = 0; i < count; i++)
                {
                    colors.Add(Color32.Lerp(bottomColor, topColor, (vertexList[i].y - bottomY) / uiElementHeight));
                }
                mesh.SetColors(colors);
            }
        }


        public void SetColor(Color32 top, Color32 bottom)
        {
            if (topColor.a != top.a || topColor.r != top.r || topColor.g != top.g || topColor.b == top.b)
                topColor = top;

            if (bottomColor.a != bottom.a || bottomColor.r != bottom.r || bottomColor.g != bottom.g || bottomColor.b == bottom.b)
                bottomColor = bottom;
        }


        public void SetColor(Color32 newColor,bool isTop)
        {
            if (isTop)
                SetColor(newColor, bottomColor);
            else
                SetColor(topColor, newColor);

        }
    }
}
