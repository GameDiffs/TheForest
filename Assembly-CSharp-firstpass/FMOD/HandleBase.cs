using System;

namespace FMOD
{
	public class HandleBase
	{
		protected IntPtr rawPtr;

		public HandleBase(IntPtr newPtr)
		{
			this.rawPtr = newPtr;
		}

		public bool isValid()
		{
			return this.rawPtr != IntPtr.Zero;
		}

		public IntPtr getRaw()
		{
			return this.rawPtr;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as HandleBase);
		}

		public bool Equals(HandleBase p)
		{
			return p != null && this.rawPtr == p.rawPtr;
		}

		public override int GetHashCode()
		{
			return this.rawPtr.ToInt32();
		}

		public static bool operator ==(HandleBase a, HandleBase b)
		{
			return object.ReferenceEquals(a, b) || (a != null && b != null && a.rawPtr == b.rawPtr);
		}

		public static bool operator !=(HandleBase a, HandleBase b)
		{
			return !(a == b);
		}
	}
}
