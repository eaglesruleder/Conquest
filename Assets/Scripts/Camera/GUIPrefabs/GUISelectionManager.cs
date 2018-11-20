using UnityEngine;

public class GUISelectionManager : MonoBehaviour {

	public GameObject StatBtn;
	public GUISelectionStat StatPanel;

	public GameObject AbilityBtn;
	public GUISelectionAbility AbilityPanel;

	public GameObject LevelBtn;
	public GUISelectionLevel LevelPanel;

	public GameObject SelectedBtn;
	public GUISelectionSelect SelectedPanel;

	public GUIPlayer playerData;

    public void UpdatePanels()
    {
        StatPanel.UpdatePanel(playerData.selectedObject);
        LevelPanel.UpdatePanel(playerData);

        // Update is applies when selection changes, so on new items
        OpenPanel(StatPanel.gameObject);
    }

	public void OpenPanel(GameObject OnPanel)
	{
		StatPanel.gameObject.SetActive(false);
		AbilityPanel.gameObject.SetActive(false);
		LevelPanel.gameObject.SetActive(false);
		SelectedPanel.gameObject.SetActive(false);

		if(playerData.selectedObjects.Count != 0 && OnPanel != null)
		{
			OnPanel.SetActive(true);
		}
		else if (OnPanel == LevelPanel.gameObject)
		{
			LevelPanel.gameObject.SetActive(true);
		}
	}
}
