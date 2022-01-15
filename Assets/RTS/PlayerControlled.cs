using UnityEngine;

namespace OdWyer.RTS
{
	public abstract class PlayerControlled : MonoBehaviour
		,IUnitLifecycle
	{
		public string playerID;

		public MeshHandler SelectionObj;

		public virtual void SetPlayer(string PlayerID) => playerID = PlayerID;

		public void Selected(bool Select)
		{
			if (SelectionObj)
				SelectionObj.gameObject.SetActive(Select);
		}

		public virtual void BeforeDestroy()
		{
			Selected(false);
			if (!IsInvoking("EndNow"))
				Invoke("EndNow", 1f);
		}
		public virtual void EndNow() => Destroy(gameObject);
	}
}