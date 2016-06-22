using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class CraterMaker : MonoBehaviour
{
	public Terrain MyTerrain;

	public int insidetextureindex;

	public void Create(Vector3 position, float radius, float depth, float noise)
	{
		this.Create(new Vector2(position.x, position.z), radius, depth, noise);
	}

	public void Create(Vector2 position, float radius, float depth, float noise)
	{
		base.StartCoroutine(this.RealCreate(position, radius, depth, noise));
	}

	[DebuggerHidden]
	public IEnumerator RealCreate(Vector2 position, float radius, float depth, float noise)
	{
		CraterMaker.<RealCreate>c__Iterator15 <RealCreate>c__Iterator = new CraterMaker.<RealCreate>c__Iterator15();
		<RealCreate>c__Iterator.position = position;
		<RealCreate>c__Iterator.radius = radius;
		<RealCreate>c__Iterator.depth = depth;
		<RealCreate>c__Iterator.noise = noise;
		<RealCreate>c__Iterator.<$>position = position;
		<RealCreate>c__Iterator.<$>radius = radius;
		<RealCreate>c__Iterator.<$>depth = depth;
		<RealCreate>c__Iterator.<$>noise = noise;
		<RealCreate>c__Iterator.<>f__this = this;
		return <RealCreate>c__Iterator;
	}
}
