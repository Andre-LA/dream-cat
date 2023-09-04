using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DreamCat
{
	public class Tutu : MonoBehaviour
	{
		Transform _tr;
		DreamCatActions _actions;

		[Tooltip("Movement speed of Tutu")]
		[SerializeField] 
		float _moveSpeed;

		[Tooltip("Jump force of Tutu")]
		[SerializeField] 
		float _jumpForce;

		State _state = State.Idle;
		uint stateTicks = 0;

		[Header("Components")]
		[SerializeField]
		Animator _animator;
		Rigidbody _rb;
		BoxCollider _collider;
		SpriteRenderer _spriteRenderer;

		// input
		Vector2 MoveInput => _actions.Gameplay.Move.ReadValue<Vector2>();
		bool JumpInput => _actions.Gameplay.Jump.triggered;
		bool AttackInput => _actions.Gameplay.Attack.triggered;

		// properties
		bool IsFirstTick => stateTicks == 0;
		bool IsOnGround => _rb.velocity.y <= 0f && RaycastToGround(out RaycastHit hit);

		// etc
		Vector3 _offsetToCenter = Vector3.up;
		Vector3 _attackingDirection = Vector3.zero;

		enum State
		{
			Idle = 0,
			Walking,
			Attacking,
			Jump,
			Falling,
		}

		void EnableCollision()
		{
			_collider.enabled = true;
		}

		void DisableCollision()
		{
			_collider.enabled = false;
		}

		bool RaycastFromCenter(out RaycastHit hit, Vector3 direction, float distance, LayerMask layerMask, QueryTriggerInteraction queryTriggerInteraction)
		{
			return Physics.Raycast(
				_tr.position + _offsetToCenter,
				direction,
				out hit,
				distance,
				layerMask,
				queryTriggerInteraction
			);
		}

		bool RaycastToGround(out RaycastHit hit)
		{
			return RaycastFromCenter(
				out hit,
				Vector3.down,
				1f,
				LayerMask.GetMask("Ground"),
				QueryTriggerInteraction.Ignore
			);
		}

		bool RaycastAttack(out RaycastHit hit)
		{
			return RaycastFromCenter(
				out hit,
				_attackingDirection,
				1f,
				LayerMask.GetMask("SleepDisturber"),
				QueryTriggerInteraction.Collide
			);
		}

		void TryAlignWithGround()
		{
			if (RaycastToGround(out RaycastHit hit)) {
				_rb.position = new Vector3(_rb.position.x, hit.point.y, _rb.position.z);
				_rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
			}
		}

		void ApplyMovementVelocity(Vector2 moveAction)
		{
			Vector2 moveVelocity = _moveSpeed * moveAction;
			_rb.velocity = new Vector3(moveVelocity.x, _rb.velocity.y, moveVelocity.y);
			_animator.SetFloat("MoveSpeed", moveVelocity.sqrMagnitude);
			if (moveAction.x != 0f)
			{
				_spriteRenderer.flipX = moveAction.x < 0f;
			}
		}

		bool IsMoving(Vector2 movingInput)
		{
			return movingInput.sqrMagnitude > 0f;
		}

		State OnIdle()
		{
			if (IsMoving(MoveInput))
			{
				return State.Walking;
			}

			if (!IsFirstTick && JumpInput)
			{
				return State.Jump;
			}

			if (!IsFirstTick && AttackInput)
			{
				return State.Attacking;
			}

			if (IsFirstTick)
			{
				_animator.SetFloat("MoveSpeed", 0f);
				_rb.useGravity = false;
			}

			_rb.velocity = Vector3.zero;
			TryAlignWithGround();

			return State.Idle;
		}

		State OnWalking()
		{
			Vector2 moveAction = MoveInput;

			if (!IsMoving(moveAction))
			{
				return State.Idle;
			}

			if (!IsFirstTick && JumpInput)
			{
				return State.Jump;
			}

			if (!IsFirstTick && AttackInput)
			{
				return State.Attacking;
			}

			if (!IsOnGround)
			{
				return State.Falling;
			}

			if (IsFirstTick)
			{
				_rb.useGravity = false;
			}

			ApplyMovementVelocity(moveAction);
			TryAlignWithGround();

			if (IsMoving(moveAction))
			{
				_attackingDirection = new Vector3(
					moveAction.x,
					0f,
					moveAction.y
				);
			}

			return State.Walking;
		}

		State OnJump()
		{
			_rb.useGravity = true;
			_rb.velocity = new Vector3(_rb.velocity.x, _jumpForce, _rb.velocity.z);
			return State.Falling;
		}

		State OnFalling()
		{
			if (IsOnGround)
			{
				EnableCollision();
				return State.Idle;
			}

			if (IsFirstTick)
			{
				_rb.useGravity = true;
				DisableCollision();
			}

			ApplyMovementVelocity(MoveInput);
			return State.Falling;
		}

		State OnAttacking()
		{
			if (IsFirstTick)
			{
				_rb.velocity = Vector3.zero;
				_rb.useGravity = false;
				bool isAttackingSomething = RaycastAttack(out RaycastHit hit);

				if (isAttackingSomething)
				{
					var disturber = hit.transform.GetComponent<SleepDisturber>();
					disturber?.StopDisturbance();
				}

				_animator.Play("Attack");
			}
			else if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
			{
				return State.Idle;
			}

			Debug.Log($"Preso?? {IsFirstTick}, {_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")}");

			TryAlignWithGround();

			return State.Attacking;
		}

		void OnTick()
		{
			int loops = 0;
			State startingState = _state;
			do
			{
				startingState = _state;
				switch (_state)
				{
					case State.Idle: _state = OnIdle(); break;
					case State.Walking: _state = OnWalking(); break;
					case State.Attacking: _state = OnAttacking(); break;
					case State.Jump: _state = OnJump(); break;
					case State.Falling: _state = OnFalling(); break;
				}
				if (startingState != _state)
				{
					stateTicks = 0;
				}
				loops++;
				if (loops > 100)
				{
					Debug.LogWarning("Infinite loop got");
					break;
				}
			} while (startingState != _state);
			++stateTicks;
		}

		void Awake()
		{
			_tr = GetComponent<Transform>();
			_rb = GetComponent<Rigidbody>();
			_collider = GetComponent<BoxCollider>();
			_actions = new DreamCatActions();
		}

		void Start()
		{
			_spriteRenderer = _animator.GetComponent<SpriteRenderer>();
		}
		void Update()
		{
			OnTick();
		}

		void OnEnable()
		{
			_actions.Gameplay.Enable();
		}

		void OnDisable()
		{
			_actions.Gameplay.Disable();
		}

		void OnDrawGizmos()
		{
			var origin = transform.position + _offsetToCenter;
			Gizmos.DrawLine(origin, origin + Vector3.down);

			Gizmos.DrawLine(origin, origin + _attackingDirection);
		}
	}
}
