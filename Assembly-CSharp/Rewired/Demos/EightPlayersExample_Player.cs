using System;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu(""), RequireComponent(typeof(CharacterController))]
	public class EightPlayersExample_Player : MonoBehaviour
	{
		public int playerId;

		public float moveSpeed = 3f;

		public float bulletSpeed = 15f;

		public GameObject bulletPrefab;

		private Player player;

		private CharacterController cc;

		private Vector3 moveVector;

		private bool fire;

		[NonSerialized]
		private bool initialized;

		private void Awake()
		{
			this.cc = base.GetComponent<CharacterController>();
		}

		private void Initialize()
		{
			this.player = ReInput.players.GetPlayer(this.playerId);
			this.initialized = true;
		}

		private void Update()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			if (!this.initialized)
			{
				this.Initialize();
			}
			this.GetInput();
			this.ProcessInput();
		}

		private void GetInput()
		{
			this.moveVector.x = this.player.GetAxis("Move Horizontal");
			this.moveVector.y = this.player.GetAxis("Move Vertical");
			this.fire = this.player.GetButtonDown("Fire");
		}

		private void ProcessInput()
		{
			if (this.moveVector.x != 0f || this.moveVector.y != 0f)
			{
				this.cc.Move(this.moveVector * this.moveSpeed * Time.deltaTime);
			}
			if (this.fire)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.bulletPrefab, base.transform.position + base.transform.right, base.transform.rotation);
				gameObject.GetComponent<Rigidbody>().AddForce(base.transform.right * this.bulletSpeed, ForceMode.VelocityChange);
			}
		}
	}
}
