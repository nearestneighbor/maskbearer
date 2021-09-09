using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeshGradient : BaseMeshEffect
{
    public Color m_topLeftColor = Color.white;
	public Color m_topRightColor = Color.white;
	public Color m_bottomRightColor = Color.white;
	public Color m_bottomLeftColor = Color.white;

    public override void ModifyMesh(VertexHelper vh)
    {
		if (enabled)
		{
			var rect = graphic.rectTransform.rect;
			var vertex = default(UIVertex);
            var vertexPos = new [] { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };

			for (int i = 0; i < vh.currentVertCount; i++)
            {
				vh.PopulateUIVertex (ref vertex, i);
				Vector2 normalizedPosition = vertexPos[i % 4];
				vertex.color *= Bilerp(m_bottomLeftColor, m_bottomRightColor, m_topLeftColor, m_topRightColor, normalizedPosition);
				vh.SetUIVertex (vertex, i);
			}
		}
    }

    

	public static Color Bilerp(Color a1, Color a2, Color b1, Color b2, Vector2 t)
	{
		Color a = Color.LerpUnclamped(a1, a2, t.x);
		Color b = Color.LerpUnclamped(b1, b2, t.x);
		return Color.LerpUnclamped(a, b, t.y);
	}
}
