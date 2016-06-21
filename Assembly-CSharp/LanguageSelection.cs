using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Language Selection"), RequireComponent(typeof(UIPopupList))]
public class LanguageSelection : MonoBehaviour
{
	private UIPopupList mList;

	private void Awake()
	{
		this.mList = base.GetComponent<UIPopupList>();
		this.Refresh();
	}

	private void Start()
	{
		EventDelegate.Add(this.mList.onChange, delegate
		{
			Localization.language = UIPopupList.current.value;
		});
	}

	public void Refresh()
	{
		if (this.mList != null && Localization.knownLanguages != null)
		{
			this.mList.Clear();
			int i = 0;
			int num = Localization.knownLanguages.Length;
			while (i < num)
			{
				this.mList.items.Add(Localization.knownLanguages[i]);
				i++;
			}
			this.mList.value = Localization.language;
		}
	}
}
