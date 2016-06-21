using System;
using TheForest.UI;
using UnityEngine;

namespace TheForest.Utils
{
	public class ActionTriggerEvent : MonoBehaviour
	{
		public GameObject _targetOverride;

		public InputMappingIcons.Actions _action;

		public string _event = "OnClick";

		public bool _gamepadOnlyIcon = true;

		public bool _isAxis;

		public float _axisSpeed = 0.1f;

		private string _actionName;

		private void Awake()
		{
			this._actionName = this._action.ToString();
		}

		public void OnEnable()
		{
			if (this._action != InputMappingIcons.Actions.None && (!this._gamepadOnlyIcon || Input.IsGamePad))
			{
				ActionIconSystem.RegisterIcon(base.transform, this._action, ActionIconSystem.CurrentViewOptions.AllowInBook);
			}
		}

		public void OnDisable()
		{
			if (this._action != InputMappingIcons.Actions.None)
			{
				ActionIconSystem.UnregisterIcon(base.transform);
			}
		}

		private void Update()
		{
			if (this._isAxis)
			{
				float num = Input.GetAxis(this._actionName);
				if (Mathf.Abs(num) > 0.0001f)
				{
					num *= this._axisSpeed;
					if (this._targetOverride)
					{
						this._targetOverride.SendMessage(this._event, num, SendMessageOptions.DontRequireReceiver);
					}
					else
					{
						base.SendMessage(this._event, num, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			else if (Input.GetButtonDown(this._actionName))
			{
				if (this._targetOverride)
				{
					this._targetOverride.SendMessage(this._event, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					base.SendMessage(this._event, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
}
