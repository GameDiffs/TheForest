using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMOD.Studio
{
	public class Util
	{
		public static RESULT ParseID(string idString, out Guid id)
		{
			return Util.FMOD_Studio_ParseID(Encoding.UTF8.GetBytes(idString + '\0'), out id);
		}

		[DllImport("fmodstudio")]
		private static extern RESULT FMOD_Studio_ParseID(byte[] idString, out Guid id);
	}
}
