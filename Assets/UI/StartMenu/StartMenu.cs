using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace DreamCat
{
	public class StartMenu : MonoBehaviour
	{
		public void Btn_StartGame()
		{
			SceneManager.LoadScene(1);
		}

		public void Btn_QuitGame()
		{
			Application.Quit();
		}
	}
}

