using UnityEngine;
using System.Collections;

public class CameraSway : MonoBehaviour
{
	void Update()
		{
		if (Input.GetMouseButton(2) && !(Input.GetMouseButton(0) || Input.GetMouseButton(1)))
			{
			transform.RotateAround(transform.position, transform.right, Input.GetAxis("Mouse Y") * -1);
			Vector3 tempEuler = transform.localEulerAngles;
			if (70 < tempEuler.x && tempEuler.x < 290)
				{
				if (tempEuler.x > 180)
					tempEuler.x = 290;
				else
					tempEuler.x = 70;
				}
			transform.localEulerAngles = tempEuler;
			}
		}
}
