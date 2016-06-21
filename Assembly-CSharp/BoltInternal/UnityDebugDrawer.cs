using Bolt;
using System;
using UnityEngine;

namespace BoltInternal
{
	public class UnityDebugDrawer : IDebugDrawer
	{
		private bool isEditor;

		void IDebugDrawer.IsEditor(bool isEditor)
		{
			this.isEditor = isEditor;
		}

		void IDebugDrawer.SelectGameObject(GameObject gameObject)
		{
		}

		void IDebugDrawer.Indent(int level)
		{
		}

		void IDebugDrawer.Label(string text)
		{
			DebugInfo.Label(text);
		}

		void IDebugDrawer.LabelBold(string text)
		{
			DebugInfo.LabelBold(text);
		}

		void IDebugDrawer.LabelField(string text, object value)
		{
			DebugInfo.LabelField(text, value);
		}

		void IDebugDrawer.Separator()
		{
			GUILayout.Space(2f);
		}
	}
}
