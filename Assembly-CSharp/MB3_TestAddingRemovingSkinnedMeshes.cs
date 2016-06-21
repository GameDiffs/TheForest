using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class MB3_TestAddingRemovingSkinnedMeshes : MonoBehaviour
{
	public MB3_MeshBaker meshBaker;

	public GameObject[] g;

	private void Start()
	{
		base.StartCoroutine(this.TestScript());
	}

	[DebuggerHidden]
	private IEnumerator TestScript()
	{
		MB3_TestAddingRemovingSkinnedMeshes.<TestScript>c__Iterator107 <TestScript>c__Iterator = new MB3_TestAddingRemovingSkinnedMeshes.<TestScript>c__Iterator107();
		<TestScript>c__Iterator.<>f__this = this;
		return <TestScript>c__Iterator;
	}
}
