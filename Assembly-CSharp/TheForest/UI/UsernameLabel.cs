using Steamworks;
using System;
using UnityEngine;

namespace TheForest.UI
{
	public class UsernameLabel : MonoBehaviour
	{
		private void OnEnable()
		{
			UILabel component = base.GetComponent<UILabel>();
			if (component)
			{
				try
				{
					string personaName = SteamFriends.GetPersonaName();
					component.text = personaName;
				}
				catch (Exception var_2_24)
				{
					component.text = "Anonymous";
				}
			}
		}
	}
}
