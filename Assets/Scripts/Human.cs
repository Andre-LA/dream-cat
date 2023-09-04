using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamCat 
{
	public class Human : MonoBehaviour
	{
		[Range(0f, 1f)]
		[SerializeField]
		float _sleepStatus = 1f;

		[Range(0f, 1f)]
		[SerializeField]
		float _recoveryPerSecond = 0f;

		Animator _animator;

		public float SleepStatus => _sleepStatus;

		Coroutine _recoveringCo;

		public void Disturb(float disturbance)
		{
			_sleepStatus -= disturbance;
			if (_sleepStatus <= 0f)
			{
				AwakeHuman();
			}
		}

		void AwakeHuman()
		{
			StopCoroutine(_recoveringCo);
		}

		void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		void Start()
		{
			_recoveringCo = StartCoroutine(Recover());
		}

		void Update()
		{
			_animator.SetFloat("Sleep Health", _sleepStatus);
		}

		IEnumerator Recover()
		{
			while (true)
			{
				float recoveryDelta = _recoveryPerSecond * Time.deltaTime;
				_sleepStatus = Mathf.Clamp01(_sleepStatus + recoveryDelta);
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
