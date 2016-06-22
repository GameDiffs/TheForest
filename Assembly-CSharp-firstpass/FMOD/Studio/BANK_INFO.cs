using System;

namespace FMOD.Studio
{
	public struct BANK_INFO
	{
		public int size;

		public IntPtr userData;

		public int userDataLength;

		public FILE_OPENCALLBACK openCallback;

		public FILE_CLOSECALLBACK closeCallback;

		public FILE_READCALLBACK readCallback;

		public FILE_SEEKCALLBACK seekCallback;
	}
}
