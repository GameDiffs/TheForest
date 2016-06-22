using System;

namespace Valve.VR
{
	public struct RenderModel_t
	{
		public IntPtr rVertexData;

		public uint unVertexCount;

		public IntPtr rIndexData;

		public uint unTriangleCount;

		public int diffuseTextureId;
	}
}
