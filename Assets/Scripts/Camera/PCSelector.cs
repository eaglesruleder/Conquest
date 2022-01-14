using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using OdWyer.Graphics;
using OdWyer.RTS;

namespace OdWyer.Control
{
    public class PCSelector : MonoBehaviour
    {
        static PCSelector thisPlayer;

        Dictionary<string, List<PlayerControlled>> onScreen = new Dictionary<string, List<PlayerControlled>>();
        public static List<PlayerControlled> PlayerOnScreen
        {
            get
            {
                if (!thisPlayer.onScreen.ContainsKey(PlayerManager.ThisPlayerID))
                    return new List<PlayerControlled>();

                return thisPlayer.onScreen[PlayerManager.ThisPlayerID];
            }
        }

        public static PlayerControlled selectedObject;
        public static List<PlayerControlled> selectedObjects;

        public static bool lockOut { get { return thisPlayer.leftDragging || thisPlayer.rightDragging; } }


        private Vector2 _mouseStart = Vector2.zero;
        private Vector2 _mouseEnd = Vector2.zero;

        internal bool leftDragging = false;
        internal bool rightDragging = false;

        private Vector3 _vecRightStart = Vector3.zero;
        private Vector3 _vecRightEnd = Vector3.zero;


        private LineRenderer _drawToMove;
        private CircleRenderer _drawMoveRad;
        private CircleRenderer _drawToRad;
        private CircleRenderer _drawFromRad;

        void Awake()
        {
            if (thisPlayer == null)
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

            _drawToMove = gameObject.AddComponent<LineRenderer>();
            _drawToMove.useWorldSpace = true;

            _drawToMove.material = new Material(Shader.Find("Unlit/Color"));
            _drawToMove.material.color = new Color(1f, 1f, 1f);

            _drawToMove.startWidth = 0.2f;
            _drawToMove.endWidth = 0.2f;

            _drawToMove.positionCount = 3;


            //	Create temp object with CircleRenderer attached
            GameObject temp = new GameObject();
            temp.AddComponent<CircleRenderer>();

            //	Instantiate for each CircleRenderer required
            _drawMoveRad = Instantiate(temp.GetComponent<CircleRenderer>()) as CircleRenderer;
            _drawMoveRad.Initialise();
            _drawToRad = Instantiate(temp.GetComponent<CircleRenderer>()) as CircleRenderer;
            _drawToRad.Initialise();
            _drawFromRad = Instantiate(temp.GetComponent<CircleRenderer>()) as CircleRenderer;
            _drawFromRad.Initialise();

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
                if (!_mouseEnd.Equals(Vector2.zero))
                {
                    mouseCur = _mouseEnd;
                    mouseCur.x = Input.mousePosition.x;
                }
                else
                {
                    return;
                }
            }

            //	ALL ABOUT THE LEFT
            //	If left is down, and we are not dragging right
            if (Input.GetMouseButton(0) && !rightDragging)
            {
                //	If this is a new event
                if (_mouseStart.Equals(Vector2.zero))
                {
                    //	Start as a new event
                    _mouseStart = mouseCur;
                    leftDragging = true;
                }

                //	Else if we are not yet dragging but the distance is > 5
                else if (_mouseEnd.Equals(Vector2.zero) && Vector2.Distance(_mouseStart, mouseCur) > 5)
                {
                    //	Start it as a dragging event
                    _mouseEnd = mouseCur;
                }

                //	On Drag
                else if (!_mouseEnd.Equals(Vector2.zero))
                {
                    _mouseEnd = mouseCur;

                    //	Create GUI Box
                    Rect box = new Rect(
                         (_mouseStart.x > _mouseEnd.x) ? _mouseEnd.x : _mouseStart.x
                        , Screen.height - ((_mouseStart.y > _mouseEnd.y) ? _mouseStart.y : _mouseEnd.y)
                        , (_mouseStart.x > _mouseEnd.x) ? _mouseStart.x - _mouseEnd.x : _mouseEnd.x - _mouseStart.x
                        , (_mouseStart.y > _mouseEnd.y) ? _mouseStart.y - _mouseEnd.y : _mouseEnd.y - _mouseStart.y);
                    GUI.Box(box, "");
                }
            }

