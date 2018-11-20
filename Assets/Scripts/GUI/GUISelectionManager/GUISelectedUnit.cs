using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

    void Update()
    {
        if(selected is Unit)
        {
            selectedTitle.text = selected.name;
            selectedKills.text = ((Unit) selected).currentKills.ToString();
            selectedHealth.text = selected.currentHealth.ToString() + "/" + ((Unit)selected).health.ToString();
            selectedSupplies.text = ((Unit)selected).currentSupply.ToString() + "/" + ((Unit)selected).supply.ToString();
        }
    }

    public void ActivateSubPanel(GameObject SubPanel)
    {

    }
}
