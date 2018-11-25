using UnityEngine;
using System.Collections;

namespace OdWyer.Graphics
{
	public class CircleRenderer : MonoBehaviour
	{
		private const int MIN_EDGES = 20;
		private const float MAX_EDGE_LENGTH = 100;

		private const float MIN_CIRC = MIN_EDGES * MAX_EDGE_LENGTH;

		private Color _lineColor;
		private Material _lineMaterial;
		private float _lineWidth;

		private LineRenderer _edgeRenderer;

		private float _radius = MIN_CIRC;
		public float radius
		{
			get { return _radius; }
			set
			{
				_radius = value;

				if(_edgeRenderer)
					BuildRenderer();
			}
		}


		internal float anglePerEdge
		{
			get
			{
				if(!_edgeRenderer)
					return 0;

				return (2 * Mathf.PI) / (_edgeRenderer.positionCount - 1);
			}
		}

		void Awake()
		{
			_lineColor = new Color(1f, 1f, 1f);
			_lineMaterial = new Material(Shader.Find("Unlit/Color"));
			_lineWidth = 0.2f;
		}

	    public void Initialise()
	    {
	        _edgeRenderer = gameObject.AddComponent<LineRenderer>();
	        _edgeRenderer.useWorldSpace = false;

			_edgeRenderer.material = _lineMaterial;
			_edgeRenderer.material.color = _lineColor;

			_edgeRenderer.startWidth = _lineWidth;
			_edgeRenderer.endWidth = _lineWidth;

			BuildRenderer();
	    }

		internal void BuildRenderer()
		{
			if(!_edgeRenderer)
				Initialise();

			float circumfrence = Mathf.PI * radius * radius;
			_edgeRenderer.positionCount = ((circumfrence < MIN_CIRC) ? MIN_EDGES : Mathf.CeilToInt(circumfrence / MAX_EDGE_LENGTH)) + 1;

			for (int i = 0; i < _edgeRenderer.positionCount; i++)
			{
				_edgeRenderer.SetPosition(i, Position(i));
			}
		}

		internal Vector3 Position(int vertex)
		{
			if(!_edgeRenderer)
				return Vector3.zero;

			if(	vertex == 0
			||	vertex == _edgeRenderer.positionCount - 1
			)
				return new Vector3(radius, 0, 0);

			return new Vector3(radius * Mathf.Cos(anglePerEdge * vertex), 0, radius * Mathf.Sin(anglePerEdge * vertex));
		}
	}
}