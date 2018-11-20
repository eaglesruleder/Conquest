using UnityEngine;
using System.Collections;

[System.Serializable]
public class Serializable_LineRenderer
{
	public float width;
	public float[] color;
	public Color toColor
	{get
		{
			if (color.Length == 3)
			{
				return new Color(color[0], color[1], color[2]);
			}
			else if (color.Length == 4)
			{
				return new Color(color[0], color[1], color[2], color[3]);
			}
			else
			{
				throw new UnityEngine.UnityException("hue length of " + color.Length + " declared but not usable");
			}
		}
	}

	public Serializable_LineRenderer()
	{
		width = 0f;
		color = new float[]{0, 0, 0, 0};
	}
	
	public Serializable_LineRenderer(float width, float[] color)
	{
		this.width = width;
		this.color = color;
	}
	
	//	Add a capsult collider to an existing object
	public LineRenderer AddComponent(GameObject source)
	{
		//	Radius of 0 is a null collider
		if(width > 0)
		{
			LineRenderer line = source.AddComponent<LineRenderer>();

			line.material = new Material(Shader.Find("Unlit/Color"));
			line.material.color = toColor;

			return line;
		}
		return null;
	}
}