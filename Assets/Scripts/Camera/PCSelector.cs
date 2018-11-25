using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class PCSelector : MonoBehaviour{

	static PCSelector thisPlayer;

	Dictionary<string, List<PlayerControlled>> onScreen = new Dictionary<string, List<PlayerControlled>> ();
	public static List<PlayerControlled> PlayerOnScreen
	{
		get
		{
			List<PlayerControlled> result;
			thisPlayer.onScreen.TryGetValue (PlayerManager.ThisPlayerID, out result);
			return result;
		}
	}

	public static PlayerControlled selectedObject;
	public static List<PlayerControlled> selectedObjects;

	public static bool lockOut {get{return thisPlayer.leftDragging || thisPlayer.rightDragging;}}
	
	Vector2 mouseStart = Vector2.zero;
	Vector2 mouseEnd = Vector2.zero;

	internal bool leftDragging = false;
	internal bool rightDragging = false;

    Vector3 vecRightStart = Vector3.zero;
    Vector3 vecRightEnd = Vector3.zero;


    LineRenderer drawToMove;
    CircleRenderer drawMoveRad;
    CircleRenderer drawToRad;
    CircleRenderer drawFromRad;

    void Awake()
	{
		if(thisPlayer == null)
		{
			DontDestroyOnLoad(this);
			thisPlayer = this;

			selectedObjects = new List<PlayerControlled>();
			onScreen.Add(PlayerManager.ThisPlayerID, new List<PlayerControlled>());
		}
		else
		{
			Destroy(this.gameObject);
		}

        drawToMove = gameObject.AddComponent<LineRenderer>();
        drawToMove.useWorldSpace = true;

        drawToMove.material = new Material(Shader.Find("Unlit/Color"));
        drawToMove.material.color = new Color(1f, 1f, 1f);

		drawToMove.startWidth = 0.2f;
		drawToMove.endWidth = 0.2f;

		drawToMove.positionCount = 3;


		//	Create temp object with CircleRenderer attached
        GameObject temp = new GameObject();
        temp.AddComponent<CircleRenderer>();

		//	Instantiate for each CircleRenderer required
        drawMoveRad = Instantiate(temp.GetComponent<CircleRenderer>()) as CircleRenderer;
        drawMoveRad.Initialise();
        drawToRad = Instantiate(temp.GetComponent<CircleRenderer>()) as CircleRenderer;
        drawToRad.Initialise();
        drawFromRad = Instantiate(temp.GetComponent<CircleRenderer>()) as CircleRenderer;
        drawFromRad.Initialise();

		//	Destroy origional temp
        DestroyImmediate(temp);

		//	Disable CircleRenderers
        ActivateMoveGUI(false);
	}

    void OnGUI()
    {
		Vector2 mouseCur = Input.mousePosition;
		
		//	If we are hovering over the GUI, and the 
		if (EventSystem.current.IsPointerOverGameObject())
		{
			if(!mouseEnd.Equals(Vector2.zero))
			{
				mouseCur = mouseEnd;
				mouseCur.x = Input.mousePosition.x;
			}
			else
			{
				return;
			}
		}
		
		//	ALL ABOUT THE LEFT
		//	If left is down, and we are not dragging right
		if(Input.GetMouseButton(0) && !rightDragging)
		{
			//	If this is a new event
			if(mouseStart.Equals(Vector2.zero))
			{
				//	Start as a new event
				mouseStart = mouseCur;
				leftDragging = true;
			}

			//	Else if we are not yet dragging but the distance is > 5
			else if(mouseEnd.Equals(Vector2.zero) && Vector2.Distance(mouseStart, mouseCur) > 5)
			{
				//	Start it as a dragging event
				mouseEnd = mouseCur;
			}

			//	On Drag
			else if(!mouseEnd.Equals(Vector2.zero))
			{
				mouseEnd = mouseCur;

				//	Create GUI Box
				Rect box = new Rect(
					 (mouseStart.x > mouseEnd.x) ? mouseEnd.x : mouseStart.x
					,Screen.height - ((mouseStart.y > mouseEnd.y) ? mouseStart.y : mouseEnd.y)
					,(mouseStart.x > mouseEnd.x) ? mouseStart.x - mouseEnd.x : mouseEnd.x - mouseStart.x
					,(mouseStart.y > mouseEnd.y) ? mouseStart.y - mouseEnd.y : mouseEnd.y - mouseStart.y);
				GUI.Box(box, "");
			}
		}

		//	Else if we are dragging left (without input)
		else if (leftDragging)
		{
			List<PlayerControlled> tempObjs = new List<PlayerControlled>();

			//	If a non-dragging event, process as a raycast
			if(mouseEnd.Equals(Vector2.zero))
			{
				//	Create ray in direction of mouse
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hitOut;

				//	If we hit something
				if (Physics.Raycast(ray, out hitOut))
				{
					//	And its a PlayerControlled
					if(hitOut.collider.gameObject.GetComponent<PlayerControlled>())
					{
						tempObjs.Add(hitOut.collider.gameObject.GetComponent<PlayerControlled>());
					}
				}
			}

			//	Otherwise this is a box-selection
			else
			{
				//	First do a check for the values
				Rect rect = new Rect(
					(mouseStart.x > mouseEnd.x) ? mouseEnd.x : mouseStart.x
					,(mouseStart.y > mouseEnd.y) ? mouseEnd.y : mouseStart.y
					,(mouseStart.x > mouseEnd.x) ? mouseStart.x - mouseEnd.x : mouseEnd.x - mouseStart.x
					,(mouseStart.y > mouseEnd.y) ? mouseStart.y - mouseEnd.y : mouseEnd.y - mouseStart.y
					);
				
				//	Get all objects inside the selection box
				foreach (PlayerControlled on in PlayerOnScreen)
				{
					CapsuleCollider onColl = on.GetComponent<CapsuleCollider>();
					Vector3 forward = on.transform.forward * onColl.height / 2;
					Vector3 right = on.transform.right * onColl.radius;

					//	Check if the object exists inside the selection box
					if (   rect.Contains(Camera.main.WorldToScreenPoint(on.transform.position))
					    || rect.Contains(Camera.main.WorldToScreenPoint(on.transform.position + forward))
					    || rect.Contains(Camera.main.WorldToScreenPoint(on.transform.position - forward))
					    || rect.Contains(Camera.main.WorldToScreenPoint(on.transform.position + right))
					    || rect.Contains(Camera.main.WorldToScreenPoint(on.transform.position - right))
					    )
					{
						tempObjs.Add(on);
					}
				}
			}
				
			//	If Ctrl then remove objects from list
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl))
			{
				RemovePlayerControlled(tempObjs);
			}
			
			//	Else If Shift then Add objects to list
			else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				AddPlayerControlled(tempObjs);
			}
			
			//	Otherwise consider selection as 'From Scratch'
			else
			{
				SetPlayerControlled(tempObjs);
			}

			//	Empty out variables
			leftDragging = false;
			mouseStart = Vector2.zero;
			mouseEnd = Vector2.zero;
		}

		//	ELSE ITS ALL ABOUT THE RIGHTCLICKS

		//	If right is down, and we are not dragging left
		else if (Input.GetMouseButton(1) && !leftDragging)
		{
			//	Process order-right click commands
			if(selectedObject)
			{
				//	If this is a new event
				if(mouseStart.Equals(Vector2.zero))
				{
					//	Start as a new event
					mouseStart = mouseCur;
					rightDragging = true;
				}

				//	Else if we are not yet dragging but the distance is > 5
				else if(mouseEnd.Equals(Vector2.zero) && Vector2.Distance(mouseStart, mouseCur) > 5)
				{
					mouseEnd = mouseCur;

					//	Create a ray aimed at mouse
					Ray ray = Camera.main.ScreenPointToRay(mouseStart);
					
					//	Create a plane at selectedObject height aimed up
					Plane collPlane = new Plane(Vector3.up, selectedObject.transform.position);
					
					//	Shoot the ray at the plane, to determine location
					float distance = 0f;
					if (collPlane.Raycast(ray, out distance))
					{
						vecRightStart = ray.GetPoint(distance);
					}
					
					//	Apply
					UpdateMoveGUI(vecRightStart, selectedObject);
					ActivateMoveGUI(true);
				}

				//	Else this is a drag event
				else if(!mouseEnd.Equals(Vector2.zero))
				{
					mouseEnd = mouseCur;

					//	Create a normal from the camera, at the startPosition, thats perfectly vertical
					Vector3 cameraPos = Camera.main.transform.position;
					Vector3 planeNormal = new Vector3(cameraPos.x - vecRightStart.x, 0f, cameraPos.z - vecRightStart.z).normalized;

					//	Create a plane thats aimed at the camera to point at
					Plane collHeightPlane = new Plane(planeNormal, vecRightStart);

					//	Create a ray, at start-width, and current-height
					Ray rayHeight = Camera.main.ScreenPointToRay(new Vector2 (mouseStart.x, mouseEnd.y));

					//	Shoot the ray at the plane, to determine location
					float distance = 0f;
					if (collHeightPlane.Raycast(rayHeight, out distance))
					{
						Vector3 testPos = rayHeight.GetPoint(distance);
						//	If the position is between -20 and 20 apply it
						if(20 > testPos.y && testPos.y > -20)
						{
							vecRightEnd = testPos;
						}
					}

					//	Apply
					UpdateMoveGUI(vecRightEnd, selectedObject);
				}
			}
		}

		else if (rightDragging)
		{
			//	Process order-right click commands
			if(selectedObject)
			{
				//	If a non-dragging event, process as a raycast
				//	NOTE: If no target is found, then use to discover move point
				if(mouseEnd.Equals(Vector2.zero))
				{
					//	Create a ray at the mouse
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					RaycastHit hitOut;

					//	If we hit a player controlled
					if (Physics.Raycast(ray, out hitOut))
					{
						PlayerControlled tempObj = hitOut.collider.gameObject.GetComponent<PlayerControlled>();
						if (tempObj != null)
						{
							//	Determine if enemy or 'alt' override
							if(PlayerManager.ThisPlayerID != tempObj.playerID || (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt)))
							{
								//	For each selected update target
								foreach (PlayerControlled i in selectedObjects)
								{
									//	As long as not self targeting
									if (!i.Equals(tempObj))
									{
										i.SetTarget(tempObj);
									}
								}
							}
						}
					}
					//	If we didnt get anything
					else
					{
						//	Get plane at selectedObject height
						Plane heightPlane = new Plane(Vector3.up, selectedObject.transform.position);

						//	And set move position to collision position
						float distance = 0f;
						if (heightPlane.Raycast(ray, out distance))
						{
							vecRightEnd = ray.GetPoint(distance);
						}
					}
				}

				//	If move-to position is not empty
				if(!vecRightEnd.Equals(Vector3.zero))
				{
					//	Apply
					Move(vecRightEnd);
					ActivateMoveGUI(false);
				}
			}

			//	Empty out variables
			rightDragging = false;
			mouseStart = Vector2.zero;
			mouseEnd = Vector2.zero;
			vecRightStart = Vector3.zero;
			vecRightEnd = Vector3.zero;
		}
    }

	//	Global accessor to add a pc to test list when the MeshHandler OnBecameVisible()
	public static void OnScreen(PlayerControlled pc)
	{
		List<PlayerControlled> result;
		if(thisPlayer.onScreen.TryGetValue (pc.playerID, out result))
		{
			result.Add (pc);
		}
		else
		{
			thisPlayer.onScreen.Add(pc.playerID, new List<PlayerControlled>(){pc});
		}
	}

	//	Global accessor to remove a pc from the test list when the MeshHandler OnBecameVisible()
	public static void OffScreen(PlayerControlled pc)
	{
		List<PlayerControlled> result;
		if(thisPlayer.onScreen.TryGetValue (pc.playerID, out result))
		{
			result.Remove (pc);
		}
	}


	//	Internal Reset
	void ClearPlayerControlled()
	{
		foreach (PlayerControlled off in selectedObjects)
		{
			off.Selected(false);
		}
		
		selectedObject = null;
		selectedObjects = new List<PlayerControlled>();
		GUISelectionManager.UpdatePanels();
	}

	//	Internal reset then add all
	void SetPlayerControlled(List<PlayerControlled> PlayerControlledIn)
	{
		ClearPlayerControlled ();
		AddPlayerControlled(PlayerControlledIn);
		GUISelectionManager.UpdatePanels();
	}

	//	Add a range of pc to selected
    void AddPlayerControlled(List<PlayerControlled> PlayerControlledIn)
    {
        foreach(PlayerControlled pc in PlayerControlledIn)
        {
			if (!selectedObjects.Contains (pc))
			{
				selectedObjects.Add (pc);
				pc.Selected (true);
				//	If there is no selected object update it to this
				if (!selectedObject)
				{
					selectedObject = pc;
				}
			}
        }
		GUISelectionManager.UpdatePanels ();
    }

	//	Remove a range of pc to selected
    void RemovePlayerControlled(List<PlayerControlled> PlayerControlledIn)
    {
        foreach(PlayerControlled pc in PlayerControlledIn)
        {
			if (selectedObjects.Remove(pc))
			{
				pc.Selected(false);
				//	If this was the selected object then set it to the first element, else null
				if (selectedObject.Equals(pc))
				{
					selectedObject = (selectedObjects.Count > 0) ? selectedObjects[0] : null;
				}
			}
        }
		GUISelectionManager.UpdatePanels();
    }

	//	When in use, enable all of the line renderers
	void ActivateMoveGUI(bool Enabled)
	{
		drawToMove.enabled = Enabled;
		drawMoveRad.gameObject.SetActive(Enabled);
		drawToRad.gameObject.SetActive(Enabled);
		drawFromRad.gameObject.SetActive(Enabled);
	}

	//	When in use, update the positions of the line renderers
    void UpdateMoveGUI(Vector3 ToPos, PlayerControlled Selected)
    {
		//	Update the 3 point line to reflect the start
		drawToMove.SetPosition(0, Selected.transform.position);

		//	Target
        drawToMove.SetPosition(1, ToPos);

		//	Then back to the new pos x and z, but origional height to show difference
        Vector3 belowVec = Selected.transform.position;
        belowVec.y = ToPos.y;
        drawToMove.SetPosition(2, belowVec);

		//	Set one radius to the ship size at its postion
        drawFromRad.SetRadius(Selected.GetComponent<CapsuleCollider>().height / 2);
        drawFromRad.transform.position = belowVec;

		//	Set one radius to the ships size at target
        drawToRad.SetRadius(Selected.GetComponent<CapsuleCollider>().height / 2);
        drawToRad.transform.position = ToPos;

		//	Set one radius to show the radius around the ship to give perspective
        drawMoveRad.SetRadius(Vector3.Distance(ToPos, belowVec));
        drawMoveRad.transform.position = belowVec;
    }

	//	Update the move position of all selected
	void Move(Vector3 TargetPosition)
	{
		//	If Shift add as a waypoint destination
		bool increment = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

		//	Apply the move to selectedObject directly
		selectedObject.SetMove(TargetPosition, increment);

		//	And for all other selected objs
		foreach (PlayerControlled pc in selectedObjects)
		{
			if(!pc.Equals(selectedObject))
			{
				//	Apply the move relative to selectedObj
				Vector3 relPos = pc.transform.position - selectedObject.transform.position;
				pc.SetMove(TargetPosition + relPos, increment);
			}
		}
	}
}
