using System;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu(""), RequireComponent(typeof(TouchControllerExample))]
	public class CustomControllersTiltDemo : MonoBehaviour
	{
		public Transform target;

		public float speed = 10f;

		private CustomController controller;

		private Player player;

		private void Awake()
		{
			Screen.orientation = ScreenOrientation.LandscapeLeft;
			this.player = ReInput.players.GetPlayer(0);
			ReInput.InputSourceUpdateEvent += new Action(this.OnInputUpdate);
			this.controller = (CustomController)this.player.controllers.GetControllerWithTag(ControllerType.Custom, "TiltController");
		}

		private void Update()
		{
			if (this.target == null)
			{
				return;
			}
			Vector3 a = Vector3.zero;
			a.y = this.player.GetAxis("Tilt Vertical");
			a.x = this.player.GetAxis("Tilt Horizontal");
			if (a.sqrMagnitude > 1f)
			{
				a.Normalize();
			}
			a *= Time.deltaTime;
			this.target.Translate(a * this.speed);
		}

		private void OnInputUpdate()
		{
			Vector3 acceleration = Input.acceleration;
			this.controller.SetAxisValue(0, acceleration.x);
			this.controller.SetAxisValue(1, acceleration.y);
			this.controller.SetAxisValue(2, acceleration.z);
		}
	}
}
