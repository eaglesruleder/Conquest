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
        if (!(Input.GetMouseButton(0) || Input.GetMouseButton(1)))
        {
            // Init camera translation for this frame.
            Vector3 translation = Vector3.zero;

            //Mouse Influence
            //When middle-click rotate
            if (Input.GetMouseButton(2))
                transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X"));
            //Else determine screen margin influence.
            else
            {
                // Move camera if mouse pointer reaches screen borders
                if (Input.mousePosition.x <= Screen.width * screenMarginMin)
                    translation += transform.right * -ScrollSpeed * Time.deltaTime;

                if (Input.mousePosition.x >= Screen.width * screenMarginMax)
                    translation += transform.right * ScrollSpeed * Time.deltaTime;

                if (Input.mousePosition.y <= Screen.height * screenMarginMin)
                    translation += transform.forward * -ScrollSpeed * Time.deltaTime;

                if (Input.mousePosition.y >= Screen.height * screenMarginMax)
                    translation += transform.forward * ScrollSpeed * Time.deltaTime;
            }

            //Keyboard Influence
            float translationStep = 2 * ScrollSpeed * Time.deltaTime;
            translation += transform.right * Input.GetAxis("Horizontal") * translationStep;
            translation += transform.forward * Input.GetAxis("Vertical") * translationStep;

            //Mouse Wheel Influence

            float zoom = Input.GetAxis("Mouse ScrollWheel") * HeightSpeed * Time.deltaTime;
            if (zoom != 0)
                translation += Vector3.up * -HeightSpeed * zoom;

            //Keep camera within level and zoom area
            Vector3 levelLimits = transform.position + translation;

            if ((levelLimits.x * levelLimits.x) + (levelLimits.z * levelLimits.z) > (levelSize * levelSize))
            {
                translation.z = 0;
                translation.x = 0;
            }

            if (!(HeightMin < levelLimits.y || levelLimits.y < HeightMax))
                translation.y = 0;

            // Finally move camera parallel to world axis
            transform.position += translation;
        }
    }
}