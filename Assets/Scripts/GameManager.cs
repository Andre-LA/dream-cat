using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		public static Human Human { get { return _human.Instance; } }
	}
}