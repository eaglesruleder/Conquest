using UnityEngine;

using OdWyer.Control;

public class PlayerControl : MonoBehaviour
{
    public float screenMarginPercent = 5;
    public float screenMarginMin
        { get { return (0 + screenMarginPercent) / 100; } }
    public  float screenMarginMax
        { get { return (100 - screenMarginPercent) / 100; } }

    public int scrollSpeed = 25;

    public int heightSpeed = 20;
    public int heightMin = 25;
    public int heightMax = 250;

    public float levelSize = 250;
    private float _levelSizeSquare;

    void Awake()
    {
        _levelSizeSquare = levelSize * levelSize;
    }

    void Update()
    {
        if (!PCSelector.lockOut)
        {
            if (Input.GetMouseButton(2))
                transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X"));
            else
            {
                Vector3 translation = GetUserTranslation();
                translation = ClampToLimits(translation);

                transform.position += translation;
            }
        }
    }

    Vector3 GetUserTranslation()
    {
        Vector3 translation = Vector3.zero;

        float step = scrollSpeed * Time.deltaTime;

        if (Input.mousePosition.x <= Screen.width * screenMarginMin)
            translation += transform.right * -step;

        else if (Input.mousePosition.x >= Screen.width * screenMarginMax)
            translation += transform.right * step;

        if (Input.mousePosition.y <= Screen.height * screenMarginMin)
            translation += transform.forward * -step;

        else if (Input.mousePosition.y >= Screen.height * screenMarginMax)
            translation += transform.forward * step;
        
        translation += transform.right * Input.GetAxis("Horizontal") * step;
        translation += transform.forward * Input.GetAxis("Vertical") * step;

        float zoom = Input.GetAxis("Mouse ScrollWheel") * heightSpeed * Time.deltaTime;
        if (zoom != 0)
            translation += Vector3.up * -heightSpeed * zoom;

        return translation;
    }

    Vector3 ClampToLimits(Vector3 position)
    {
        if (position.sqrMagnitude > _levelSizeSquare)
        {
            position.z = 0;
            position.x = 0;
        }

        if (position.y < heightMin
        ||  position.y > heightMax)
            position.y = 0;

        return position;
    }
}