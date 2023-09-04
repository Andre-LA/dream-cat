using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DreamCat
{
	public class SleepDisturber : MonoBehaviour
	{
		[Tooltip("How much disturbance will be applied per second to the human")]
		[Range(0f, 1f)]
		[SerializeField]
		float _disturbancePerSecond = 1f;

		[Tooltip("How much seconds to initially wait to start disturbing")]
		[SerializeField]
		float _initialSilenceTime = 1f;

		[Tooltip("How much seconds should the disturber wait to start disturbing again, one of the elements are choosen randomly")]
		[SerializeField]
		float[] _silenceDurations = new float[1];

		Coroutine _disturbanceCoroutine;

		[Header("Components")]
		[SerializeField]
		Animator _animator;

		[ContextMenu("Start Disturbance")]
		void StartDisturbance()
		{
			if (_disturbanceCoroutine == null)
			{
				_animator.SetBool("Is Disturbing", true);
				_disturbanceCoroutine = StartCoroutine(Disturb());
			}
		}

		[ContextMenu("Stop Disturbance")]
		public void StopDisturbance()
		{
			if (_disturbanceCoroutine != null)
			{
				_animator.SetBool("Is Disturbing", false);
				StopCoroutine(_disturbanceCoroutine);
				_disturbanceCoroutine = null;
				StartCoroutine(SilenceThenDisturb());
			}
		}

		IEnumerator SilenceThenDisturb()
		{
			float silenceDuration = _silenceDurations[Random.Range(0, _silenceDurations.Length)];
			Debug.Log($"silence duration: {silenceDuration}");
			yield return new WaitForSeconds(silenceDuration);
			StartDisturbance();
		}

		IEnumerator Disturb()
		{
			while (true)
			{
				float disturbanceDelta = _disturbancePerSecond * Time.deltaTime;
				GameManager.Human?.Disturb(disturbanceDelta);
				yield return new WaitForEndOfFrame();
			}
		}

		IEnumerator Start()
		{
			yield return new WaitForSeconds(_initialSilenceTime);
			StartDisturbance();
		}
	}
}
