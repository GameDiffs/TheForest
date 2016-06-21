using System;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Key Navigation")]
public class UIKeyNavigation : MonoBehaviour
{
	public enum Constraint
	{
		None,
		Vertical,
		Horizontal,
		Explicit
	}

	public static BetterList<UIKeyNavigation> list = new BetterList<UIKeyNavigation>();

	public UIKeyNavigation.Constraint constraint;

	public GameObject onUp;

	public GameObject onDown;

	public GameObject onLeft;

	public GameObject onRight;

	public GameObject onClick;

	public GameObject onTab;

	public bool startsSelected;

	[NonSerialized]
	private bool mStarted;

	public static UIKeyNavigation current
	{
		get
		{
			GameObject hoveredObject = UICamera.hoveredObject;
			if (hoveredObject == null)
			{
				return null;
			}
			return hoveredObject.GetComponent<UIKeyNavigation>();
		}
	}

	public bool isColliderEnabled
	{
		get
		{
			if (!base.enabled || !base.gameObject.activeInHierarchy)
			{
				return false;
			}
			Collider component = base.GetComponent<Collider>();
			if (component != null)
			{
				return component.enabled;
			}
			Collider2D component2 = base.GetComponent<Collider2D>();
			return component2 != null && component2.enabled;
		}
	}

	protected virtual void OnEnable()
	{
		UIKeyNavigation.list.Add(this);
		if (this.mStarted)
		{
			this.Start();
		}
	}

	private void Start()
	{
		this.mStarted = true;
		if (this.startsSelected && this.isColliderEnabled)
		{
			UICamera.hoveredObject = base.gameObject;
		}
	}

	protected virtual void OnDisable()
	{
		UIKeyNavigation.list.Remove(this);
	}

	private static bool IsActive(GameObject go)
	{
		if (!go || !go.activeInHierarchy)
		{
			return false;
		}
		Collider component = go.GetComponent<Collider>();
		if (component != null)
		{
			return component.enabled;
		}
		Collider2D component2 = go.GetComponent<Collider2D>();
		return component2 != null && component2.enabled;
	}

	public GameObject GetLeft()
	{
		if (UIKeyNavigation.IsActive(this.onLeft))
		{
			return this.onLeft;
		}
		if (this.constraint == UIKeyNavigation.Constraint.Vertical || this.constraint == UIKeyNavigation.Constraint.Explicit)
		{
			return null;
		}
		return this.Get(Vector3.left, 1f, 2f);
	}

	public GameObject GetRight()
	{
		if (UIKeyNavigation.IsActive(this.onRight))
		{
			return this.onRight;
		}
		if (this.constraint == UIKeyNavigation.Constraint.Vertical || this.constraint == UIKeyNavigation.Constraint.Explicit)
		{
			return null;
		}
		return this.Get(Vector3.right, 1f, 2f);
	}

	public GameObject GetUp()
	{
		if (UIKeyNavigation.IsActive(this.onUp))
		{
			return this.onUp;
		}
		if (this.constraint == UIKeyNavigation.Constraint.Horizontal || this.constraint == UIKeyNavigation.Constraint.Explicit)
		{
			return null;
		}
		return this.Get(Vector3.up, 2f, 1f);
	}

	public GameObject GetDown()
	{
		if (UIKeyNavigation.IsActive(this.onDown))
		{
			return this.onDown;
		}
		if (this.constraint == UIKeyNavigation.Constraint.Horizontal || this.constraint == UIKeyNavigation.Constraint.Explicit)
		{
			return null;
		}
		return this.Get(Vector3.down, 2f, 1f);
	}

	public GameObject Get(Vector3 myDir, float x = 1f, float y = 1f)
	{
		Transform transform = base.transform;
		myDir = transform.TransformDirection(myDir);
		Vector3 center = UIKeyNavigation.GetCenter(base.gameObject);
		float num = 3.40282347E+38f;
		GameObject result = null;
		for (int i = 0; i < UIKeyNavigation.list.size; i++)
		{
			UIKeyNavigation uIKeyNavigation = UIKeyNavigation.list[i];
			if (!(uIKeyNavigation == this) && uIKeyNavigation.isColliderEnabled)
			{
				UIWidget component = uIKeyNavigation.GetComponent<UIWidget>();
				if (!(component != null) || component.alpha != 0f)
				{
					Vector3 direction = UIKeyNavigation.GetCenter(uIKeyNavigation.gameObject) - center;
					float num2 = Vector3.Dot(myDir, direction.normalized);
					if (num2 >= 0.707f)
					{
						direction = transform.InverseTransformDirection(direction);
						direction.x *= x;
						direction.y *= y;
						float sqrMagnitude = direction.sqrMagnitude;
						if (sqrMagnitude <= num)
						{
							result = uIKeyNavigation.gameObject;
							num = sqrMagnitude;
						}
					}
				}
			}
		}
		return result;
	}

	protected static Vector3 GetCenter(GameObject go)
	{
		UIWidget component = go.GetComponent<UIWidget>();
		UICamera uICamera = UICamera.FindCameraForLayer(go.layer);
		if (uICamera != null)
		{
			Vector3 vector = go.transform.position;
			if (component != null)
			{
				Vector3[] worldCorners = component.worldCorners;
				vector = (worldCorners[0] + worldCorners[2]) * 0.5f;
			}
			vector = uICamera.cachedCamera.WorldToScreenPoint(vector);
			vector.z = 0f;
			return vector;
		}
		if (component != null)
		{
			Vector3[] worldCorners2 = component.worldCorners;
			return (worldCorners2[0] + worldCorners2[2]) * 0.5f;
		}
		return go.transform.position;
	}

	public virtual void OnNavigate(KeyCode key)
	{
		if (UIPopupList.isOpen)
		{
			return;
		}
		GameObject gameObject = null;
		switch (key)
		{
		case KeyCode.UpArrow:
			gameObject = this.GetUp();
			break;
		case KeyCode.DownArrow:
			gameObject = this.GetDown();
			break;
		case KeyCode.RightArrow:
			gameObject = this.GetRight();
			break;
		case KeyCode.LeftArrow:
			gameObject = this.GetLeft();
			break;
		}
		if (gameObject != null)
		{
			UICamera.hoveredObject = gameObject;
		}
	}

	public virtual void OnKey(KeyCode key)
	{
		if (key == KeyCode.Tab)
		{
			GameObject gameObject = this.onTab;
			if (gameObject == null)
			{
				if (UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift))
				{
					gameObject = this.GetLeft();
					if (gameObject == null)
					{
						gameObject = this.GetUp();
					}
					if (gameObject == null)
					{
						gameObject = this.GetDown();
					}
					if (gameObject == null)
					{
						gameObject = this.GetRight();
					}
				}
				else
				{
					gameObject = this.GetRight();
					if (gameObject == null)
					{
						gameObject = this.GetDown();
					}
					if (gameObject == null)
					{
						gameObject = this.GetUp();
					}
					if (gameObject == null)
					{
						gameObject = this.GetLeft();
					}
				}
			}
			if (gameObject != null)
			{
				UICamera.selectedObject = gameObject;
			}
		}
	}

	protected virtual void OnClick()
	{
		if (NGUITools.GetActive(this.onClick))
		{
			UICamera.hoveredObject = this.onClick;
		}
	}
}
