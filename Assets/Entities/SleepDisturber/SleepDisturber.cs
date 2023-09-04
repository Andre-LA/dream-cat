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

		[Tooltip("Minimum interval to disturber start disturbing again")]
		[SerializeField]
		float minWait = 4f;

		[Tooltip("Maximum interval to disturber start disturbing again")]
		[SerializeField]
		float maxWait = 6f;

		[Tooltip("If the sprite should be invisible when Off, set to true if this sleep disturber it's alongside with an 3D object")]
		[SerializeField]
		bool _shouldBeInvisibleWhenOff = false;

		Coroutine _disturbanceCoroutine;

		[Header("Components")]
		[SerializeField]
		Animator _animator;
		SpriteRenderer _spriteRenderer;

		[ContextMenu("Start Disturbance")]
		void StartDisturbance()
		{
			if (_disturbanceCoroutine == null)
			{
				_animator.SetBool("Is Disturbing", true);
				if (_shouldBeInvisibleWhenOff)
				{
					_spriteRenderer.enabled = true;
				}
				_disturbanceCoroutine = StartCoroutine(Disturb());
			}
		}

		[ContextMenu("Stop Disturbance")]
		public void StopDisturbance()
		{
			if (_disturbanceCoroutine != null)
			{
				_animator.SetBool("Is Disturbing", false);
				if (_shouldBeInvisibleWhenOff)
				{
					_spriteRenderer.enabled = false;
				}
				StopCoroutine(_disturbanceCoroutine);
				_disturbanceCoroutine = null;
				StartCoroutine(SilenceThenDisturb());
			}
		}

		IEnumerator SilenceThenDisturb()
		{
			float silenceDuration = Random.Range(minWait, maxWait);
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
			_spriteRenderer = _animator.GetComponent<SpriteRenderer>();
			if (_shouldBeInvisibleWhenOff)
			{
				_spriteRenderer.enabled = false;
			}
			yield return new WaitForSeconds(_initialSilenceTime);
			StartDisturbance();
		}
	}
}
