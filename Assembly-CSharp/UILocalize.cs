using System;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Localize"), ExecuteInEditMode, RequireComponent(typeof(UIWidget))]
public class UILocalize : MonoBehaviour
{
	public string key;

	private bool mStarted;

	public string value
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				UIWidget component = base.GetComponent<UIWidget>();
				UILabel uILabel = component as UILabel;
				UISprite uISprite = component as UISprite;
				if (uILabel != null)
				{
					UIInput uIInput = NGUITools.FindInParents<UIInput>(uILabel.gameObject);
					if (uIInput != null && uIInput.label == uILabel)
					{
						uIInput.defaultText = value;
					}
					else
					{
						uILabel.text = value;
					}
				}
				else if (uISprite != null)
				{
					UIButton uIButton = NGUITools.FindInParents<UIButton>(uISprite.gameObject);
					if (uIButton != null && uIButton.tweenTarget == uISprite.gameObject)
					{
						uIButton.normalSprite = value;
					}
					uISprite.spriteName = value;
					uISprite.MakePixelPerfect();
				}
			}
		}
	}

	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnLocalize();
		}
	}

	private void Start()
	{
		this.mStarted = true;
		this.OnLocalize();
	}

	private void OnLocalize()
	{
		if (string.IsNullOrEmpty(this.key))
		{
			UILabel component = base.GetComponent<UILabel>();
			if (component != null)
			{
				this.key = component.text;
			}
		}
		if (!string.IsNullOrEmpty(this.key))
		{
			this.value = Localization.Get(this.key);
		}
	}
}
