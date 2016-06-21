using System;
using System.Collections.Generic;
using UnityEngine;

public class TestJSON : MonoBehaviour
{
	public class TestClass
	{
		public int variable;

		public List<int> ints = new List<int>();

		public Dictionary<string, int> dic = new Dictionary<string, int>();

		public int[] intar = new int[]
		{
			1,
			2
		};

		public object[] ar = new object[2];

		public int[,] md = new int[2, 2];

		public int property
		{
			get;
			set;
		}
	}

	private void Awake()
	{
		Loom.Initialize();
	}

	private void OnGUI()
	{
		TestJSON.TestClass testClass = new TestJSON.TestClass
		{
			variable = 1,
			property = 2
		};
		testClass.ints.Add(1);
		testClass.ints.Add(2);
		testClass.dic["hello"] = 1;
		testClass.dic["mum"] = 2;
		testClass.intar[0] = 99;
		testClass.ar[0] = testClass.ints;
		testClass.ar[1] = testClass.dic;
		testClass.md[1, 1] = 1000;
		if (GUILayout.Button("Press me", new GUILayoutOption[0]))
		{
			JSONLevelSerializer.SerializeLevelToServer("ftp://whydoidoit.net/testIt.json", "testserializer", "T3sts3rializer", delegate(Exception error)
			{
				Debug.Log(base.transform.position.ToString());
			});
		}
		if (GUILayout.Button("Or me", new GUILayoutOption[0]))
		{
			LevelSerializer.LoadSavedLevelFromFile("test.data");
		}
	}
}
