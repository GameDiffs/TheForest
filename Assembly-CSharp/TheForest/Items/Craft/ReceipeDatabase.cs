using System;
using UnityEngine;

namespace TheForest.Items.Craft
{
	public class ReceipeDatabase : ScriptableObject
	{
		public int _lastId;

		public Receipe[] _receipes;

		public static ReceipeDatabase _instance;

		public static Receipe[] Receipes
		{
			get
			{
				return ReceipeDatabase._instance._receipes;
			}
		}

		public void OnEnable()
		{
			base.hideFlags = HideFlags.None;
			if (ReceipeDatabase._instance == null)
			{
				ReceipeDatabase._instance = this;
			}
			else
			{
				Debug.LogError("Multiple Receipe Database");
			}
		}
	}
}
