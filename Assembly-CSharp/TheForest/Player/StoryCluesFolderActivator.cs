using System;
using UnityEngine;

namespace TheForest.Player
{
	public class StoryCluesFolderActivator : MonoBehaviour
	{
		public StoryCluesFolder _target;

		private void Awake()
		{
			this._target.Init();
		}
	}
}
