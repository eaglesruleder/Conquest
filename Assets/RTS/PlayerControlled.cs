using UnityEngine;

namespace OdWyer.RTS
{
	public class PlayerControlled : MonoBehaviour
		,IUnitLifecycle
	{
		public GameObject SelectionObj;

		public void Selected(bool Select)
		{
			if (SelectionObj)
				SelectionObj.SetActive(Select);
		}

		public virtual void BeforeDestroy()
		{
			Selected(false);
			if (!IsInvoking("EndNow"))
				Invoke("EndNow", 1f);
		}
		private void EndNow() => Destroy(gameObject);
	}
}