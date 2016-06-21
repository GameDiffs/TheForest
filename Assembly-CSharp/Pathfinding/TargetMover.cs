using System;
using UnityEngine;

namespace Pathfinding
{
	public class TargetMover : MonoBehaviour
	{
		public LayerMask mask;

		public Transform target;

		private RichAI[] ais;

		private AIPath[] ais2;

		public bool onlyOnDoubleClick;

		private Camera cam;

		public void Start()
		{
			this.cam = Camera.main;
			this.ais = (UnityEngine.Object.FindObjectsOfType(typeof(RichAI)) as RichAI[]);
			this.ais2 = (UnityEngine.Object.FindObjectsOfType(typeof(AIPath)) as AIPath[]);
		}

		public void OnGUI()
		{
			if (this.onlyOnDoubleClick && this.cam != null && Event.current.type == EventType.MouseDown && Event.current.clickCount == 2)
			{
				this.UpdateTargetPosition();
			}
		}

		private void Update()
		{
			if (!this.onlyOnDoubleClick && this.cam != null)
			{
				this.UpdateTargetPosition();
			}
		}

		public void UpdateTargetPosition()
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(this.cam.ScreenPointToRay(Input.mousePosition), out raycastHit, float.PositiveInfinity, this.mask) && raycastHit.point != this.target.position)
			{
				this.target.position = raycastHit.point;
				if (this.ais != null && this.onlyOnDoubleClick)
				{
					for (int i = 0; i < this.ais.Length; i++)
					{
						if (this.ais[i] != null)
						{
							this.ais[i].UpdatePath();
						}
					}
				}
				if (this.ais2 != null && this.onlyOnDoubleClick)
				{
					for (int j = 0; j < this.ais2.Length; j++)
					{
						if (this.ais2[j] != null)
						{
							this.ais2[j].SearchPath();
						}
					}
				}
			}
		}
	}
}
