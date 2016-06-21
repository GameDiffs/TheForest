using System;
using UnityEngine;
using Valve.VR;

public class SteamVR_ControllerManager : MonoBehaviour
{
	public GameObject left;

	public GameObject right;

	public GameObject[] objects;

	private uint[] indices;

	private bool[] connected = new bool[16];

	private uint leftIndex = 4294967295u;

	private uint rightIndex = 4294967295u;

	private static string[] labels = new string[]
	{
		"left",
		"right"
	};

	private void Awake()
	{
		int num = (this.objects == null) ? 0 : this.objects.Length;
		GameObject[] array = new GameObject[2 + num];
		this.indices = new uint[2 + num];
		array[0] = this.right;
		this.indices[0] = 4294967295u;
		array[1] = this.left;
		this.indices[1] = 4294967295u;
		for (int i = 0; i < num; i++)
		{
			array[2 + i] = this.objects[i];
			this.indices[2 + i] = 4294967295u;
		}
		this.objects = array;
	}

	private void OnEnable()
	{
		for (int i = 0; i < this.objects.Length; i++)
		{
			GameObject gameObject = this.objects[i];
			if (gameObject != null)
			{
				gameObject.SetActive(false);
			}
		}
		this.OnTrackedDeviceRoleChanged(new object[0]);
		for (int j = 0; j < SteamVR.connected.Length; j++)
		{
			if (SteamVR.connected[j])
			{
				this.OnDeviceConnected(new object[]
				{
					j,
					true
				});
			}
		}
		SteamVR_Utils.Event.Listen("input_focus", new SteamVR_Utils.Event.Handler(this.OnInputFocus));
		SteamVR_Utils.Event.Listen("device_connected", new SteamVR_Utils.Event.Handler(this.OnDeviceConnected));
		SteamVR_Utils.Event.Listen("TrackedDeviceRoleChanged", new SteamVR_Utils.Event.Handler(this.OnTrackedDeviceRoleChanged));
	}

	private void OnDisable()
	{
		SteamVR_Utils.Event.Remove("input_focus", new SteamVR_Utils.Event.Handler(this.OnInputFocus));
		SteamVR_Utils.Event.Remove("device_connected", new SteamVR_Utils.Event.Handler(this.OnDeviceConnected));
		SteamVR_Utils.Event.Remove("TrackedDeviceRoleChanged", new SteamVR_Utils.Event.Handler(this.OnTrackedDeviceRoleChanged));
	}

	private void OnInputFocus(params object[] args)
	{
		bool flag = (bool)args[0];
		if (flag)
		{
			for (int i = 0; i < this.objects.Length; i++)
			{
				GameObject gameObject = this.objects[i];
				if (gameObject != null)
				{
					string str = (i >= 2) ? (i - 1).ToString() : SteamVR_ControllerManager.labels[i];
					this.ShowObject(gameObject.transform, "hidden (" + str + ")");
				}
			}
		}
		else
		{
			for (int j = 0; j < this.objects.Length; j++)
			{
				GameObject gameObject2 = this.objects[j];
				if (gameObject2 != null)
				{
					string str2 = (j >= 2) ? (j - 1).ToString() : SteamVR_ControllerManager.labels[j];
					this.HideObject(gameObject2.transform, "hidden (" + str2 + ")");
				}
			}
		}
	}

	private void HideObject(Transform t, string name)
	{
		Transform transform = new GameObject(name).transform;
		transform.parent = t.parent;
		t.parent = transform;
		transform.gameObject.SetActive(false);
	}

	private void ShowObject(Transform t, string name)
	{
		Transform parent = t.parent;
		if (parent.gameObject.name != name)
		{
			return;
		}
		t.parent = parent.parent;
		UnityEngine.Object.Destroy(parent.gameObject);
	}

	private void SetTrackedDeviceIndex(int objectIndex, uint trackedDeviceIndex)
	{
		if (trackedDeviceIndex != 4294967295u)
		{
			for (int i = 0; i < this.objects.Length; i++)
			{
				if (i != objectIndex && this.indices[i] == trackedDeviceIndex)
				{
					GameObject gameObject = this.objects[i];
					if (gameObject != null)
					{
						gameObject.SetActive(false);
					}
					this.indices[i] = 4294967295u;
				}
			}
		}
		if (trackedDeviceIndex != this.indices[objectIndex])
		{
			this.indices[objectIndex] = trackedDeviceIndex;
			GameObject gameObject2 = this.objects[objectIndex];
			if (gameObject2 != null)
			{
				if (trackedDeviceIndex == 4294967295u)
				{
					gameObject2.SetActive(false);
				}
				else
				{
					gameObject2.SetActive(true);
					gameObject2.BroadcastMessage("SetDeviceIndex", (int)trackedDeviceIndex, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	private void OnTrackedDeviceRoleChanged(params object[] args)
	{
		this.Refresh();
	}

	private void OnDeviceConnected(params object[] args)
	{
		uint num = (uint)((int)args[0]);
		bool flag = this.connected[(int)((UIntPtr)num)];
		this.connected[(int)((UIntPtr)num)] = false;
		bool flag2 = (bool)args[1];
		if (flag2)
		{
			CVRSystem system = OpenVR.System;
			if (system != null && system.GetTrackedDeviceClass(num) == ETrackedDeviceClass.Controller)
			{
				this.connected[(int)((UIntPtr)num)] = true;
				flag = !flag;
			}
		}
		if (flag)
		{
			this.Refresh();
		}
	}

	public void Refresh()
	{
		int i = 0;
		CVRSystem system = OpenVR.System;
		if (system != null)
		{
			this.leftIndex = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.LeftHand);
			this.rightIndex = system.GetTrackedDeviceIndexForControllerRole(ETrackedControllerRole.RightHand);
		}
		if (this.leftIndex == 4294967295u && this.rightIndex == 4294967295u)
		{
			uint num = 0u;
			while ((ulong)num < (ulong)((long)this.connected.Length))
			{
				if (this.connected[(int)((UIntPtr)num)])
				{
					this.SetTrackedDeviceIndex(i++, num);
					break;
				}
				num += 1u;
			}
		}
		else
		{
			this.SetTrackedDeviceIndex(i++, ((ulong)this.rightIndex >= (ulong)((long)this.connected.Length) || !this.connected[(int)((UIntPtr)this.rightIndex)]) ? 4294967295u : this.rightIndex);
			this.SetTrackedDeviceIndex(i++, ((ulong)this.leftIndex >= (ulong)((long)this.connected.Length) || !this.connected[(int)((UIntPtr)this.leftIndex)]) ? 4294967295u : this.leftIndex);
			if (this.leftIndex != 4294967295u && this.rightIndex != 4294967295u)
			{
				uint num2 = 0u;
				while ((ulong)num2 < (ulong)((long)this.connected.Length))
				{
					if (i >= this.objects.Length)
					{
						break;
					}
					if (this.connected[(int)((UIntPtr)num2)])
					{
						if (num2 != this.leftIndex && num2 != this.rightIndex)
						{
							this.SetTrackedDeviceIndex(i++, num2);
						}
					}
					num2 += 1u;
				}
			}
		}
		while (i < this.objects.Length)
		{
			this.SetTrackedDeviceIndex(i++, 4294967295u);
		}
	}
}
