using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ExplosionFireAndDebris : MonoBehaviour
{
	public Transform[] debrisPrefabs;

	public Transform firePrefab;

	public int numDebrisPieces;

	public int numFires;

	[DebuggerHidden]
	private IEnumerator Start()
	{
		ExplosionFireAndDebris.<Start>c__Iterator11D <Start>c__Iterator11D = new ExplosionFireAndDebris.<Start>c__Iterator11D();
		<Start>c__Iterator11D.<>f__this = this;
		return <Start>c__Iterator11D;
	}

	private void AddFire(Transform t, Vector3 pos, Vector3 normal)
	{
		pos += normal * 0.5f;
		Transform transform = (Transform)UnityEngine.Object.Instantiate(this.firePrefab, pos, Quaternion.identity);
		transform.parent = t;
	}
}
