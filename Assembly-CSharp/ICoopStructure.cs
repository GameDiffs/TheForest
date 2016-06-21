using Bolt;
using System;
using System.Collections.Generic;
using UnityEngine;

public interface ICoopStructure
{
	bool WasBuilt
	{
		get;
		set;
	}

	bool WasPlaced
	{
		get;
		set;
	}

	int MultiPointsCount
	{
		get;
		set;
	}

	List<Vector3> MultiPointsPositions
	{
		get;
		set;
	}

	IProtocolToken CustomToken
	{
		get;
		set;
	}
}
