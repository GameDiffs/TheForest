using System;
using UnityEngine;

namespace TheForest.UI
{
	public class ActionIconWorld : MonoBehaviour
	{
		public InputMappingIcons.Actions _action;

		public ActionIconSystem.CurrentViewOptions _currentViewOption;

		public bool _overrideDepth;

		public float _depth;

		public bool _overrideHeight;

		public float _height;

		public float _oldDepth;

		public float _oldHeight;

		public UISprite FillSprite
		{
			get;
			set;
		}

		public void OnEnable()
		{
			if (this._action != InputMappingIcons.Actions.None)
			{
				this.FillSprite = ActionIconSystem.RegisterIcon(base.transform, this._action, this._currentViewOption);
				if (this._overrideDepth || this._overrideHeight)
				{
					ActionIcon actionIcon = ActionIconSystem.GetActionIcon(base.transform);
					if (actionIcon)
					{
						if (this._overrideDepth)
						{
							if (this._currentViewOption == ActionIconSystem.CurrentViewOptions.AllowInBook)
							{
								this._oldDepth = actionIcon._follow._depthRatioBook;
								actionIcon._follow._depthRatioBook = this._depth;
							}
							else
							{
								this._oldDepth = actionIcon._follow._minDepth;
								actionIcon._follow._minDepth = this._depth;
							}
						}
						if (this._overrideHeight)
						{
							if (this._currentViewOption == ActionIconSystem.CurrentViewOptions.AllowInBook)
							{
								this._oldHeight = actionIcon._follow._worldOffsetBook.y;
								actionIcon._follow._worldOffsetBook.y = this._height;
							}
							else
							{
								this._oldHeight = actionIcon._follow._worldOffset.y;
								actionIcon._follow._worldOffset.y = this._height;
							}
						}
					}
				}
			}
		}

		public void OnDisable()
		{
			if (this._action != InputMappingIcons.Actions.None)
			{
				ActionIcon actionIcon = ActionIconSystem.UnregisterIcon(base.transform);
				if (actionIcon)
				{
					if (this._overrideDepth)
					{
						if (this._currentViewOption == ActionIconSystem.CurrentViewOptions.AllowInBook)
						{
							actionIcon._follow._depthRatioBook = this._oldDepth;
						}
						else
						{
							actionIcon._follow._minDepth = this._oldDepth;
						}
					}
					if (this._overrideHeight)
					{
						if (this._currentViewOption == ActionIconSystem.CurrentViewOptions.AllowInBook)
						{
							actionIcon._follow._worldOffsetBook.y = this._oldHeight;
						}
						else
						{
							actionIcon._follow._worldOffset.y = this._oldHeight;
						}
					}
				}
			}
		}
	}
}
