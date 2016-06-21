using System;

namespace Inspector
{
	public class FieldAttribute : InspectorAttribute
	{
		public bool allowSceneObjects = true;

		public FieldAttribute() : base(null)
		{
		}

		public FieldAttribute(string label) : base(label)
		{
		}
	}
}
