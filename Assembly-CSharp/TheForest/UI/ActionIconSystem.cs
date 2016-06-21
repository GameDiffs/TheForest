using System;
using System.Collections.Generic;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.UI
{
	public class ActionIconSystem : MonoBehaviour
	{
		public enum CurrentViewOptions
		{
			AllowInWorld,
			AllowInBook,
			AllowInPlane
		}

		public ActionIcon _textIconPrefab;

		public ActionIcon _spriteIconPrefab;

		public Transform _iconHolderTr;

		public Transform _iconHolderBookTr;

		public Transform _iconHolderPlaneTr;

		private static ActionIconSystem Instance;

		private Queue<ActionIcon> _textIconPool = new Queue<ActionIcon>();

		private Queue<ActionIcon> _spriteIconPool = new Queue<ActionIcon>();

		private Dictionary<Transform, ActionIcon> _activeIcons = new Dictionary<Transform, ActionIcon>();

		private void Awake()
		{
			ActionIconSystem.Instance = this;
		}

		private void Update()
		{
			if (LocalPlayer.Inventory)
			{
				bool flag = LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.World || LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Sleep;
				if (flag != this._iconHolderTr.gameObject.activeSelf)
				{
					this._iconHolderTr.gameObject.SetActive(flag);
				}
				if (!flag)
				{
					bool flag2 = LocalPlayer.Inventory.CurrentView == PlayerInventory.PlayerViews.Book;
					if (flag2 != this._iconHolderBookTr.gameObject.activeSelf)
					{
						this._iconHolderBookTr.gameObject.SetActive(flag2);
					}
				}
			}
		}

		private void OnDestroy()
		{
			if (ActionIconSystem.Instance == this)
			{
				ActionIconSystem.Instance = null;
			}
		}

		public static UISprite RegisterIcon(Transform target, InputMappingIcons.Actions action, ActionIconSystem.CurrentViewOptions currentViewOption = ActionIconSystem.CurrentViewOptions.AllowInWorld)
		{
			if (ActionIconSystem.Instance && !ActionIconSystem.Instance._activeIcons.ContainsKey(target))
			{
				ActionIcon actionIcon;
				if (!InputMappingIcons.UsesText(action))
				{
					if (ActionIconSystem.Instance._spriteIconPool.Count > 0)
					{
						actionIcon = ActionIconSystem.Instance._spriteIconPool.Dequeue();
						actionIcon.gameObject.SetActive(true);
						ActionIconSystem.Instance.SetIconHolderTr(actionIcon.transform, currentViewOption);
					}
					else
					{
						actionIcon = UnityEngine.Object.Instantiate<ActionIcon>(ActionIconSystem.Instance._spriteIconPrefab);
						ActionIconSystem.Instance.SetIconHolderTr(actionIcon.transform, currentViewOption);
						actionIcon.transform.localScale = ActionIconSystem.Instance._spriteIconPrefab.transform.localScale;
					}
					actionIcon._sprite.spriteName = InputMappingIcons.GetMappingFor(action);
					UISpriteData atlasSprite = actionIcon._sprite.GetAtlasSprite();
					if (atlasSprite == null)
					{
						ActionIconSystem.Instance.DisableActionIcon(actionIcon);
						return null;
					}
					actionIcon._sprite.width = Mathf.RoundToInt((float)atlasSprite.width / (float)atlasSprite.height * (float)actionIcon._sprite.height);
				}
				else
				{
					if (ActionIconSystem.Instance._textIconPool.Count > 0)
					{
						actionIcon = ActionIconSystem.Instance._textIconPool.Dequeue();
						actionIcon.gameObject.SetActive(true);
						ActionIconSystem.Instance.SetIconHolderTr(actionIcon.transform, currentViewOption);
					}
					else
					{
						actionIcon = UnityEngine.Object.Instantiate<ActionIcon>(ActionIconSystem.Instance._textIconPrefab);
						ActionIconSystem.Instance.SetIconHolderTr(actionIcon.transform, currentViewOption);
						actionIcon.transform.localScale = ActionIconSystem.Instance._textIconPrefab.transform.localScale;
					}
					actionIcon._label.text = InputMappingIcons.GetMappingFor(action);
				}
				actionIcon._follow._target = target;
				actionIcon._fillSprite.gameObject.SetActive(false);
				actionIcon._follow._inBook = (currentViewOption == ActionIconSystem.CurrentViewOptions.AllowInBook);
				ActionIconSystem.Instance._activeIcons.Add(target, actionIcon);
				return actionIcon._fillSprite;
			}
			return null;
		}

		public static ActionIcon UnregisterIcon(Transform target)
		{
			ActionIcon actionIcon;
			if (ActionIconSystem.Instance && ActionIconSystem.Instance._activeIcons.TryGetValue(target, out actionIcon))
			{
				actionIcon._follow._target2 = null;
				ActionIconSystem.Instance._activeIcons.Remove(target);
				ActionIconSystem.Instance.DisableActionIcon(actionIcon);
				return actionIcon;
			}
			return null;
		}

		private void DisableActionIcon(ActionIcon ai)
		{
			if (ai._label != null)
			{
				ActionIconSystem.Instance._textIconPool.Enqueue(ai);
			}
			else
			{
				ActionIconSystem.Instance._spriteIconPool.Enqueue(ai);
			}
			ai.gameObject.SetActive(false);
		}

		public static ActionIcon GetActionIcon(Transform target)
		{
			ActionIcon result;
			if (ActionIconSystem.Instance && ActionIconSystem.Instance._activeIcons.TryGetValue(target, out result))
			{
				return result;
			}
			return null;
		}

		private void SetIconHolderTr(Transform t, ActionIconSystem.CurrentViewOptions currentViewOption)
		{
			switch (currentViewOption)
			{
			case ActionIconSystem.CurrentViewOptions.AllowInWorld:
				t.parent = this._iconHolderTr;
				break;
			case ActionIconSystem.CurrentViewOptions.AllowInBook:
				t.parent = this._iconHolderBookTr;
				break;
			case ActionIconSystem.CurrentViewOptions.AllowInPlane:
				t.parent = this._iconHolderPlaneTr;
				break;
			}
		}
	}
}
