using System;
using UnityEngine;

public class getStructureStrength : MonoBehaviour
{
	public enum strength
	{
		weak,
		normal,
		strong,
		veryStrong
	}

	public enum structureType
	{
		wall,
		floor,
		foundation
	}

	public getStructureStrength.strength _strength;

	public getStructureStrength.structureType _type;
}
