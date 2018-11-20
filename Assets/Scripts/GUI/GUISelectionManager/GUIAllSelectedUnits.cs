using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUIAllSelectedUnits : MonoBehaviour {

    List<PlayerControlled> selectedObjects;
    public GameObject childItem;

    public void UpdateSelected()
    {
        
    }

	public void UpdatePanel(List<PlayerControlled> SelectedObjects)
    {
        selectedObjects = SelectedObjects;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        RectTransform childTrans = (RectTransform) childItem.transform;

        float xOffset = childTrans.sizeDelta.x / 2;
        float yOffset = childTrans.sizeDelta.y;

		for(int i = 0; i < selectedObjects.Count; i++)
        {
            GameObject newChild = Instantiate(childItem) as GameObject;
            
            RectTransform newTransform = (RectTransform)newChild.transform;

            newTransform.anchorMin = new Vector2(0.5f, 1f);
            newTransform.anchorMax = new Vector2(0.5f, 1f);
            newTransform.pivot = new Vector2(0.5f, 1f);

            newTransform.SetParent(transform);
            newTransform.anchoredPosition = new Vector3(((i % 2 == 0) ? -1 : 1) * xOffset, -1 * yOffset * Mathf.FloorToInt(i / 2));

			PlayerControlled temp = selectedObjects[i];
            Slider healthSlider = newTransform.Find("Health").GetComponent<Slider>();
            healthSlider.maxValue = temp.health;
            healthSlider.value = temp.currentHealth;
            if (temp is Unit)
            {
                Slider suppliesSlider = newTransform.Find("Supplies").GetComponent<Slider>();
                suppliesSlider.gameObject.SetActive(true);
                suppliesSlider.maxValue = ((Unit)temp).supply;
                suppliesSlider.value = ((Unit)temp).currentSupply;
            }
            else
            {
                newTransform.Find("Supplies").gameObject.SetActive(false);
            }
        }

		float expectedHeight = 30 * Mathf.CeilToInt(selectedObjects.Count / 2);
        ((RectTransform)transform).sizeDelta = new Vector2(((RectTransform)transform).sizeDelta.x, (expectedHeight <= 120) ? 120 : expectedHeight);
    }
}