            //	Else if we are dragging left (without input)
            else if (leftDragging)
            {
                List<PlayerControlled> tempObjs = new List<PlayerControlled>();

                //	If a non-dragging event, process as a raycast
                if (_mouseEnd.Equals(Vector2.zero))
                {
                    //	Create ray in direction of mouse
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitOut;

                    //	If we hit something
                    if (Physics.Raycast(ray, out hitOut))
                    {
                        //	And its a PlayerControlled
                        if (hitOut.collider.gameObject.GetComponent<PlayerControlled>())
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
                        (_mouseStart.x > _mouseEnd.x) ? _mouseEnd.x : _mouseStart.x
                        , (_mouseStart.y > _mouseEnd.y) ? _mouseEnd.y : _mouseStart.y
                        , (_mouseStart.x > _mouseEnd.x) ? _mouseStart.x - _mouseEnd.x : _mouseEnd.x - _mouseStart.x
                        , (_mouseStart.y > _mouseEnd.y) ? _mouseStart.y - _mouseEnd.y : _mouseEnd.y - _mouseStart.y
                        );

                    //	Get all objects inside the selection box
                    foreach (PlayerControlled on in PlayerOnScreen)
                    {
                        CapsuleCollider onColl = on.GetComponent<CapsuleCollider>();
                        Vector3 forward = on.transform.forward * onColl.height / 2;
                        Vector3 right = on.transform.right * onColl.radius;

                        //	Check if the object exists inside the selection box
                        if (rect.Contains(Camera.main.WorldToScreenPoint(on.transform.position))
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
                _mouseStart = Vector2.zero;
                _mouseEnd = Vector2.zero;
            }

            //	ELSE ITS ALL ABOUT THE RIGHTCLICKS

            //	If right is down, and we are not dragging left
            else if (Input.GetMouseButton(1) && !leftDragging)
            {
                //	Process order-right click commands
                if (selectedObject)
                {
                    //	If this is a new event
                    if (_mouseStart.Equals(Vector2.zero))
                    {
                        //	Start as a new event
                        _mouseStart = mouseCur;
                        rightDragging = true;
                    }

                    //	Else if we are not yet dragging but the distance is > 5
                    else if (_mouseEnd.Equals(Vector2.zero) && Vector2.Distance(_mouseStart, mouseCur) > 5)
                    {
                        _mouseEnd = mouseCur;

                        //	Create a ray aimed at mouse
                        Ray ray = Camera.main.ScreenPointToRay(_mouseStart);

                        //	Create a plane at selectedObject height aimed up
                        Plane collPlane = new Plane(Vector3.up, selectedObject.transform.position);

                        //	Shoot the ray at the plane, to determine location
                        float distance = 0f;
                        if (collPlane.Raycast(ray, out distance))
                        {
                            _vecRightStart = ray.GetPoint(distance);
                        }

                        //	Apply
                        UpdateMoveGUI(_vecRightStart, selectedObject);
                        ActivateMoveGUI(true);
                    }

                    //	Else this is a drag event
                    else if (!_mouseEnd.Equals(Vector2.zero))
                    {
                        _mouseEnd = mouseCur;

                        //	Create a normal from the camera, at the startPosition, thats perfectly vertical
                        Vector3 cameraPos = Camera.main.transform.position;
                        Vector3 planeNormal = new Vector3(cameraPos.x - _vecRightStart.x, 0f, cameraPos.z - _vecRightStart.z).normalized;

                        //	Create a plane thats aimed at the camera to point at
                        Plane collHeightPlane = new Plane(planeNormal, _vecRightStart);

                        //	Create a ray, at start-width, and current-height
                        Ray rayHeight = Camera.main.ScreenPointToRay(new Vector2(_mouseStart.x, _mouseEnd.y));

                        //	Shoot the ray at the plane, to determine location
                        float distance = 0f;
                        if (collHeightPlane.Raycast(rayHeight, out distance))
                        {
                            Vector3 testPos = rayHeight.GetPoint(distance);
                            //	If the position is between -20 and 20 apply it
                            if (20 > testPos.y && testPos.y > -20)
                            {
                                _vecRightEnd = testPos;
                            }
                        }

                        //	Apply
                        UpdateMoveGUI(_vecRightEnd, selectedObject);
                    }
                }
            }

            else if (rightDragging)
            {
                //	Process order-right click commands
                if (selectedObject)
                {
                    //	If a non-dragging event, process as a raycast
                    //	NOTE: If no target is found, then use to discover move point
                    if (_mouseEnd.Equals(Vector2.zero))
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
                                if (PlayerManager.ThisPlayerID != tempObj.playerID || (Input.GetKey(KeyCode.RightAlt) || Input.GetKey(KeyCode.LeftAlt)))
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
                                _vecRightEnd = ray.GetPoint(distance);
                            }
                        }
                    }

                    //	If move-to position is not empty
                    if (!_vecRightEnd.Equals(Vector3.zero))
                    {
                        //	Apply
                        Move(_vecRightEnd);
                        ActivateMoveGUI(false);
                    }
                }

                //	Empty out variables
                rightDragging = false;
                _mouseStart = Vector2.zero;
                _mouseEnd = Vector2.zero;
                _vecRightStart = Vector3.zero;
                _vecRightEnd = Vector3.zero;
            }
        }

