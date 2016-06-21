using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TheForest
{
	public class DebugConsole : MonoBehaviour
	{
		private class LogContent
		{
			public GUIContent content;

			public LogType type;

			public int amount;
		}

		public bool _showOverlay;

		public bool _showLog;

		public bool _showConsole;

		public bool _showGamePadWheel;

		public GUIStyle _consoleRowStyle;

		public GUIStyle _logRowStyle;

		public GUIStyle _textStyle;

		public static int BatchedTasksNear;

		public static int BatchedTasksFar;

		private DebugConsoleRoutine _routineMB;

		private Coroutine _inputRoutine;

		private bool _destroyOnTitleSceneLoad;

		private int _maxLogs;

		private Vector2 _logsScrollPos;

		private Queue<DebugConsole.LogContent> _logs;

		private DebugConsole.LogContent _lastLog;

		private string[] _history = new string[100];

		private int _historyEnd = -1;

		private int _historyCurrent = -1;

		private string _consoleInput = string.Empty;

		private string _autocomplete;

		private char[] _alphaNum;

		private float _fps = 60f;

		private bool _showWSDetail;

		private bool _focusConsoleField;

		private bool _selectConsoleText;

		private Dictionary<string, MethodInfo> _availableConsoleMethods;

		private Dictionary<Type, int> _counters;

		private static Dictionary<Type, int> Counters;

		private void Awake()
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
