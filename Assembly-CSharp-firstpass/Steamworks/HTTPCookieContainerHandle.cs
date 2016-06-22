using System;

namespace Steamworks
{
	public struct HTTPCookieContainerHandle : IEquatable<HTTPCookieContainerHandle>, IComparable<HTTPCookieContainerHandle>
	{
		public static readonly HTTPCookieContainerHandle Invalid = new HTTPCookieContainerHandle(0u);

		public uint m_HTTPCookieContainerHandle;

		public HTTPCookieContainerHandle(uint value)
		{
			this.m_HTTPCookieContainerHandle = value;
		}

		public override string ToString()
		{
			return this.m_HTTPCookieContainerHandle.ToString();
		}

		public override bool Equals(object other)
		{
			return other is HTTPCookieContainerHandle && this == (HTTPCookieContainerHandle)other;
		}

		public override int GetHashCode()
		{
			return this.m_HTTPCookieContainerHandle.GetHashCode();
		}

		public bool Equals(HTTPCookieContainerHandle other)
		{
			return this.m_HTTPCookieContainerHandle == other.m_HTTPCookieContainerHandle;
		}

		public int CompareTo(HTTPCookieContainerHandle other)
		{
			return this.m_HTTPCookieContainerHandle.CompareTo(other.m_HTTPCookieContainerHandle);
		}

		public static bool operator ==(HTTPCookieContainerHandle x, HTTPCookieContainerHandle y)
		{
			return x.m_HTTPCookieContainerHandle == y.m_HTTPCookieContainerHandle;
		}

		public static bool operator !=(HTTPCookieContainerHandle x, HTTPCookieContainerHandle y)
		{
			return !(x == y);
		}

		public static explicit operator HTTPCookieContainerHandle(uint value)
		{
			return new HTTPCookieContainerHandle(value);
		}

		public static explicit operator uint(HTTPCookieContainerHandle that)
		{
			return that.m_HTTPCookieContainerHandle;
		}
	}
}
