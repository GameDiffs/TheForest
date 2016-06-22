using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rewired.UI.ControlMapper
{
	[AddComponentMenu(""), RequireComponent(typeof(CanvasGroup))]
	public class Window : MonoBehaviour
	{
		public class Timer
		{
			private bool _started;

			private float end;

			public bool started
			{
				get
				{
					return this._started;
				}
			}

			public bool finished
			{
				get
				{
					if (!this.started)
					{
						return false;
					}
					if (Time.realtimeSinceStartup < this.end)
					{
						return false;
					}
					this._started = false;
					return true;
				}
			}

			public float remaining
			{
				get
				{
					if (!this._started)
					{
						return 0f;
					}
					return this.end - Time.realtimeSinceStartup;
				}
			}

			public void Start(float length)
			{
				this.end = Time.realtimeSinceStartup + length;
				this._started = true;
			}

			public void Stop()
			{
				this._started = false;
			}
		}

		public Image backgroundImage;

		public GameObject content;

		private bool _initialized;

		private int _id = -1;

		private RectTransform _rectTransform;

		private Text _titleText;

		private List<Text> _contentText;

		private GameObject _defaultUIElement;

		private Action<int> _updateCallback;

		private Func<int, bool> _isFocusedCallback;

		private Window.Timer _timer;

		private CanvasGroup _canvasGroup;

		public UnityAction cancelCallback;

		public bool hasFocus
		{
			get
			{
				return this._isFocusedCallback != null && this._isFocusedCallback(this._id);
			}
		}

		public int id
		{
			get
			{
				return this._id;
			}
		}

		public RectTransform rectTransform
		{
			get
			{
				if (this._rectTransform == null)
				{
					this._rectTransform = base.gameObject.GetComponent<RectTransform>();
				}
				return this._rectTransform;
			}
		}

		public Text titleText
		{
			get
			{
				return this._titleText;
			}
		}

		public List<Text> contentText
		{
			get
			{
				return this._contentText;
			}
		}

		public GameObject defaultUIElement
		{
			get
			{
				return this._defaultUIElement;
			}
			set
			{
				this._defaultUIElement = value;
			}
		}

		public Action<int> updateCallback
		{
			get
			{
				return this._updateCallback;
			}
			set
			{
				this._updateCallback = value;
			}
		}

		public Window.Timer timer
		{
			get
			{
				return this._timer;
			}
		}

		public int width
		{
			get
			{
				return (int)this.rectTransform.sizeDelta.x;
			}
			set
			{
				Vector2 sizeDelta = this.rectTransform.sizeDelta;
				sizeDelta.x = (float)value;
				this.rectTransform.sizeDelta = sizeDelta;
			}
		}

		public int height
		{
			get
			{
				return (int)this.rectTransform.sizeDelta.y;
			}
			set
			{
				Vector2 sizeDelta = this.rectTransform.sizeDelta;
				sizeDelta.y = (float)value;
				this.rectTransform.sizeDelta = sizeDelta;
			}
		}

		protected bool initialized
		{
			get
			{
				return this._initialized;
			}
		}

		private void OnEnable()
		{
			base.StartCoroutine("OnEnableAsync");
		}

		protected virtual void Update()
		{
			if (!this._initialized)
			{
				return;
			}
			if (!this.hasFocus)
			{
				return;
			}
			if (this._updateCallback != null)
			{
				this._updateCallback(this._id);
			}
		}

		public virtual void Initialize(int id, Func<int, bool> isFocusedCallback)
		{
			if (this._initialized)
			{
				UnityEngine.Debug.LogError("Window is already initialized!");
				return;
			}
			this._id = id;
			this._isFocusedCallback = isFocusedCallback;
			this._timer = new Window.Timer();
			this._contentText = new List<Text>();
			this._canvasGroup = base.GetComponent<CanvasGroup>();
			this._initialized = true;
		}

		public void SetSize(int width, int height)
		{
			this.rectTransform.sizeDelta = new Vector2((float)width, (float)height);
		}

		public void CreateTitleText(GameObject prefab, Vector2 offset)
		{
			this.CreateText(prefab, ref this._titleText, "Title Text", UIPivot.TopCenter, Rewired.UI.UIAnchor.TopHStretch, offset);
		}

		public void CreateTitleText(GameObject prefab, Vector2 offset, string text)
		{
			this.CreateTitleText(prefab, offset);
			this.SetTitleText(text);
		}

		public void AddContentText(GameObject prefab, UIPivot pivot, Rewired.UI.UIAnchor anchor, Vector2 offset)
		{
			Text item = null;
			this.CreateText(prefab, ref item, "Content Text", pivot, anchor, offset);
			this._contentText.Add(item);
		}

		public void AddContentText(GameObject prefab, UIPivot pivot, Rewired.UI.UIAnchor anchor, Vector2 offset, string text)
		{
			this.AddContentText(prefab, pivot, anchor, offset);
			this.SetContentText(text, this._contentText.Count - 1);
		}

		public void AddContentImage(GameObject prefab, UIPivot pivot, Rewired.UI.UIAnchor anchor, Vector2 offset)
		{
			this.CreateImage(prefab, "Image", pivot, anchor, offset);
		}

		public void AddContentImage(GameObject prefab, UIPivot pivot, Rewired.UI.UIAnchor anchor, Vector2 offset, string text)
		{
			this.AddContentImage(prefab, pivot, anchor, offset);
		}

		public void CreateButton(GameObject prefab, UIPivot pivot, Rewired.UI.UIAnchor anchor, Vector2 offset, string buttonText, UnityAction confirmCallback, UnityAction cancelCallback, bool setDefault)
		{
			if (prefab == null)
			{
				return;
			}
			ButtonInfo buttonInfo;
			GameObject gameObject = this.CreateButton(prefab, "Button", anchor, pivot, offset, out buttonInfo);
			if (gameObject == null)
			{
				return;
			}
			UnityEngine.UI.Button component = gameObject.GetComponent<UnityEngine.UI.Button>();
			if (confirmCallback != null)
			{
				component.onClick.AddListener(confirmCallback);
			}
			CustomButton customButton = component as CustomButton;
			if (cancelCallback != null && customButton != null)
			{
				customButton.CancelEvent += cancelCallback;
			}
			if (buttonInfo.text != null)
			{
				buttonInfo.text.text = buttonText;
			}
			if (setDefault)
			{
				this._defaultUIElement = gameObject;
			}
		}

		public string GetTitleText(string text)
		{
			if (this._titleText == null)
			{
				return string.Empty;
			}
			return this._titleText.text;
		}

		public void SetTitleText(string text)
		{
			if (this._titleText == null)
			{
				return;
			}
			this._titleText.text = text;
		}

		public string GetContentText(int index)
		{
			if (this._contentText == null || this._contentText.Count <= index || this._contentText[index] == null)
			{
				return string.Empty;
			}
			return this._contentText[index].text;
		}

		public float GetContentTextHeight(int index)
		{
			if (this._contentText == null || this._contentText.Count <= index || this._contentText[index] == null)
			{
				return 0f;
			}
			return this._contentText[index].rectTransform.sizeDelta.y;
		}

		public void SetContentText(string text, int index)
		{
			if (this._contentText == null || this._contentText.Count <= index || this._contentText[index] == null)
			{
				return;
			}
			this._contentText[index].text = text;
		}

		public void SetUpdateCallback(Action<int> callback)
		{
			this.updateCallback = callback;
		}

		public virtual void TakeInputFocus()
		{
			if (EventSystem.current == null)
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(this._defaultUIElement);
			this.Enable();
		}

		public virtual void Enable()
		{
			this._canvasGroup.interactable = true;
		}

		public virtual void Disable()
		{
			this._canvasGroup.interactable = false;
		}

		public virtual void Cancel()
		{
			if (!this.initialized)
			{
				return;
			}
			if (this.cancelCallback != null)
			{
				this.cancelCallback();
			}
		}

		private void CreateText(GameObject prefab, ref Text textComponent, string name, UIPivot pivot, Rewired.UI.UIAnchor anchor, Vector2 offset)
		{
			if (prefab == null || this.content == null)
			{
				return;
			}
			if (textComponent != null)
			{
				UnityEngine.Debug.LogError("Window already has " + name + "!");
				return;
			}
			GameObject gameObject = UITools.InstantiateGUIObject<Text>(prefab, this.content.transform, name, pivot, anchor.min, anchor.max, offset);
			if (gameObject == null)
			{
				return;
			}
			textComponent = gameObject.GetComponent<Text>();
		}

		private void CreateImage(GameObject prefab, string name, UIPivot pivot, Rewired.UI.UIAnchor anchor, Vector2 offset)
		{
			if (prefab == null || this.content == null)
			{
				return;
			}
			UITools.InstantiateGUIObject<Image>(prefab, this.content.transform, name, pivot, anchor.min, anchor.max, offset);
		}

		private GameObject CreateButton(GameObject prefab, string name, Rewired.UI.UIAnchor anchor, UIPivot pivot, Vector2 offset, out ButtonInfo buttonInfo)
		{
			buttonInfo = null;
			if (prefab == null)
			{
				return null;
			}
			GameObject gameObject = UITools.InstantiateGUIObject<ButtonInfo>(prefab, this.content.transform, name, pivot, anchor.min, anchor.max, offset);
			if (gameObject == null)
			{
				return null;
			}
			buttonInfo = gameObject.GetComponent<ButtonInfo>();
			UnityEngine.UI.Button component = gameObject.GetComponent<UnityEngine.UI.Button>();
			if (component == null)
			{
				UnityEngine.Debug.Log("Button prefab is missing Button component!");
				return null;
			}
			if (buttonInfo == null)
			{
				UnityEngine.Debug.Log("Button prefab is missing ButtonInfo component!");
				return null;
			}
			return gameObject;
		}

		[DebuggerHidden]
		private IEnumerator OnEnableAsync()
		{
			Window.<OnEnableAsync>c__Iterator111 <OnEnableAsync>c__Iterator = new Window.<OnEnableAsync>c__Iterator111();
			<OnEnableAsync>c__Iterator.<>f__this = this;
			return <OnEnableAsync>c__Iterator;
		}
	}
}
