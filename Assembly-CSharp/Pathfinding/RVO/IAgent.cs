using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.RVO
{
	public interface IAgent
	{
		Vector3 InterpolatedPosition
		{
			get;
		}

		Vector3 Position
		{
			get;
		}

		Vector3 DesiredVelocity
		{
			get;
			set;
		}

		Vector3 Velocity
		{
			get;
			set;
		}

		bool Locked
		{
			get;
			set;
		}

		float Radius
		{
			get;
			set;
		}

		float Height
		{
			get;
			set;
		}

		float MaxSpeed
		{
			get;
			set;
		}

		float NeighbourDist
		{
			get;
			set;
		}

		float AgentTimeHorizon
		{
			get;
			set;
		}

		float ObstacleTimeHorizon
		{
			get;
			set;
		}

		RVOLayer Layer
		{
			get;
			set;
		}

		RVOLayer CollidesWith
		{
			get;
			set;
		}

		bool DebugDraw
		{
			get;
			set;
		}

		int MaxNeighbours
		{
			get;
			set;
		}

		List<ObstacleVertex> NeighbourObstacles
		{
			get;
		}

		void SetYPosition(float yCoordinate);

		void Teleport(Vector3 pos);
	}
}
