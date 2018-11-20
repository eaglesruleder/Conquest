using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUISelectionStat : MonoBehaviour {

    public PlayerControlled selected = null;

    public GameObject selectedTitle;
    public GameObject selectedKillsLabel;
    public GameObject selectedKills;
    public GameObject selectedHealthLabel;
    public GameObject selectedHealth;
    public GameObject selectedSuppliesLabel;
    public GameObject selectedSupplies;

    public void UpdatePanel(PlayerControlled SelectedObject)
    {
        selected = SelectedObject;
        if (selected is Unit)
        {
            selectedKillsLabel.SetActive(true);
            selectedKills.SetActive(true);
            selectedSuppliesLabel.SetActive(true);
            selectedSupplies.SetActive(true);
        }
        else
        {
            selectedKillsLabel.SetActive(false);
            selectedKills.SetActive(false);
            selectedSuppliesLabel.SetActive(false);
            selectedSupplies.SetActive(false);
        }
    }

    void Update()
    {
        if(selected is Unit)
        {
            selectedTitle.GetComponent<Text>().text = selected.pcName;
            selectedKills.GetComponent<Text>().text = ((Unit) selected).currentKills.ToString();
            selectedHealth.GetComponent<Text>().text = selected.currentHull.ToString() + "/" + ((Unit)selected).ReturnMaxHealth().ToString();
            selectedSupplies.GetComponent<Text>().text = ((Unit)selected).currentSupply.ToString() + "/" + ((Unit)selected).ReturnSupplyMax().ToString();
        }
    }
}
