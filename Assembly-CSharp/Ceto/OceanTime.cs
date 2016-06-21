using System;
using UnityEngine;

namespace Ceto
{
	public class OceanTime : IOceanTime
	{
		public float Now
		{
			get
			{
				return Time.time;
			}
		}
	}
}
