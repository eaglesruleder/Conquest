using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GUIPlayer : MonoBehaviour{
	
	public static int HUMAN_TECH = 0;
	public static int MANTIS_TECH = 1;
	public static int CERILION_TECH = 2;
	public Technology[] templateTechs;

	public string playerID = "player";
	public Technology[] playerTechs;

	public List<PlayerControlled> onScreen = new List<PlayerControlled>();

	public GUISelectionManager selectManager;

	public PlayerControlled selectedObject = null;
	public List<PlayerControlled> selectedObjects = new List<PlayerControlled>();
	
	Vector2 mouseStart = Vector2.zero;
	Vector2 mouseEnd = Vector2.zero;
	bool mouseDragging = false;

    Vector2 mouseRightStart = Vector2.zero;
    Vector3 vecRightStart = Vector3.zero;
    Vector3 vecRightEnd = Vector3.zero;
    bool mouseRightDragging = false;

    LineRenderer drawToMove;
    CircleRenderer drawMoveRad;
    CircleRenderer drawToRad;
    CircleRenderer drawFromRad;

    void Awake()
	{
        drawToMove = gameObject.AddComponent<LineRenderer>();
        drawToMove.useWorldSpace = true;
        drawToMove.material = new Material(Shader.Find("Unlit/Texture"));
        drawToMove.material.color = new Color(1f, 0.92f, 0.016f, 0.5f);
        drawToMove.SetWidth(0.2f, 0.2f);
        drawToMove.SetVertexCount(3);

        GameObject temp = new GameObject();
        temp.AddComponent<CircleRenderer>();
        drawMoveRad = Instantiate(temp.GetComponent<CircleRenderer>()) as CircleRenderer;
        drawMoveRad.Initialise();
        drawToRad = Instantiate(temp.GetComponent<CircleRenderer>()) as CircleRenderer;
        drawToRad.Initialise();
        drawFromRad = Instantiate(temp.GetComponent<CircleRenderer>()) as CircleRenderer;
        drawFromRad.Initialise();
        DestroyImmediate(temp);

        ActivateMoveGUI(false);

        playerTechs = new Technology[templateTechs.Length];
		for(int i = 0; i < templateTechs.Length; i++)
		{
			templateTechs[i].Initialise();
			playerTechs[i] = templateTechs[i];
		}
	}

    void OnGUI()
    {
        // If the mouse button is down
        if (Input.GetMouseButton(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mouseCur = Input.mousePosition;
                if (!mouseDragging)
                {
                    if (mouseStart.Equals(Vector2.zero))
                    {
                        mouseStart = mouseCur;
                    }
                    else if (!mouseStart.Equals(mouseCur))
                    {
                        mouseEnd = mouseCur;
                        mouseDragging = true;
                    }
                }
                else
                {
                    mouseEnd = mouseCur;
                }
            }

            if (mouseDragging)
            {
                float top = 0;
                float left = 0;
                float width = 0;
                float height = 0;

                if (mouseEnd.x - mouseStart.x >= 0)
                {
                    left = mouseStart.x;
                    width = mouseEnd.x - mouseStart.x;
                }
                else
                {
                    left = mouseEnd.x;
                    width = mouseStart.x - mouseEnd.x;
                }

                if (mouseStart.y >= mouseEnd.y)
                {
                    top = mouseStart.y;
                    height = mouseStart.y - mouseEnd.y;
                }
                else
                {
                    top = mouseEnd.y;
                    height = mouseEnd.y - mouseStart.y;
                }
                top = Screen.height - top;

                Rect box = new Rect(left, top, width, height);
                GUI.Box(box, "");
            }
        }
        // Else if its not down, but there are values retained:

        // If there is both a start and end, process as a box
        else if (!mouseStart.Equals(Vector2.zero) && !mouseEnd.Equals(Vector2.zero))
        {
            List<PlayerControlled> tempObjs = new List<PlayerControlled>();

            // Find the bounds
            bool startxIsMax = mouseStart.x > mouseEnd.x;
            bool startYIsMax = mouseStart.y > mouseEnd.y;
            Vector2 maxValues = new Vector2((startxIsMax) ? mouseStart.x : mouseEnd.x, (startYIsMax) ? mouseStart.y : mouseEnd.y);
            Vector2 minValues = new Vector2((startxIsMax) ? mouseEnd.x : mouseStart.x, (startYIsMax) ? mouseEnd.y : mouseStart.y);

            // Get all objects inside the selection box
            foreach (PlayerControlled on in onScreen)
            {
                Vector3 tempLoc = Camera.main.GetComponent<Camera>().WorldToScreenPoint(on.transform.position);
                if ((maxValues.x >= tempLoc.x && tempLoc.x >= minValues.x) && (maxValues.y >= tempLoc.y && tempLoc.y >= minValues.y))
                {
                    tempObjs.Add(on);
                }
            }

            // If Ctrl then remove objects from list
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftControl))
            {
                RemovePlayerControlled(tempObjs);
            }

            // Else If Shift then Add objects to list
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                AddPlayerControlled(tempObjs);
            }

            // Otherwise consider selection as 'From Scratch'
            else
            {
                SetPlayerControlled(tempObjs);
            }

            mouseDragging = false;
            mouseStart = Vector2.zero;
            mouseEnd = Vector2.zero;
        }

        // Else if there is only a start, process as a raycast
        else if (!mouseStart.Equals(Vector2.zero))
        {
            PlayerControlled tempObj = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitOut;
            if (Physics.Raycast(ray, out hitOut))
            {
                tempObj = hitOut.collider.gameObject.GetComponent<PlayerControlled>();
            }

            // If Ctrl then remove objects from list
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                RemovePlayerControlled(tempObj);
            }

            // Else If Shift then Add objects to list
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                AddPlayerControlled(tempObj);
                selectedObject = tempObj;
            }

            // Otherwise consider selection as 'From Scratch'
            else
            {
                SetPlayerControlled(tempObj);
            }

            mouseDragging = false;
            mouseStart = Vector2.zero;
            mouseEnd = Vector2.zero;
        }


        // Else if its all about the Right button:

        // If the Right button is down
        else if ((Input.GetMouseButton(1) || Input.GetMouseButtonDown(1)) && selectedObject)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector2 mouseRightCur = Input.mousePosition;
                if (!mouseRightDragging)
                {
                    if (mouseRightStart.Equals(Vector2.zero))
                    {
                        mouseRightStart = mouseRightCur;
                    }
                    else if (!mouseRightStart.Equals(mouseRightCur))
                    {
                        mouseRightDragging = true;

                        Ray ray = Camera.main.ScreenPointToRay(mouseRightStart);

                        Plane collPlane = new Plane(Vector3.up, selectedObject.transform.position);
                        float distance = 0f;
                        if (collPlane.Raycast(ray, out distance))
                        {
                            vecRightStart = ray.GetPoint(distance);
                        }

                        vecRightEnd = vecRightStart;

                        UpdateMoveGUI(vecRightEnd, selectedObject);
                        ActivateMoveGUI(true);
                    }
                }
                else
                {
                    Vector3 cameraPos = Camera.main.transform.position;
                    Vector3 planeNormal = new Vector3(cameraPos.x - vecRightStart.x, 0f, cameraPos.z - vecRightStart.z).normalized;

                    Plane collHeightPlane = new Plane(planeNormal, vecRightStart);

                    Vector2 mouseRightHeight = mouseRightCur;
                    mouseRightHeight.x = mouseRightStart.x;
                    Ray rayHeight = Camera.main.ScreenPointToRay(mouseRightHeight);

                    float distance = 0f;
                    if (collHeightPlane.Raycast(rayHeight, out distance))
                    {
                        Vector3 testPos = rayHeight.GetPoint(distance);
                        if(20 > testPos.y && testPos.y > -20)
                        {
                            vecRightEnd = testPos;
                        }
                    }

                    UpdateMoveGUI(vecRightEnd, selectedObject);
                }
            }
        }

        // Else if there is an end-position, process as a drag movement
        else if (!vecRightEnd.Equals(Vector3.zero))
        {
            Move(vecRightEnd);

            ActivateMoveGUI(false);

            mouseRightStart = Vector3.zero;
            vecRightStart = Vector3.zero;
            vecRightEnd = Vector3.zero;
            mouseRightDragging = false;
        }

        // Else if there is a start position, process as a raycast for movement or target selection
        else if (!mouseRightStart.Equals(Vector2.zero))
        {
            PlayerControlled tempObj = null;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitOut;
            if (Physics.Raycast(ray, out hitOut))
            {
                tempObj = hitOut.collider.gameObject.GetComponent<PlayerControlled>();
            }
            else if (selectedObject)
            {
                Plane heightPlane = new Plane(Vector3.up, selectedObject.transform.position);
                float distance = 0f;
                if (heightPlane.Raycast(ray, out distance))
                {
                    Move(ray.GetPoint(distance));
                }
            }

            if (tempObj != null)
            {
                bool pass = (playerID == tempObj.playerID) ? (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt)) : true;
                foreach (PlayerControlled i in selectedObjects)
                {
                    if (!i.Equals(tempObj) && pass)
                    {
                        i.SetTarget(tempObj);
                    }
                }
            }
            mouseRightDragging = false;
            mouseRightStart = Vector2.zero;
            vecRightStart = Vector3.zero;
            vecRightEnd = Vector3.zero;
        }

        // Finally as a error catcher, if none of the else-if checks above, but still values, something went wrong so reset
        else if (mouseStart != Vector2.zero || mouseEnd != Vector2.zero || mouseDragging || mouseRightStart != Vector2.zero || vecRightStart != Vector3.zero || vecRightEnd != Vector3.zero || mouseRightDragging)
        {
            selectManager.UpdatePanels();
            mouseDragging = false;
            mouseStart = Vector2.zero;
            mouseEnd = Vector2.zero;
            mouseRightDragging = false;
            mouseRightStart = Vector2.zero;
            vecRightStart = Vector3.zero;
            vecRightEnd = Vector3.zero;
        }
    }

    public void AddPlayerControlled(List<PlayerControlled> PlayerControlledIn)
    {
        foreach(PlayerControlled pc in PlayerControlledIn)
        {
            AddPlayerControlled(pc);
        }
    }

    public void AddPlayerControlled(PlayerControlled PlayerControlledIn)
    {
        if (!selectedObjects.Contains(PlayerControlledIn))
        {
            selectedObjects.Add(PlayerControlledIn);
            PlayerControlledIn.Selected(true);
            if(!selectedObject)
            {
                selectedObject = PlayerControlledIn;
                selectManager.UpdatePanels();
            }
        }
    }

    public bool RemovePlayerControlled(List<PlayerControlled> PlayerControlledIn)
    {
        bool result = true;
        foreach(PlayerControlled pc in PlayerControlledIn)
        {
            if(RemovePlayerControlled(pc))
            {
                result = false;
            }
        }
        return result;
    }

    public bool RemovePlayerControlled(PlayerControlled PlayerControlledIn)
    {
        if (selectedObjects.Remove(PlayerControlledIn))
        {
            PlayerControlledIn.Selected(false);
            if (selectedObject.Equals(PlayerControlledIn))
            {
                selectedObject = (selectedObjects.Count > 0) ? selectedObjects[0] : null;
            }
            selectManager.UpdatePanels();
            return true;
        }
        return false;
    }

    public void SetPlayerControlled(List<PlayerControlled> PlayerControlledIn)
    {
        foreach(PlayerControlled off in selectedObjects)
        {
            off.Selected(false);
        }

        selectedObject = null;
        selectedObjects = new List<PlayerControlled>();

        AddPlayerControlled(PlayerControlledIn);
        selectManager.UpdatePanels();
    }

    public void SetPlayerControlled(PlayerControlled PlayerControlledIn)
    {
        foreach (PlayerControlled off in selectedObjects)
        {
            off.Selected(false);
        }

        selectedObject = null;
        selectedObjects = new List<PlayerControlled>();

        AddPlayerControlled(PlayerControlledIn);
        selectManager.UpdatePanels();
    }

    private void Move(Vector3 TargetPosition)
    {
        bool increment = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        selectedObject.SetMove(TargetPosition, increment);
        foreach (PlayerControlled pc in selectedObjects)
        {
            if(!pc.Equals(selectedObject))
            {
                Vector3 relPos = pc.transform.position - selectedObject.transform.position;
                pc.SetMove(TargetPosition + relPos, increment);
            }
        }
    }

    private void UpdateMoveGUI(Vector3 ToPos, PlayerControlled Selected)
    {
        drawToMove.SetPosition(0, selectedObject.transform.position);
        drawToMove.SetPosition(1, ToPos);
        Vector3 belowVec = Selected.transform.position;
        belowVec.y = ToPos.y;
        drawToMove.SetPosition(2, belowVec);

        drawFromRad.SetRadius(Selected.selectionHeight / 2);
        drawFromRad.transform.position = belowVec;

        drawToRad.SetRadius(Selected.selectionHeight / 2);
        drawToRad.transform.position = ToPos;

        drawMoveRad.SetRadius(Vector3.Distance(ToPos, belowVec));
        drawMoveRad.transform.position = belowVec;
    }

    private void ActivateMoveGUI(bool Enabled)
    {
        drawToMove.enabled = Enabled;
        drawMoveRad.gameObject.SetActive(Enabled);
        drawToRad.gameObject.SetActive(Enabled);
        drawFromRad.gameObject.SetActive(Enabled);
    }
}
