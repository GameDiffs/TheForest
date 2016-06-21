using System;
using UnityEngine;

namespace TheForest.UI
{
	public class MpGameRow : MonoBehaviour
	{
		public UILabel _gameName;

		public UILabel _playerLimit;

		public UILabel _continueButtonLabel;

		public UILabel _newButtonLabel;

		public CoopLobbyInfo _lobby;

		public bool _previousPlayed;
	}
}
