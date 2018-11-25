using UnityEngine;

public class GUISelectionManager : MonoBehaviour {

	static GUISelectionManager thisGUISelectionManager;

	public GameObject TechBtn;
	public GUITech TechPanel;

	public GameObject AllSelectedUnitsBtn;
    public GUIAllSelectedUnits AllSelectedUnitsPanel;

	public GameObject SelectedUnitBtn;
	public GUISelectedUnit SelectedUnitPanel;

	void Awake()
	{
		if(thisGUISelectionManager == null)
			thisGUISelectionManager = this;
		
		else
			Destroy(this.gameObject);
	}

    public static void UpdatePanels()
    {
		//TechPanel.UpdatePanel(GUIPlayer.PlayerOnScreen);
		thisGUISelectionManager.SelectedUnitPanel.UpdatePanel(PCSelector.selectedObject);
		thisGUISelectionManager.AllSelectedUnitsPanel.UpdatePanel(PCSelector.selectedObjects);

        // Update is applies when selection changes, so on new items
		if(PCSelector.selectedObjects.Count == 1)
        {
			thisGUISelectionManager.SelectedUnitBtn.SetActive(true);
			thisGUISelectionManager.AllSelectedUnitsBtn.SetActive(false);
			thisGUISelectionManager.OpenPanel(thisGUISelectionManager.SelectedUnitPanel.gameObject);
        }
		else if (PCSelector.selectedObjects.Count > 1)
        {
			thisGUISelectionManager.SelectedUnitBtn.SetActive(true);
			thisGUISelectionManager.AllSelectedUnitsBtn.SetActive(true);
			thisGUISelectionManager.OpenPanel(thisGUISelectionManager.AllSelectedUnitsPanel.gameObject);
        }
        else
        {
			thisGUISelectionManager.SelectedUnitBtn.SetActive(false);
            thisGUISelectionManager.AllSelectedUnitsBtn.SetActive(false);
			thisGUISelectionManager.OpenPanel(null);
        }
    }

	public void OpenPanel(GameObject OnPanel)
	{
        TechPanel.gameObject.SetActive(false);
        AllSelectedUnitsPanel.gameObject.SetActive(false);
        SelectedUnitPanel.gameObject.SetActive(false);

		if(PCSelector.PlayerOnScreen.Count != 0 && OnPanel != null)
		{
			OnPanel.SetActive(true);
		}
        else if (OnPanel == TechPanel.gameObject)
		{
            TechPanel.gameObject.SetActive(true);
		}
	}
}