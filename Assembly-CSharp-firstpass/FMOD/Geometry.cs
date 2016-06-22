using System;
using System.Runtime.InteropServices;

namespace FMOD
{
	public class Geometry : HandleBase
	{
		public Geometry(IntPtr raw) : base(raw)
		{
		}

		public RESULT release()
		{
			RESULT rESULT = Geometry.FMOD5_Geometry_Release(base.getRaw());
			if (rESULT == RESULT.OK)
			{
				this.rawPtr = IntPtr.Zero;
			}
			return rESULT;
		}

		public RESULT addPolygon(float directocclusion, float reverbocclusion, bool doublesided, int numvertices, VECTOR[] vertices, out int polygonindex)
		{
			return Geometry.FMOD5_Geometry_AddPolygon(this.rawPtr, directocclusion, reverbocclusion, doublesided, numvertices, vertices, out polygonindex);
		}

		public RESULT getNumPolygons(out int numpolygons)
		{
			return Geometry.FMOD5_Geometry_GetNumPolygons(this.rawPtr, out numpolygons);
		}

		public RESULT getMaxPolygons(out int maxpolygons, out int maxvertices)
		{
			return Geometry.FMOD5_Geometry_GetMaxPolygons(this.rawPtr, out maxpolygons, out maxvertices);
		}

		public RESULT getPolygonNumVertices(int index, out int numvertices)
		{
			return Geometry.FMOD5_Geometry_GetPolygonNumVertices(this.rawPtr, index, out numvertices);
		}

		public RESULT setPolygonVertex(int index, int vertexindex, ref VECTOR vertex)
		{
			return Geometry.FMOD5_Geometry_SetPolygonVertex(this.rawPtr, index, vertexindex, ref vertex);
		}

		public RESULT getPolygonVertex(int index, int vertexindex, out VECTOR vertex)
		{
			return Geometry.FMOD5_Geometry_GetPolygonVertex(this.rawPtr, index, vertexindex, out vertex);
		}

		public RESULT setPolygonAttributes(int index, float directocclusion, float reverbocclusion, bool doublesided)
		{
			return Geometry.FMOD5_Geometry_SetPolygonAttributes(this.rawPtr, index, directocclusion, reverbocclusion, doublesided);
		}

		public RESULT getPolygonAttributes(int index, out float directocclusion, out float reverbocclusion, out bool doublesided)
		{
			return Geometry.FMOD5_Geometry_GetPolygonAttributes(this.rawPtr, index, out directocclusion, out reverbocclusion, out doublesided);
		}

		public RESULT setActive(bool active)
		{
			return Geometry.FMOD5_Geometry_SetActive(this.rawPtr, active);
		}

		public RESULT getActive(out bool active)
		{
			return Geometry.FMOD5_Geometry_GetActive(this.rawPtr, out active);
		}

		public RESULT setRotation(ref VECTOR forward, ref VECTOR up)
		{
			return Geometry.FMOD5_Geometry_SetRotation(this.rawPtr, ref forward, ref up);
		}

		public RESULT getRotation(out VECTOR forward, out VECTOR up)
		{
			return Geometry.FMOD5_Geometry_GetRotation(this.rawPtr, out forward, out up);
		}

		public RESULT setPosition(ref VECTOR position)
		{
			return Geometry.FMOD5_Geometry_SetPosition(this.rawPtr, ref position);
		}

		public RESULT getPosition(out VECTOR position)
		{
			return Geometry.FMOD5_Geometry_GetPosition(this.rawPtr, out position);
		}

		public RESULT setScale(ref VECTOR scale)
		{
			return Geometry.FMOD5_Geometry_SetScale(this.rawPtr, ref scale);
		}

		public RESULT getScale(out VECTOR scale)
		{
			return Geometry.FMOD5_Geometry_GetScale(this.rawPtr, out scale);
		}

		public RESULT save(IntPtr data, out int datasize)
		{
			return Geometry.FMOD5_Geometry_Save(this.rawPtr, data, out datasize);
		}

		public RESULT setUserData(IntPtr userdata)
		{
			return Geometry.FMOD5_Geometry_SetUserData(this.rawPtr, userdata);
		}

		public RESULT getUserData(out IntPtr userdata)
		{
			return Geometry.FMOD5_Geometry_GetUserData(this.rawPtr, out userdata);
		}

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_Release(IntPtr geometry);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_AddPolygon(IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, VECTOR[] vertices, out int polygonindex);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_GetNumPolygons(IntPtr geometry, out int numpolygons);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_GetMaxPolygons(IntPtr geometry, out int maxpolygons, out int maxvertices);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_GetPolygonNumVertices(IntPtr geometry, int index, out int numvertices);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_SetPolygonVertex(IntPtr geometry, int index, int vertexindex, ref VECTOR vertex);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_GetPolygonVertex(IntPtr geometry, int index, int vertexindex, out VECTOR vertex);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_SetPolygonAttributes(IntPtr geometry, int index, float directocclusion, float reverbocclusion, bool doublesided);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_GetPolygonAttributes(IntPtr geometry, int index, out float directocclusion, out float reverbocclusion, out bool doublesided);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_SetActive(IntPtr geometry, bool active);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_GetActive(IntPtr geometry, out bool active);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_SetRotation(IntPtr geometry, ref VECTOR forward, ref VECTOR up);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_GetRotation(IntPtr geometry, out VECTOR forward, out VECTOR up);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_SetPosition(IntPtr geometry, ref VECTOR position);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_GetPosition(IntPtr geometry, out VECTOR position);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_SetScale(IntPtr geometry, ref VECTOR scale);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_GetScale(IntPtr geometry, out VECTOR scale);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_Save(IntPtr geometry, IntPtr data, out int datasize);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_SetUserData(IntPtr geometry, IntPtr userdata);

		[DllImport("fmod")]
		private static extern RESULT FMOD5_Geometry_GetUserData(IntPtr geometry, out IntPtr userdata);
	}
}
