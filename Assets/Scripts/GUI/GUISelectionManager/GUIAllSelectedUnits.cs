using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUIAllSelectedUnits : MonoBehaviour {

	List<ListSlot> displayed;
    public GameObject childItem;

	public void UpdatePanel(List<PlayerControlled> SelectedObjects)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
		
        RectTransform childTrans = (RectTransform) childItem.transform;
        float xOffset = childTrans.sizeDelta.x / 2;
        float yOffset = childTrans.sizeDelta.y;

		displayed = new List<ListSlot>();

		for(int i = 0; i < SelectedObjects.Count; i++)
        {
			ListSlot cur = new ListSlot(transform, childItem);
			cur.SetPos(i, xOffset, yOffset);
			cur.SetPC(SelectedObjects[i]);
			displayed.Add (cur);
        }

		float expectedHeight = 30 * Mathf.CeilToInt(SelectedObjects.Count / 2);
        ((RectTransform)transform).sizeDelta = new Vector2(((RectTransform)transform).sizeDelta.x, (expectedHeight <= 120) ? 120 : expectedHeight);
    }

	void Update()
	{
		List<ListSlot> toRemove = new List<ListSlot> ();

		foreach(ListSlot slot in displayed)
		{
			if(slot.reference)
			{
				slot.UpdatePC();
			}
			else
			{
				toRemove.Add(slot);
			}
		}

		if(toRemove.Count != 0)
		{
			foreach(ListSlot slot in toRemove)
			{
				displayed.Remove(slot);
			}

			RectTransform childTrans = (RectTransform) childItem.transform;
			float xOffset = childTrans.sizeDelta.x / 2;
			float yOffset = childTrans.sizeDelta.y;

			for(int i = 0; i < displayed.Count; i++)
			{
				displayed[i].SetPos(i, xOffset, yOffset);
			}
		}
	}

	internal class ListSlot
	{
		internal PlayerControlled reference;

		GameObject display;
		RectTransform trans;
		Slider health;
		Slider supplies;

		internal ListSlot(Transform parentTrans, GameObject clone)
		{
			display = Instantiate(clone) as GameObject;
			trans = (RectTransform)display.transform;
			
			trans.anchorMin = new Vector2(0.5f, 1f);
			trans.anchorMax = new Vector2(0.5f, 1f);
			trans.pivot = new Vector2(0.5f, 1f);
			
			trans.SetParent(parentTrans);
			
			health = trans.Find("Health").GetComponent<Slider>();
			supplies = trans.Find("Supplies").GetComponent<Slider>();
		}

		internal void SetPos(int pos, float xOffset, float yOffset)
		{
			trans.anchoredPosition = new Vector3(((pos % 2 == 0) ? -1 : 1) * xOffset, -1 * yOffset * Mathf.FloorToInt(pos / 2));
		}
		
		internal void SetPC (PlayerControlled pc)
		{
			reference = pc;

			health.maxValue = reference.Health;
			health.value = reference.CurrentHealth;
			
			if (reference is Unit)
			{
				supplies.gameObject.SetActive(true);
				supplies.maxValue = ((Unit)reference).Supply;
				supplies.value = ((Unit)reference).CurrentSupply;
			}
			else
			{
				supplies.gameObject.SetActive(false);
			}
		}

		internal void UpdatePC()
		{
			if(reference)
			{
				health.value = reference.CurrentHealth;
				if(reference is Unit)
				{
					supplies.value = ((Unit)reference).CurrentSupply;
				}
			}
		}
	}

}
