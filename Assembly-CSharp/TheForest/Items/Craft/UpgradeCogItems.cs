using System;
using UnityEngine;

namespace TheForest.Items.Craft
{
	[Serializable]
	public class UpgradeCogItems
	{
		[ItemIdPicker]
		public int _itemId;

		public UpgradeCog.Patterns _pattern;

		public Transform _prefab;

		public float _alignedPatternExtents;

		public int _amount = 1;

		public int _maxViewsPerItem = 30;

		public bool _pointDownwards;

		public WeaponStatUpgrade[] _weaponStatUpgrades;

		public AnimationCurve _bonusDecayCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 1f),
			new Keyframe(1f, 0.1f)
		});
	}
}
