using UnityEngine;
using UnityEngine.UI;

using OdWyer.RTS;

public class GUISelectionAbility : MonoBehaviour {

    public PlayerControlled selected = null;

    public Text DamSec;
    public Text SupSec;
    public Text Range;

    public Button ActAbility;

    public void UpdatePanel(PlayerControlled SelectedObject)
    {
        selected = SelectedObject;

		TargetingBehaviour targeting = selected.GetComponent<TargetingBehaviour>();
        if(targeting)
        {
            Unit temp = selected as Unit;

            DamSec.gameObject.SetActive(true);
            DamSec.text = "" + targeting.DamPerSec;
            SupSec.gameObject.SetActive(true);
            SupSec.text = "" + targeting.SupPerSec;
            Range.gameObject.SetActive(true);
            Range.text = "" + targeting.EngageDistance;

            ActAbility.gameObject.SetActive(false);
        }
        else
        {
            DamSec.gameObject.SetActive(false);
            SupSec.gameObject.SetActive(false);
            Range.gameObject.SetActive(false);
        }
    }
}
