using System;

namespace Inspector
{
	public class GroupAttribute : InspectorAttribute
	{
		public bool drawFoldout = true;

		public GroupAttribute() : base(null)
		{
		}

		public GroupAttribute(string label) : base(label)
		{
		}
	}
}
