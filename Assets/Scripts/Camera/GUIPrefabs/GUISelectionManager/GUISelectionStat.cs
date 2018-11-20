using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUISelectionStat : MonoBehaviour {

    public Unit selectedUnit = null;
    public Structure selectedStructure = null;

    public GameObject selectedTitle;
    public GameObject selectedKillsLabel;
    public GameObject selectedKills;
    public GameObject selectedHealthLabel;
    public GameObject selectedHealth;
    public GameObject selectedSuppliesLabel;
    public GameObject selectedSupplies;

    public void UpdatePanel(PlayerControlled SelectedObject)
    {
        if (SelectedObject != null)
        {
            if (SelectedObject.unitComponent)
            {
                selectedUnit = SelectedObject.unitComponent;
                selectedStructure = null;

                selectedKillsLabel.SetActive(true);
                selectedKills.SetActive(true);
                selectedSuppliesLabel.SetActive(true);
                selectedSupplies.SetActive(true);
            }
            else if (SelectedObject.structureComponent)
            {
                selectedStructure = SelectedObject.structureComponent;
                selectedUnit = null;

                selectedKillsLabel.SetActive(false);
                selectedKills.SetActive(false);
                selectedSuppliesLabel.SetActive(false);
                selectedSupplies.SetActive(false);
            }
        }
    }

    void Update()
    {
        if(selectedUnit != null)
        {
            selectedTitle.GetComponent<Text>().text = selectedUnit.name;
            selectedKills.GetComponent<Text>().text = selectedUnit.Kills.ToString();
            selectedHealth.GetComponent<Text>().text = selectedUnit.hull.currentHull.ToString() + "/" + selectedUnit.hull.ReturnHullMax().ToString();
        }
    }
}
