using UnityEngine;
using System.Collections;

public class CameraSway : MonoBehaviour
{
	//	Seperate script designed to allow influence on the camera ONLY in up/down
	//	NOTE: This means that it is pivoting AROUND the x / right-left axis
	void Update()
	{
		//	If the middle mouse button is down and we are not performing a GUI action
		if(Input.GetMouseButton(2) && !PCSelector.lockOut)
		{
			//	Rotate around right
			transform.RotateAround(transform.position, transform.right, Input.GetAxis("Mouse Y") * -1);
			
			//	Limit x to 0 - 70 Deg
			Vector3 tempEuler = transform.localEulerAngles;

			//	First process if over 180deg to determine we arent going 'up'
			if(tempEuler.x > 180)
			{
				tempEuler.x = 0;
			}
			//	Else limit to the down of 70 deg
			else if(tempEuler.x > 70)
			{
				tempEuler.x = 70;
			}

			//	Apply
			transform.localEulerAngles = tempEuler;
		}
	}
}
