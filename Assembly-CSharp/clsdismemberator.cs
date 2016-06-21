using System;
using System.Collections.Generic;
using UnityEngine;

public class clsdismemberator : MonoBehaviour
{
	public SkinnedMeshRenderer vargamskinnedmeshrenderer;

	public Material vargamstumpmaterial;

	public GameObject vargamparticleparent;

	public GameObject vargamparticlechild;

	[HideInInspector]
	public Transform vargamfixer;

	[HideInInspector]
	public Vector3 vargamfixereuler;

	[HideInInspector]
	public Vector3[] vargamoriginalbonepositions = new Vector3[0];

	[HideInInspector]
	public Vector3[] vargamoriginalboneangles = new Vector3[0];

	[HideInInspector]
	public clsdismemberatorindexer[] vargamrigidbodyconnections;

	[HideInInspector]
	public clsdismemberatorindexer[] vargambonetriangles;

	[HideInInspector]
	public clsdismemberatorindexer[] vargambonetrianglesindexes;

	[HideInInspector]
	public clsdismemberatorindexer[] vargambonefulltrianglesindexes;

	[HideInInspector]
	public clsdismemberatorindexer[] vargambonevertices;

	[HideInInspector]
	public clsdismemberatorvertexindexer[] vargamboneverticesfullindex;

	[HideInInspector]
	public clsdismemberatorindexer[] vargamboneseparationvertices;

	[HideInInspector]
	public clsdismemberatorseparationverticesuvhelper[] vargamboneseparationverticesuvhelper;

	[HideInInspector]
	public clsdismemberatorindexer[] vargamboneseparationpatchtriangleindexes;

	[HideInInspector]
	public List<Transform> vargamboneindexes;

	[HideInInspector]
	public int[] vargamboneindexesparents;

	[HideInInspector]
	public int[] vargamboneindexescharacterjointconnect;

	[HideInInspector]
	public clsdismemberatorbonerelationsindexes[] vargambonerelationsindexes;

	[HideInInspector]
	public Vector2[] vargamboneseparationsubmeshhelper;

	[HideInInspector]
	public int vargamparallelcutcounter;

	[HideInInspector]
	public List<Transform> vargamcutpartscache;

	[HideInInspector]
	public bool[] vargamcutparts;
}
