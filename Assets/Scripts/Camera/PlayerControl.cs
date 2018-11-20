using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float screenMarginPercent = 5;
    float screenMarginMin;
    float screenMarginMax;
    public int ScrollSpeed = 25;

    public int HeightSpeed = 20;
    public int HeightMin = 25;
    public int HeightMax = 250;

    public int PanSpeed = 50;
    public int PanAngleMin = 0;
    public int PanAngleMax = 45;

    public int levelSize = 250;

    void Awake()
    {
        screenMarginMin = (0 + screenMarginPercent) / 100;
        screenMarginMax = (100 - screenMarginPercent) / 100;
    }

    void Update()
    {
        if (!PCSelector.lockOut)
        {
            // Init camera translation for this frame.
            Vector3 translation = Vector3.zero;

			//	Determine step distance
			float step =  ScrollSpeed * Time.deltaTime;

            //Mouse Influence
            //When middle-click
            if (Input.GetMouseButton(2))
			{
				//	Rotate around 'up'
				//	Works with CameraSway but this obj needs to be perfectly horizontal for movement
				transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X"));
			}

            //Else determine screen margin influence.
            else
            {
                // Move camera if mouse pointer reaches screen borders
                if (Input.mousePosition.x <= Screen.width * screenMarginMin)
					translation += transform.right * -step;

                if (Input.mousePosition.x >= Screen.width * screenMarginMax)
					translation += transform.right * step;

                if (Input.mousePosition.y <= Screen.height * screenMarginMin)
					translation += transform.forward * -step;

                if (Input.mousePosition.y >= Screen.height * screenMarginMax)
					translation += transform.forward * step;
            }

            //Keyboard Influence
			translation += transform.right * Input.GetAxis("Horizontal") * step;
			translation += transform.forward * Input.GetAxis("Vertical") * step;

            //Mouse Wheel Influence
            float zoom = Input.GetAxis("Mouse ScrollWheel") * HeightSpeed * Time.deltaTime;
            if (zoom != 0)
                translation += Vector3.up * -HeightSpeed * zoom;

            //Keep camera within level and zoom area
            Vector3 levelLimits = transform.position + translation;

			//	If outside the radius
            if (Vector3.Distance(levelLimits, Vector3.zero) > levelSize)
            {
                translation.z = 0;
                translation.x = 0;
            }

			//	If below threshhold zero out
			if (levelLimits.y < HeightMin)
			{
                translation.y = 0;
			}
			//	PLACEHOLDER for when universe zoom
			else if(levelLimits.y > HeightMax)
			{
				translation.y = 0;
			}

            // Apply
            transform.position += translation;
        }
    }
}