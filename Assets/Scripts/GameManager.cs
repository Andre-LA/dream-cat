using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DreamCat
{
	public class GameManager : MonoBehaviour
	{
		class Singleton<T> where T : MonoBehaviour
		{
			static T _instance = null;

			public T Instance 
			{
				get
				{
					if (!_instance)
					{
						_instance = FindObjectOfType<T>();
					}
					return _instance;
				}
			}
		}

		static Singleton<Human> _human = new Singleton<Human>();
		public static Human Human => _human.Instance;

		static Singleton<GameManager> _instance = new Singleton<GameManager>();
		public static GameManager Instance => _instance.Instance;

		static float gameStartTime = 0f;
		static float GameTime => Time.time - gameStartTime;

		[SerializeField]
		float _gameDuration;
		
		public float TimeRemaining => Mathf.Clamp(_gameDuration - GameTime, 0f, _gameDuration);
		public float TimeRemainingScalar => TimeRemaining / _gameDuration;

		static bool _isGameOver = false;
		public static bool IsGameOver => _isGameOver;

		[SerializeField]
		GameObject _gameOverCanvas;

		void Awake()
		{
			gameStartTime = Time.time;
			_isGameOver = false;
		}

		public static void SetAsGameOver()
		{
			_isGameOver = true;
			Instance._gameOverCanvas.SetActive(true);
		}

		public static void RestartGame()
		{
			Scene curScene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(curScene.buildIndex);
		}
	}
}