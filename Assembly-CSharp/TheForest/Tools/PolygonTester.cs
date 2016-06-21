using System;
using System.Collections.Generic;
using TheForest.Utils;
using UniLinq;
using UnityEngine;

namespace TheForest.Tools
{
	public class PolygonTester : MonoBehaviour
	{
		public List<Transform> _points;

		public Transform _tester;

		public bool _testerIsInPolygon;

		public bool _updateList = true;

		private List<Vector3> _points3d;

		private void Update()
		{
			if (this._updateList || this._points3d == null)
			{
				this._updateList = false;
				this._points3d = (from tr in this._points
				where tr != null
				select tr.position).ToList<Vector3>();
			}
			if (this._tester && this._points3d != null)
			{
				this._testerIsInPolygon = MathEx.IsPointInPolygon(this._tester.position, this._points3d);
			}
		}
	}
}
