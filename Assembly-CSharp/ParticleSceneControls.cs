using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSceneControls : MonoBehaviour
{
	public enum Mode
	{
		Activate,
		Instantiate,
		Trail
	}

	public enum AlignMode
	{
		Normal,
		Up
	}

	[Serializable]
	public class DemoParticleSystem
	{
		public Transform transform;

		public ParticleSceneControls.Mode mode;

		public ParticleSceneControls.AlignMode align;

		public int maxCount;

		public float minDist;

		public int camOffset = 15;

		public string instructionText;
	}

	[Serializable]
	public class DemoParticleSystemList
	{
		public ParticleSceneControls.DemoParticleSystem[] items;
	}

	public ParticleSceneControls.DemoParticleSystemList demoParticles;

	public float distFromSurface = 0.5f;

	public float multiply = 1f;

	public bool clearOnChange;

	public GUIText titleGuiText;

	public Transform demoCam;

	public GUIText interactionGuiText;

	private ParticleSystemMultiplier particleMultiplier;

	private List<Transform> currentParticleList = new List<Transform>();

	private Transform instance;

	private static int selectedIndex;

	private Vector3 camOffsetVelocity = Vector3.zero;

	private Vector3 lastPos;

	private static ParticleSceneControls.DemoParticleSystem selected;

	private void Awake()
	{
		this.Select(ParticleSceneControls.selectedIndex);
	}

	private void Update()
	{
		this.demoCam.localPosition = Vector3.SmoothDamp(this.demoCam.localPosition, Vector3.forward * (float)(-(float)ParticleSceneControls.selected.camOffset), ref this.camOffsetVelocity, 1f);
		if (CrossPlatformInput.GetButtonDown("NextParticleSystem"))
		{
			ParticleSceneControls.selectedIndex++;
			if (ParticleSceneControls.selectedIndex == this.demoParticles.items.Length)
			{
				ParticleSceneControls.selectedIndex = 0;
			}
			this.Select(ParticleSceneControls.selectedIndex);
			return;
		}
		if (CrossPlatformInput.GetButtonDown("PreviousParticleSystem"))
		{
			ParticleSceneControls.selectedIndex--;
			if (ParticleSceneControls.selectedIndex == -1)
			{
				ParticleSceneControls.selectedIndex = this.demoParticles.items.Length - 1;
			}
			this.Select(ParticleSceneControls.selectedIndex);
			return;
		}
		if (ParticleSceneControls.selected.mode == ParticleSceneControls.Mode.Activate)
		{
			return;
		}
		bool flag = Input.GetMouseButtonDown(0) && ParticleSceneControls.selected.mode == ParticleSceneControls.Mode.Instantiate;
		bool flag2 = Input.GetMouseButton(0) && ParticleSceneControls.selected.mode == ParticleSceneControls.Mode.Trail;
		if (flag || flag2)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit))
			{
				Quaternion rotation = Quaternion.LookRotation(raycastHit.normal);
				if (ParticleSceneControls.selected.align == ParticleSceneControls.AlignMode.Up)
				{
					rotation = Quaternion.identity;
				}
				Vector3 vector = raycastHit.point + raycastHit.normal * this.distFromSurface;
				if ((vector - this.lastPos).magnitude > ParticleSceneControls.selected.minDist)
				{
					if (ParticleSceneControls.selected.mode != ParticleSceneControls.Mode.Trail || this.instance == null)
					{
						this.instance = (Transform)UnityEngine.Object.Instantiate(ParticleSceneControls.selected.transform, vector, rotation);
						if (this.particleMultiplier != null)
						{
							this.instance.GetComponent<ParticleSystemMultiplier>().multiplier = this.multiply;
						}
						this.currentParticleList.Add(this.instance);
						if (ParticleSceneControls.selected.maxCount > 0 && this.currentParticleList.Count > ParticleSceneControls.selected.maxCount)
						{
							if (this.currentParticleList[0] != null)
							{
								UnityEngine.Object.Destroy(this.currentParticleList[0].gameObject);
							}
							this.currentParticleList.RemoveAt(0);
						}
					}
					else
					{
						this.instance.position = vector;
						this.instance.rotation = rotation;
					}
					if (ParticleSceneControls.selected.mode == ParticleSceneControls.Mode.Trail)
					{
						this.instance.transform.GetComponent<ParticleSystem>().enableEmission = false;
						this.instance.transform.GetComponent<ParticleSystem>().Emit(1);
					}
					this.instance.parent = raycastHit.transform;
					this.lastPos = vector;
				}
			}
		}
	}

	private void Select(int i)
	{
		ParticleSceneControls.selected = this.demoParticles.items[i];
		this.instance = null;
		ParticleSceneControls.DemoParticleSystem[] items = this.demoParticles.items;
		for (int j = 0; j < items.Length; j++)
		{
			ParticleSceneControls.DemoParticleSystem demoParticleSystem = items[j];
			if (demoParticleSystem != ParticleSceneControls.selected && demoParticleSystem.mode == ParticleSceneControls.Mode.Activate && demoParticleSystem != null)
			{
				demoParticleSystem.transform.gameObject.SetActive(false);
			}
		}
		if (ParticleSceneControls.selected.mode == ParticleSceneControls.Mode.Activate)
		{
			ParticleSceneControls.selected.transform.gameObject.SetActive(true);
		}
		this.particleMultiplier = ParticleSceneControls.selected.transform.GetComponent<ParticleSystemMultiplier>();
		this.multiply = 1f;
		if (this.clearOnChange)
		{
			while (this.currentParticleList.Count > 0)
			{
				UnityEngine.Object.Destroy(this.currentParticleList[0].gameObject);
				this.currentParticleList.RemoveAt(0);
			}
		}
		this.interactionGuiText.text = ParticleSceneControls.selected.instructionText;
		this.titleGuiText.text = ParticleSceneControls.selected.transform.name;
	}
}
