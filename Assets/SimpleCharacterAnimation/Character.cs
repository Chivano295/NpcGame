using UnityEngine;

namespace CharacterAnimation
{
	public class Character : MonoBehaviour
	{
		[Header("Variables")]
		[SerializeField] private float m_walkSpeed = 1.0f;
		[SerializeField] private float m_runSpeed  = 3.0f;

		[Header("Output")]
		public Vector3 m_currentLookDirection;
		public bool    m_isRunning;
		public float   m_currentSpeed;
		public float   m_idleTime;

		private void Start()
		{
			m_currentLookDirection = Vector3.forward;
		}

		private void Update()
		{
			Movement();
		}

		private void Movement()
		{
			Vector3 inputDirection = Vector3.zero;
			if (Input.GetKey(KeyCode.A))
			{
				inputDirection.x = -1f;
			}
			if (Input.GetKey(KeyCode.D))
			{
				inputDirection.x = 1f;
			}
			if (Input.GetKey(KeyCode.W))
			{
				inputDirection.y = 1f;
			}
			if (Input.GetKey(KeyCode.S))
			{
				inputDirection.y = -1f;
			}
			if (Input.GetKey(KeyCode.LeftShift))
			{
				m_isRunning = true;
			}
			else
			{
				m_isRunning = false;
			}
			inputDirection.Normalize();
			if (inputDirection == Vector3.zero)
			{
				m_currentSpeed = 0f;
				m_idleTime += Time.deltaTime;
				return;
			}
			m_currentSpeed           = m_isRunning ? m_runSpeed : m_walkSpeed;
			m_idleTime               = 0f;

			m_currentLookDirection   = Camera.main.transform.TransformDirection(inputDirection);
			m_currentLookDirection.y = 0f;
			m_currentLookDirection.Normalize();

			transform.Translate(m_currentLookDirection * m_currentSpeed * Time.deltaTime, Space.World);
			transform.rotation       = Quaternion.LookRotation(m_currentLookDirection, Vector3.up);
		}
	}
}