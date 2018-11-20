using UnityEngine;
using System.Collections;

public class MeshHandler : MonoBehaviour {

	public Mesh MeshLow;
    public Mesh MeshHigh;

    bool renderHigh = false;

    MeshFilter output;

    void OnEnable()
    {
        if (!(MeshLow || MeshHigh))
        {
            throw new UnityEngine.UnityException("MeshHandler enabled without meshes");
        }

        if(!output)
        {
            output = gameObject.GetComponent<MeshFilter>();
            if(!output)
            {
                throw new UnityEngine.UnityException("MeshHandler enabled without MeshFilter");
            }
        }

        output.mesh = (MeshLow) ? MeshLow : MeshHigh;
    }

    public void SwitchRender(int Quality)
    {
        if(!output || !(MeshLow || MeshHigh))
        {
            throw new UnityEngine.UnityException("MeshHandler switched without requirements");
        }

        if (Quality > 0)
        {
            renderHigh = true;
        }
        else if (Quality < 0)
        {
            renderHigh = false;
        }
        else
        {
            renderHigh = !renderHigh;
        }

        if (!renderHigh && MeshLow)
        {
            output.mesh = MeshLow;
        }
        else if (renderHigh && MeshHigh)
        {
            output.mesh = MeshHigh;
        }
        else if (MeshLow || MeshHigh)
        {
           output.mesh = (MeshLow) ? MeshLow : MeshHigh;
        }
        else
        {
            throw new UnityEngine.UnityException("MeshHandler switched without Meshes");
        }
    }

	void OnBecameInvisible()
	{
		if (transform.parent.gameObject.GetComponent<PlayerControlled> ())
		{
			transform.parent.gameObject.GetComponent<PlayerControlled> ().OnBecameInvisible();
		}
	}

	void OnBecameVisible()
	{
		if (transform.parent.gameObject.GetComponent<PlayerControlled> ())
		{
			transform.parent.gameObject.GetComponent<PlayerControlled> ().OnBecameVisible();
		}
	}
}
