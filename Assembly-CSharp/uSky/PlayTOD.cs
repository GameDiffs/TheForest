using System;
using UnityEngine;

namespace uSky
{
	[AddComponentMenu("uSky/Play TOD"), RequireComponent(typeof(uSkyManager))]
	public class PlayTOD : MonoBehaviour
	{
		public bool PlayTimelapse = true;

		public float PlaySpeed = 0.1f;

		private uSkyManager m_uSM;

		private uSkyManager uSM
		{
			get
			{
				if (this.m_uSM == null)
				{
					this.m_uSM = base.gameObject.GetComponent<uSkyManager>();
					if (this.m_uSM == null)
					{
						Debug.Log("Can't not find uSkyManager");
					}
				}
				return this.m_uSM;
			}
		}

		private void Start()
		{
			if (this.PlayTimelapse)
			{
				this.uSM.SkyUpdate = true;
			}
		}

		private void Update()
		{
			if (this.PlayTimelapse)
			{
				this.uSM.Timeline = this.uSM.Timeline + Time.deltaTime * this.PlaySpeed;
			}
		}
	}
}
