using System;
using System.Collections.Generic;
using UnityEngine;

public class UnderfootSurfaceDetector : MonoBehaviour
{
	public enum SurfaceType
	{
		None,
		Wood,
		Rock,
		Carpet,
		Dirt,
		Metal
	}

	public const string TAG_WOOD = "UnderfootWood";

	private Collider collider;

	private List<Collider> colliders;

	private static Dictionary<string, UnderfootSurfaceDetector.SurfaceType> SurfaceTags = new Dictionary<string, UnderfootSurfaceDetector.SurfaceType>
	{
		{
			"UnderfootWood",
			UnderfootSurfaceDetector.SurfaceType.Wood
		},
		{
			"BreakableWood",
			UnderfootSurfaceDetector.SurfaceType.Wood
		},
		{
			"structure",
			UnderfootSurfaceDetector.SurfaceType.Wood
		},
		{
			"SLTier1",
			UnderfootSurfaceDetector.SurfaceType.Wood
		},
		{
			"SLTier2",
			UnderfootSurfaceDetector.SurfaceType.Wood
		},
		{
			"SLTier3",
			UnderfootSurfaceDetector.SurfaceType.Wood
		},
		{
			"DeadTree",
			UnderfootSurfaceDetector.SurfaceType.Wood
		},
		{
			"Multisled",
			UnderfootSurfaceDetector.SurfaceType.Wood
		},
		{
			"Target",
			UnderfootSurfaceDetector.SurfaceType.Wood
		},
		{
			"UnderfootRock",
			UnderfootSurfaceDetector.SurfaceType.Rock
		},
		{
			"BreakableRock",
			UnderfootSurfaceDetector.SurfaceType.Rock
		},
		{
			"Block",
			UnderfootSurfaceDetector.SurfaceType.Rock
		},
		{
			"UnderfootMetal",
			UnderfootSurfaceDetector.SurfaceType.Metal
		},
		{
			"UnderfootCarpet",
			UnderfootSurfaceDetector.SurfaceType.Carpet
		},
		{
			"UnderfootDirt",
			UnderfootSurfaceDetector.SurfaceType.Dirt
		}
	};

	private static HashSet<string> CheckComponentTags = new HashSet<string>
	{
		"jumpObject"
	};

	public UnderfootSurfaceDetector.SurfaceType Surface
	{
		get;
		private set;
	}

	private void Start()
	{
		this.collider = base.GetComponent<Collider>();
		this.colliders = new List<Collider>();
		this.Surface = UnderfootSurfaceDetector.SurfaceType.None;
		base.InvokeRepeating("ValidateColliders", 1f, 1f);
	}

	public static UnderfootSurfaceDetector.SurfaceType GetSurfaceType(Collider collider)
	{
		UnderfootSurfaceDetector.SurfaceType result;
		if (UnderfootSurfaceDetector.SurfaceTags.TryGetValue(collider.tag, out result))
		{
			return result;
		}
		if (UnderfootSurfaceDetector.CheckComponentTags.Contains(collider.tag))
		{
			UnderfootSurface component = collider.GetComponent<UnderfootSurface>();
			if (component != null)
			{
				return component.surfaceType;
			}
		}
		return UnderfootSurfaceDetector.SurfaceType.None;
	}

	private void UpdateSurface()
	{
		if (this.colliders.Count > 0)
		{
			this.Surface = UnderfootSurfaceDetector.GetSurfaceType(this.colliders[this.colliders.Count - 1]);
		}
		else
		{
			this.Surface = UnderfootSurfaceDetector.SurfaceType.None;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		UnderfootSurfaceDetector.SurfaceType surfaceType = UnderfootSurfaceDetector.GetSurfaceType(other);
		if (surfaceType != UnderfootSurfaceDetector.SurfaceType.None)
		{
			int i = 0;
			while (i < this.colliders.Count)
			{
				if (this.colliders[i] == null || this.colliders[i] == other)
				{
					this.colliders.RemoveAt(i);
				}
				else
				{
					i++;
				}
			}
			this.colliders.Add(other);
			this.Surface = surfaceType;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		bool flag = false;
		int i = 0;
		while (i < this.colliders.Count)
		{
			if (this.colliders[i] == null || this.colliders[i] == other)
			{
				this.colliders.RemoveAt(i);
				flag = true;
			}
			else
			{
				i++;
			}
		}
		if (flag)
		{
			this.UpdateSurface();
		}
	}

	private void ValidateColliders()
	{
		bool flag = false;
		int i = 0;
		while (i < this.colliders.Count)
		{
			if (this.colliders[i] == null || !this.colliders[i].bounds.Intersects(this.collider.bounds))
			{
				this.colliders.RemoveAt(i);
				flag = true;
			}
			else
			{
				i++;
			}
		}
		if (flag)
		{
			this.UpdateSurface();
		}
	}
}
