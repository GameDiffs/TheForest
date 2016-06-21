using System;
using System.Collections.Generic;
using UnityEngine;

public class spawnTerrainMesh : MonoBehaviour
{
	public GameObject natureSpawned;

	public GameObject naturePlaced;

	public List<GameObject> spawnedGo = new List<GameObject>();

	private Transform[] targets;

	private Transform[] greebleTargets;

	private Transform parent;

	private GameObject groupGo;

	public GameObject greebles;

	public Transform figTree;

	public Transform bigStump;

	public Transform beachRock;

	public Transform beachRockMossy;

	public Transform rock05;

	public Transform rock08;

	public Transform rockLichen;

	public Transform trunkDecayed;

	public Transform logDecayed;

	public GameObject tempTree;

	public Transform[] worldObj;
}
