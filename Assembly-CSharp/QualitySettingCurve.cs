using System;
using UnityEngine;

public class QualitySettingCurve : PropertyAttribute
{
	public int _qualityLevels;

	public QualitySettingCurve(int qualityLevels)
	{
		this._qualityLevels = qualityLevels;
	}
}
