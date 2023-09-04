using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DreamCat
{
	public class SleepHealth : MonoBehaviour
	{
		[SerializeField]
		RectTransform sleepHealthRtr;

		void Update()
		{
			Vector2 maxAnchor = sleepHealthRtr.anchorMax;
			maxAnchor.x = GameManager.Human.SleepStatus;
			sleepHealthRtr.anchorMax = maxAnchor;
		}
	}
}
