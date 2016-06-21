using System;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu(""), RequireComponent(typeof(CharacterController))]
	public class CustomControllerDemo_Player : MonoBehaviour
	{
		public int playerId;

		public float speed = 1f;

		public float bulletSpeed = 20f;

		public GameObject bulletPrefab;

		private Player _player;

		private CharacterController cc;

		private Player player
		{
			get
			{
				if (this._player == null)
				{
					this._player = ReInput.players.GetPlayer(this.playerId);
				}
				return this._player;
			}
		}

		private void Awake()
		{
			this.cc = base.GetComponent<CharacterController>();
		}

		private void Update()
		{
			if (!ReInput.isReady)
			{
				return;
			}
			Vector2 a = new Vector2(this.player.GetAxis("Move Horizontal"), this.player.GetAxis("Move Vertical"));
			this.cc.Move(a * this.speed * Time.deltaTime);
			if (this.player.GetButtonDown("Fire"))
			{
				Vector3 b = Vector3.Scale(new Vector3(1f, 0f, 0f), base.transform.right);
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.bulletPrefab, base.transform.position + b, Quaternion.identity);
				gameObject.GetComponent<Rigidbody>().velocity = new Vector3(this.bulletSpeed * base.transform.right.x, 0f, 0f);
			}
			if (this.player.GetButtonDown("Change Color"))
			{
				Renderer component = base.GetComponent<Renderer>();
				Material material = component.material;
				material.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
				component.material = material;
			}
		}
	}
}
