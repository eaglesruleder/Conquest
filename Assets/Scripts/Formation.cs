using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Formation : MonoBehaviour
{
    const float deltaAngle = 30;
    const float delta3DAngle = 30;

    public static void DeltaFormation(List<PlayerControlled> units, Vector3 forward, float spreadDistance)
    {
 //       SortedList<int, List<PlayerControlled>> sortedUnits = SortUnits(units);
   //     int layers = 
    }

	public static Vector3 DeltaFormation(int position, int totalPositions, Vector3 forward, float spreadDistance)
    {
        position++;

        forward.y = 0;
        forward.Normalize();
        Vector3 back = forward * -1 * spreadDistance;
        Vector3 right = Vector3.Cross(forward, Vector3.up).normalized * ((position % 2 == 1) ? 1 : -1) * spreadDistance;

        int pushPosition = Mathf.CeilToInt(position / 2);

        Vector3 result = Vector3.zero;
        result += back * Mathf.Cos(deltaAngle * Mathf.Deg2Rad) * (pushPosition - totalPositions / 4);
        result += right * Mathf.Sin(deltaAngle * Mathf.Deg2Rad) * pushPosition;

        return result;
    }

    public static Vector3 Delta3DFormation(int position, int totalPositions, Vector3 forward, float spreadDistance)
    {
        forward.y = 0;
        forward.Normalize();
        Vector3 back = forward * -1 * spreadDistance;
        Vector3 right = Vector3.Cross(forward, Vector3.up);

        int ring = 0;
        int ringEndPos = 0;
        int slots = 0;
        float radius = 0;
        bool escape = false;

        while(!escape)
        {
            ring++;
            radius = Mathf.Sin(delta3DAngle * Mathf.Deg2Rad) * ring * spreadDistance;
            float circumfrence = Mathf.PI * radius * radius;
            slots = Mathf.CeilToInt(circumfrence / spreadDistance);
            if(position <= ringEndPos + slots)
            {
                escape = true;
            }
            else
            {
                ringEndPos += slots;
            }
        }

        float circAngle = 360 / slots * (position - ringEndPos);

        Vector3 result = Vector3.zero;
        result += Vector3.up * Mathf.Sin(circAngle * Mathf.Deg2Rad) * radius;
        result += back * Mathf.Cos(delta3DAngle * Mathf.Deg2Rad) * ring;
        result += right * Mathf.Cos(circAngle * Mathf.Deg2Rad) * radius;
        return result;
    }

    public static Vector3 BoxFormation(int position, int totalPositions, Vector3 forward, float spreadDistance)
    {
        position++;

        forward.y = 0;
        forward.Normalize();
        Vector3 back = forward * -1;
        Vector3 right = Vector3.Cross(forward, Vector3.up);

        int valence = Mathf.CeilToInt(Mathf.Pow(position, 1f/2f));
        int valencePos = position - Mathf.FloorToInt(Mathf.Pow(valence - 1, 2));
        int offsetValencePos = (Mathf.FloorToInt(valence / 2) - valencePos + ((valence % 2 == 1) ? 0 : 1)) * ((valence % 2 == 1) ? 1 : -1);
        int rightValencePos = (Mathf.FloorToInt(valence / 2) - valence + ((valence % 2 == 1) ? 0 : 1)) * ((valence % 2 == 1) ? 1 : -1);

        Vector3 result = Vector3.zero;
        if(valencePos < valence)
        {
            result += back * valence;
            result += right * offsetValencePos;
        }
        else if (valencePos == valence)
        {
            result += back * valence;
            result += right * rightValencePos;
        }
        else
        {
            result += back * (valence - (valencePos - valence));
            result += right * rightValencePos;
        }

        return result;
    }

    public static Vector3 Box3DFormation(int position, int totalPositions, Vector3 forward, float spreadDistance)
    {
        return Vector3.zero;
    }

    public static Vector3 CircleFormation(int position, int totalPositions, Vector3 forward, float spreadDistance)
    {
        return Vector3.zero;
    }

    public static Vector3 Circle3DFormation(int position, int totalPositions, Vector3 forward, float spreadDistance)
    {
        return Vector3.zero;
    }
}