        //	Global accessor to add a pc to test list when the MeshHandler OnBecameVisible()
        public static void OnScreen(PlayerControlled pc)
        {
            List<PlayerControlled> result;
            if (thisPlayer.onScreen.TryGetValue(pc.playerID, out result))
            {
                result.Add(pc);
            }
            else
            {
                thisPlayer.onScreen.Add(pc.playerID, new List<PlayerControlled>() { pc });
            }
        }

        //	Global accessor to remove a pc from the test list when the MeshHandler OnBecameVisible()
        public static void OffScreen(PlayerControlled pc)
        {
            List<PlayerControlled> result;
            if (thisPlayer.onScreen.TryGetValue(pc.playerID, out result))
            {
                result.Remove(pc);
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
            ClearPlayerControlled();
            AddPlayerControlled(PlayerControlledIn);
            GUISelectionManager.UpdatePanels();
        }

        //	Add a range of pc to selected
        void AddPlayerControlled(List<PlayerControlled> PlayerControlledIn)
        {
            foreach (PlayerControlled pc in PlayerControlledIn)
            {
                if (!selectedObjects.Contains(pc))
                {
                    selectedObjects.Add(pc);
                    pc.Selected(true);
                    //	If there is no selected object update it to this
                    if (!selectedObject)
                    {
                        selectedObject = pc;
                    }
                }
            }
            GUISelectionManager.UpdatePanels();
        }

        //	Remove a range of pc to selected
        void RemovePlayerControlled(List<PlayerControlled> PlayerControlledIn)
        {
            foreach (PlayerControlled pc in PlayerControlledIn)
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
            _drawToMove.enabled = Enabled;
            _drawMoveRad.gameObject.SetActive(Enabled);
            _drawToRad.gameObject.SetActive(Enabled);
            _drawFromRad.gameObject.SetActive(Enabled);
        }

        //	When in use, update the positions of the line renderers
        void UpdateMoveGUI(Vector3 ToPos, PlayerControlled Selected)
        {
            //	Update the 3 point line to reflect the start
            _drawToMove.SetPosition(0, Selected.transform.position);

            //	Target
            _drawToMove.SetPosition(1, ToPos);

            //	Then back to the new pos x and z, but origional height to show difference
            Vector3 belowVec = Selected.transform.position;
            belowVec.y = ToPos.y;
            _drawToMove.SetPosition(2, belowVec);

            //	Set one radius to the ship size at its postion
            _drawFromRad.radius = Selected.GetComponent<CapsuleCollider>().height / 2;
            _drawFromRad.transform.position = belowVec;

            //	Set one radius to the ships size at target
            _drawToRad.radius = Selected.GetComponent<CapsuleCollider>().height / 2;
            _drawToRad.transform.position = ToPos;

            //	Set one radius to show the radius around the ship to give perspective
            _drawMoveRad.radius = Vector3.Distance(ToPos, belowVec);
            _drawMoveRad.transform.position = belowVec;
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
                if (!pc.Equals(selectedObject))
                {
                    //	Apply the move relative to selectedObj
                    Vector3 relPos = pc.transform.position - selectedObject.transform.position;
                    pc.SetMove(TargetPosition + relPos, increment);
                }
            }
        }
    }
}