using UnityEngine;
using UnityEngine.UI;

using OdWyer.RTS;

public class GUISelectedUnit : MonoBehaviour {

    public PlayerControlled selected = null;

    public Text selectedTitle;
    public Text selectedKillsLabel;
    public Text selectedKills;
    public Text selectedHealthLabel;
    public Text selectedHealth;
    public Text selectedSuppliesLabel;
    public Text selectedSupplies;

    public void UpdatePanel(PlayerControlled SelectedObject)
    {
        selected = SelectedObject;
        if (selected is Unit)
        {
            selectedKillsLabel.gameObject.SetActive(true);
            selectedKills.gameObject.SetActive(true);
            selectedSuppliesLabel.gameObject.SetActive(true);
            selectedSupplies.gameObject.SetActive(true);
        }
        else
        {
            selectedKillsLabel.gameObject.SetActive(false);
            selectedKills.gameObject.SetActive(false);
            selectedSuppliesLabel.gameObject.SetActive(false);
            selectedSupplies.gameObject.SetActive(false);
        }
    }

    public void Update()
    {
		if (!selected)
			return;

		selectedTitle.text = selected.name;

		HullBehaviour hull = selected.GetComponent<HullBehaviour>();
		if (hull)
		{
			selectedHealth.text = hull.CurrentHealth.ToString() + "/" + hull.MaxHealth.ToString();
			selectedSupplies.text = hull.CurrentSupply.ToString() + "/" + hull.MaxSupply.ToString();
		}

		TargetingBehaviour targeting = selected.GetComponent<TargetingBehaviour>();
		if(targeting)
			selectedKills.text = targeting.currentKills.ToString();
    }

    public void ActivateSubPanel(GameObject SubPanel)
    {

    }
}
