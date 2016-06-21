using System;
using UnityEngine;

namespace TheForest.UI
{
	public class OptionMenuLoader : MonoBehaviour
	{
		public Transform _optionMenuPrefab;

		public GameObject _mainMenuGO;

		public GameObject _controlSettingsGO;

		private void Awake()
		{
			Transform transform = UnityEngine.Object.Instantiate<Transform>(this._optionMenuPrefab);
			transform.parent = base.transform;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			OptionMenuTweens component = transform.GetComponent<OptionMenuTweens>();
			component._backwardTweener.tweenTarget = base.gameObject;
			component._forwardTweener.tweenTarget = this._mainMenuGO;
			component._controlSettingsBackwardTweener.tweenTarget = base.gameObject;
			component._controlSettingsForwardTweener.tweenTarget = this._controlSettingsGO;
		}
	}
}
