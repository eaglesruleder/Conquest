using System.Collections.Generic;

using UnityEngine;

namespace OdWyer.RTS
{
	public class IsVisibleTracker : MonoBehaviour
	{
		public static List<IsVisibleTracker> OnScreen = new List<IsVisibleTracker>();

		public void OnBecameVisible()
		{
			if (!OnScreen.Contains(this))
				OnScreen.Add(this);
		}

		public void OnBecameInvisible() => OnScreen.Remove(this);
	}
}