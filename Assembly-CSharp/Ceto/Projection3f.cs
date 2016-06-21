using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ceto
{
	public class Projection3f : IProjection
	{
		private Ocean m_ocean;

		private Matrix4x4 m_projectorP;

		private Matrix4x4 m_projectorV;

		private Matrix4x4 m_projectorR;

		private Matrix4x4 m_projectorI;

		private Vector3[] m_frustumCorners;

		private List<Vector3> m_pointList;

		private static readonly Vector4[] m_quad = new Vector4[]
		{
			new Vector4(0f, 0f, 0f, 1f),
			new Vector4(1f, 0f, 0f, 1f),
			new Vector4(1f, 1f, 0f, 1f),
			new Vector4(0f, 1f, 0f, 1f)
		};

		private static readonly Vector4[] m_corners = new Vector4[]
		{
			new Vector4(-1f, -1f, -1f, 1f),
			new Vector4(1f, -1f, -1f, 1f),
			new Vector4(1f, 1f, -1f, 1f),
			new Vector4(-1f, 1f, -1f, 1f),
			new Vector4(-1f, -1f, 1f, 1f),
			new Vector4(1f, -1f, 1f, 1f),
			new Vector4(1f, 1f, 1f, 1f),
			new Vector4(-1f, 1f, 1f, 1f)
		};

		private static readonly int[,] m_indices = new int[,]
		{
			{
				0,
				1
			},
			{
				1,
				2
			},
			{
				2,
				3
			},
			{
				3,
				0
			},
			{
				4,
				5
			},
			{
				5,
				6
			},
			{
				6,
				7
			},
			{
				7,
				4
			},
			{
				0,
				4
			},
			{
				1,
				5
			},
			{
				2,
				6
			},
			{
				3,
				7
			}
		};

		public bool IsDouble
		{
			get
			{
				return false;
			}
		}

		public bool IsFlipped
		{
			get;
			set;
		}

		public Projection3f(Ocean ocean)
		{
			this.m_ocean = ocean;
			this.m_projectorP = default(Matrix4x4);
			this.m_projectorV = default(Matrix4x4);
			this.m_projectorR = Matrix4x4.identity;
			this.m_projectorI = default(Matrix4x4);
			this.m_pointList = new List<Vector3>(12);
			this.m_frustumCorners = new Vector3[8];
		}

		public void UpdateProjection(Camera cam, CameraData data, bool projectSceneView)
		{
			if (cam == null || data == null)
			{
				return;
			}
			if (data.projection == null)
			{
				data.projection = new ProjectionData();
			}
			if (data.projection.updated)
			{
				return;
			}
			if (!projectSceneView && cam.name == "SceneCamera" && Camera.main != null)
			{
				cam = Camera.main;
			}
			this.AimProjector(cam);
			Matrix4x4 matrix4x = this.m_projectorP * this.m_projectorV;
			this.CreateRangeMatrix(cam, matrix4x);
			Matrix4x4 ivp = matrix4x.inverse * this.m_projectorR;
			this.m_projectorI.SetRow(0, this.HProject(ivp, Projection3f.m_quad[0]));
			this.m_projectorI.SetRow(1, this.HProject(ivp, Projection3f.m_quad[1]));
			this.m_projectorI.SetRow(2, this.HProject(ivp, Projection3f.m_quad[2]));
			this.m_projectorI.SetRow(3, this.HProject(ivp, Projection3f.m_quad[3]));
			data.projection.projectorVP = this.m_projectorR.inverse * matrix4x;
			data.projection.interpolation = this.m_projectorI;
			data.projection.updated = true;
		}

		private void AimProjector(Camera cam)
		{
			this.m_projectorP = cam.projectionMatrix;
			Vector3 position = cam.transform.position;
			Vector3 forward = cam.transform.forward;
			Vector3 target = default(Vector3);
			float level = this.m_ocean.level;
			float num = Math.Max(0f, this.m_ocean.FindMaxDisplacement(true)) + 10f;
			if (Ocean.DISABLE_PROJECTION_FLIPPING)
			{
				if (position.y < level)
				{
					position.y = level;
				}
				this.IsFlipped = false;
				position.y = Math.Max(position.y, level + num);
			}
			else if (position.y < level)
			{
				this.IsFlipped = true;
				position.y = Math.Min(position.y, level - num);
			}
			else
			{
				this.IsFlipped = false;
				position.y = Math.Max(position.y, level + num);
			}
			target = position + forward * 50f;
			target.y = this.m_ocean.level;
			this.LookAt(position, target, Vector3.up);
		}

		private void CreateRangeMatrix(Camera cam, Matrix4x4 projectorVP)
		{
			this.m_pointList.Clear();
			Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
			Matrix4x4 inverse = (this.m_projectorP * worldToCameraMatrix).inverse;
			Vector3 up = Vector3.up;
			Vector4 v = Vector4.zero;
			float level = this.m_ocean.level;
			float num = Mathf.Max(1f, this.m_ocean.FindMaxDisplacement(true));
			for (int i = 0; i < 8; i++)
			{
				v = inverse * Projection3f.m_corners[i];
				v.x /= v.w;
				v.y /= v.w;
				v.z /= v.w;
				this.m_frustumCorners[i] = v;
			}
			for (int j = 0; j < 8; j++)
			{
				if (this.m_frustumCorners[j].y <= level + num && this.m_frustumCorners[j].y >= level - num)
				{
					this.m_pointList.Add(this.m_frustumCorners[j]);
				}
			}
			for (int k = 0; k < 12; k++)
			{
				Vector3 a = this.m_frustumCorners[Projection3f.m_indices[k, 0]];
				Vector3 b = this.m_frustumCorners[Projection3f.m_indices[k, 1]];
				Vector3 item = default(Vector3);
				Vector3 item2 = default(Vector3);
				if (this.SegmentPlaneIntersection(a, b, up, level + num, ref item))
				{
					this.m_pointList.Add(item);
				}
				if (this.SegmentPlaneIntersection(a, b, up, level - num, ref item2))
				{
					this.m_pointList.Add(item2);
				}
			}
			int count = this.m_pointList.Count;
			if (count == 0)
			{
				this.m_projectorR[0, 0] = 1f;
				this.m_projectorR[0, 3] = 0f;
				this.m_projectorR[1, 1] = 1f;
				this.m_projectorR[1, 3] = 0f;
				return;
			}
			float num2 = float.PositiveInfinity;
			float num3 = float.PositiveInfinity;
			float num4 = float.NegativeInfinity;
			float num5 = float.NegativeInfinity;
			Vector4 zero = Vector4.zero;
			for (int l = 0; l < count; l++)
			{
				zero.x = this.m_pointList[l].x;
				zero.y = level;
				zero.z = this.m_pointList[l].z;
				zero.w = 1f;
				v = projectorVP * zero;
				v.x /= v.w;
				v.y /= v.w;
				if (v.x < num2)
				{
					num2 = v.x;
				}
				if (v.y < num3)
				{
					num3 = v.y;
				}
				if (v.x > num4)
				{
					num4 = v.x;
				}
				if (v.y > num5)
				{
					num5 = v.y;
				}
			}
			this.m_projectorR[0, 0] = num4 - num2;
			this.m_projectorR[0, 3] = num2;
			this.m_projectorR[1, 1] = num5 - num3;
			this.m_projectorR[1, 3] = num3;
		}

		private Vector4 HProject(Matrix4x4 ivp, Vector4 corner)
		{
			corner.z = -1f;
			Vector4 vector = ivp * corner;
			corner.z = 1f;
			Vector4 a = ivp * corner;
			float level = this.m_ocean.level;
			Vector4 a2 = a - vector;
			float d = (vector.w * level - vector.y) / (a2.y - a2.w * level);
			return vector + a2 * d;
		}

		private bool SegmentPlaneIntersection(Vector3 a, Vector3 b, Vector3 n, float d, ref Vector3 q)
		{
			Vector3 vector = b - a;
			float num = (d - Vector3.Dot(n, a)) / Vector3.Dot(n, vector);
			if (num > -0f && num <= 1f)
			{
				q = a + num * vector;
				return true;
			}
			return false;
		}

		public void LookAt(Vector3 position, Vector3 target, Vector3 up)
		{
			Vector3 normalized = (position - target).normalized;
			Vector3 normalized2 = Vector3.Cross(up, normalized).normalized;
			Vector3 lhs = Vector3.Cross(normalized, normalized2);
			this.m_projectorV[0, 0] = normalized2.x;
			this.m_projectorV[0, 1] = normalized2.y;
			this.m_projectorV[0, 2] = normalized2.z;
			this.m_projectorV[0, 3] = -Vector3.Dot(normalized2, position);
			this.m_projectorV[1, 0] = lhs.x;
			this.m_projectorV[1, 1] = lhs.y;
			this.m_projectorV[1, 2] = lhs.z;
			this.m_projectorV[1, 3] = -Vector3.Dot(lhs, position);
			this.m_projectorV[2, 0] = normalized.x;
			this.m_projectorV[2, 1] = normalized.y;
			this.m_projectorV[2, 2] = normalized.z;
			this.m_projectorV[2, 3] = -Vector3.Dot(normalized, position);
			this.m_projectorV[3, 0] = 0f;
			this.m_projectorV[3, 1] = 0f;
			this.m_projectorV[3, 2] = 0f;
			this.m_projectorV[3, 3] = 1f;
			int num;
			int expr_16E = num = 0;
			int num2;
			int expr_172 = num2 = 0;
			float num3 = this.m_projectorV[num, num2];
			this.m_projectorV[expr_16E, expr_172] = num3 * -1f;
			int expr_199 = num2 = 0;
			int expr_19D = num = 1;
			num3 = this.m_projectorV[num2, num];
			this.m_projectorV[expr_199, expr_19D] = num3 * -1f;
			int expr_1C4 = num = 0;
			int expr_1C8 = num2 = 2;
			num3 = this.m_projectorV[num, num2];
			this.m_projectorV[expr_1C4, expr_1C8] = num3 * -1f;
			int expr_1EF = num2 = 0;
			int expr_1F3 = num = 3;
			num3 = this.m_projectorV[num2, num];
			this.m_projectorV[expr_1EF, expr_1F3] = num3 * -1f;
		}
	}
}
