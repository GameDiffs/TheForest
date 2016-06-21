using System;
using TheForest.UI;
using UnityEngine;

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class SheenBillboard : MonoBehaviour
{
	private static Mesh QuadMesh;

	public InputMappingIcons.Actions _action;

	public bool PickUp;

	private int wsToken = -1;

	private Renderer render;

	private float _Distance = 10f;

	public UISprite FillSprite
	{
		get;
		set;
	}

	private static Mesh GetSharedQuad()
	{
		if (SheenBillboard.QuadMesh == null)
		{
			SheenBillboard.QuadMesh = new Mesh();
			SheenBillboard.GenerateMesh(SheenBillboard.QuadMesh);
		}
		return SheenBillboard.QuadMesh;
	}

	public void Awake()
	{
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (component == null)
		{
			Debug.LogError("MeshFilter not found!");
			return;
		}
		this.render = base.GetComponent<Renderer>();
		component.sharedMesh = SheenBillboard.GetSharedQuad();
	}

	private void Start()
	{
		if (this.wsToken == -1)
		{
			this.wsToken = WorkScheduler.Register(new WorkScheduler.Task(this.RefreshVisibilityWork), base.transform.position, this.wsToken == -1);
		}
	}

	private void OnDestroy()
	{
		try
		{
			if (this.wsToken >= 0)
			{
				WorkScheduler.Unregister(new WorkScheduler.Task(this.RefreshVisibilityWork), this.wsToken);
				this.wsToken = -1;
			}
		}
		catch
		{
		}
	}

	public virtual void OnEnable()
	{
		if (this._action != InputMappingIcons.Actions.None)
		{
			this.FillSprite = ActionIconSystem.RegisterIcon(base.transform, this._action, ActionIconSystem.CurrentViewOptions.AllowInWorld);
		}
	}

	public virtual void OnDisable()
	{
		if (this._action != InputMappingIcons.Actions.None)
		{
			ActionIconSystem.UnregisterIcon(base.transform);
		}
	}

	private void RefreshVisibilityWork()
	{
		if (base.gameObject.activeInHierarchy)
		{
			Vector3 position = base.transform.position;
			position.y = PlayerCamLocation.PlayerLoc.y;
			float num = Vector3.Distance(position, PlayerCamLocation.PlayerLoc);
			if (base.enabled != num < this._Distance * 3f)
			{
				base.enabled = !base.enabled;
			}
		}
	}

	public void LateUpdate()
	{
		float sqrMagnitude = (base.transform.position - PlayerCamLocation.PlayerLoc).sqrMagnitude;
		bool flag = PlayerPreferences.ShowHud && sqrMagnitude < this._Distance * this._Distance;
		if (flag != this.render.enabled)
		{
			this.render.enabled = flag;
		}
	}

	private static void GenerateMesh(Mesh mesh)
	{
		int num = 1;
		Vector3[] array = new Vector3[4 * num];
		Vector2[] array2 = new Vector2[4 * num];
		int[] array3 = new int[6 * num];
		for (int i = 0; i < num; i++)
		{
			Vector3 zero = Vector3.zero;
			int num2 = 4 * i;
			array[num2] = zero;
			array[num2 + 1] = zero;
			array[num2 + 2] = zero;
			array[num2 + 3] = zero;
			array2[num2] = new Vector2(0f, 0f);
			array2[num2 + 1] = new Vector2(0f, 1f);
			array2[num2 + 2] = new Vector2(1f, 0f);
			array2[num2 + 3] = new Vector2(1f, 1f);
			int num3 = 6 * i;
			array3[num3] = 0 + num2;
			array3[num3 + 1] = 1 + num2;
			array3[num3 + 2] = 2 + num2;
			array3[num3 + 3] = 2 + num2;
			array3[num3 + 4] = 1 + num2;
			array3[num3 + 5] = 3 + num2;
		}
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		mesh.RecalculateNormals();
		mesh.bounds = new Bounds(Vector3.zero, Vector3.one);
		mesh.Optimize();
	}
}
