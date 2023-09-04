using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamCat
{
	public class GameOver : MonoBehaviour
	{


		public void Btn_Restart()
		{
			GameManager.RestartGame();
		}
	}
}
