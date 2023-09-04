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

		[SerializeField]
		Image sleepHealthImg;

		[SerializeField]
		Image imgTimerIcon;

		[SerializeField]
		Color[] sleepColors = new Color[4];

		[SerializeField]
		Sprite[] timeIcons = new Sprite[4];

		void Update()
		{
			float sleepScalar = GameManager.Human.SleepStatus;

			Vector2 maxAnchor = sleepHealthRtr.anchorMax;
			maxAnchor.x = sleepScalar;
			sleepHealthRtr.anchorMax = maxAnchor;

			int timeIdx = Mathf.RoundToInt(GameManager.Instance.TimeRemainingScalar * (timeIcons.Length - 1));
			imgTimerIcon.sprite = timeIcons[timeIdx];

			int colorIdx = Mathf.RoundToInt(sleepScalar * (sleepColors.Length - 1));
			sleepHealthImg.color = sleepColors[colorIdx];
		}
	}
}
