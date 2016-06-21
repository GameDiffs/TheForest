using System;
using UnityEngine;

namespace TheForest.Buildings.Creation
{
	public class BuildLogShuffle : MonoBehaviour
	{
		public Transform Pos1;

		public Transform Pos2;

		public Transform Pos3;

		public Transform Pos4;

		private Vector3 P1;

		private Vector3 P2;

		private Vector3 P3;

		private Vector3 P4;

		private bool P1T;

		private bool P2T;

		private bool P3T;

		private bool P4T;

		public GameObject RockMission;

		public GameObject StickMission;

		public GameObject LeavesMission;

		public GameObject LogMission;

		private bool StickShowing;

		private bool LeavesShowing;

		private bool RockShowing;

		private bool LogShowing;

		private void Awake()
		{
			this.P1 = this.Pos1.position;
			this.P2 = this.Pos2.position;
			this.P3 = this.Pos3.position;
			this.P4 = this.Pos4.position;
		}
	}
}
