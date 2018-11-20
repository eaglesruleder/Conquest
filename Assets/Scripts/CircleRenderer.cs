using UnityEngine;
using System.Collections;

public class CircleRenderer : MonoBehaviour {

    public float radius;
    int minEdges = 20;
    float maxEdgeLen = 100;
    LineRenderer edgeRenderer;

    public void Initialise()
    {
        edgeRenderer = gameObject.AddComponent<LineRenderer>();
        edgeRenderer.useWorldSpace = false;
        edgeRenderer.material = new Material(Shader.Find("Unlit/Texture"));
        edgeRenderer.material.color = new Color(1f, 0.92f, 0.016f, 0.5f);
        edgeRenderer.SetWidth(0.2f, 0.2f);
    }

    public void SetRadius(float Radius)
    {
        radius = Radius;

        float cir = Mathf.PI * Radius * Radius;

        bool pass = cir < maxEdgeLen * minEdges;
        int edges = (pass) ? minEdges : (int) Mathf.Ceil(cir / maxEdgeLen);
        edgeRenderer.SetVertexCount(edges + 1);

        float angle = 2 * Mathf.PI / edges;
        for (int i = 0; i < edges; i++)
        {
            edgeRenderer.SetPosition(i, new Vector3(radius * Mathf.Cos(angle * i), 0, radius * Mathf.Sin(angle * i)));
        }
        edgeRenderer.SetPosition(edges, new Vector3(radius, 0, 0));
    }


}
