using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUISelectionAbility : MonoBehaviour {

    public PlayerControlled selected = null;

    public Text DamSec;
    public Text SupSec;
    public Text Range;

    public Button ActAbility;

    public void UpdatePanel(PlayerControlled SelectedObject)
    {
        selected = SelectedObject;
        if(selected is Unit)
        {
            Unit temp = selected as Unit;

            DamSec.gameObject.SetActive(true);
            DamSec.text = "" + temp.DamPerSec;
            SupSec.gameObject.SetActive(true);
            SupSec.text = "" + temp.SupPerSec;
            Range.gameObject.SetActive(true);
            Range.text = "" + temp.EngageDistance;

            if(temp.upgWeaponActivatable)
            {
                ActAbility.gameObject.SetActive(true);
            }
            else
            {
                ActAbility.gameObject.SetActive(false);
            }
        }
        else
        {
            DamSec.gameObject.SetActive(false);
            SupSec.gameObject.SetActive(false);
            Range.gameObject.SetActive(false);
        }
    }
}
