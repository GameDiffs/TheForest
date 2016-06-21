using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Drag and Drop Item")]
public class UIDragDropItem : MonoBehaviour
{
	public enum Restriction
	{
		None,
		Horizontal,
		Vertical,
		PressAndHold
	}

	public UIDragDropItem.Restriction restriction;

	public bool cloneOnDrag;

	[HideInInspector]
	public float pressAndHoldDelay = 1f;

	public bool interactable = true;

	[NonSerialized]
	protected Transform mTrans;

	[NonSerialized]
	protected Transform mParent;

	[NonSerialized]
	protected Collider mCollider;

	[NonSerialized]
	protected Collider2D mCollider2D;

	[NonSerialized]
	protected UIButton mButton;

	[NonSerialized]
	protected UIRoot mRoot;

	[NonSerialized]
	protected UIGrid mGrid;

	[NonSerialized]
	protected UITable mTable;

	[NonSerialized]
	protected float mDragStartTime;

	[NonSerialized]
	protected UIDragScrollView mDragScrollView;

	[NonSerialized]
	protected bool mPressed;

	[NonSerialized]
	protected bool mDragging;

	[NonSerialized]
	protected UICamera.MouseOrTouch mTouch;

	public static List<UIDragDropItem> draggedItems = new List<UIDragDropItem>();

	protected virtual void Awake()
	{
		this.mTrans = base.transform;
		this.mCollider = base.gameObject.GetComponent<Collider>();
		this.mCollider2D = base.gameObject.GetComponent<Collider2D>();
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
		if (this.mDragging)
		{
			this.StopDragging(UICamera.hoveredObject);
		}
	}

	protected virtual void Start()
	{
		this.mButton = base.GetComponent<UIButton>();
		this.mDragScrollView = base.GetComponent<UIDragScrollView>();
	}

	protected virtual void OnPress(bool isPressed)
	{
		if (!this.interactable || UICamera.currentTouchID == -2 || UICamera.currentTouchID == -3)
		{
			return;
		}
		if (isPressed)
		{
			if (!this.mPressed)
			{
				this.mTouch = UICamera.currentTouch;
				this.mDragStartTime = RealTime.time + this.pressAndHoldDelay;
				this.mPressed = true;
			}
		}
		else if (this.mPressed && this.mTouch == UICamera.currentTouch)
		{
			this.mPressed = false;
			this.mTouch = null;
		}
	}

	protected virtual void Update()
	{
		if (this.restriction == UIDragDropItem.Restriction.PressAndHold && this.mPressed && !this.mDragging && this.mDragStartTime < RealTime.time)
		{
			this.StartDragging();
		}
	}

	protected virtual void OnDragStart()
	{
		if (!this.interactable)
		{
			return;
		}
		if (!base.enabled || this.mTouch != UICamera.currentTouch)
		{
			return;
		}
		if (this.restriction != UIDragDropItem.Restriction.None)
		{
			if (this.restriction == UIDragDropItem.Restriction.Horizontal)
			{
				Vector2 totalDelta = this.mTouch.totalDelta;
				if (Mathf.Abs(totalDelta.x) < Mathf.Abs(totalDelta.y))
				{
					return;
				}
			}
			else if (this.restriction == UIDragDropItem.Restriction.Vertical)
			{
				Vector2 totalDelta2 = this.mTouch.totalDelta;
				if (Mathf.Abs(totalDelta2.x) > Mathf.Abs(totalDelta2.y))
				{
					return;
				}
			}
			else if (this.restriction == UIDragDropItem.Restriction.PressAndHold)
			{
				return;
			}
		}
		this.StartDragging();
	}

	public virtual void StartDragging()
	{
		if (!this.interactable)
		{
			return;
		}
		if (!this.mDragging)
		{
			if (this.cloneOnDrag)
			{
				this.mPressed = false;
				GameObject gameObject = NGUITools.AddChild(base.transform.parent.gameObject, base.gameObject);
				gameObject.transform.localPosition = base.transform.localPosition;
				gameObject.transform.localRotation = base.transform.localRotation;
				gameObject.transform.localScale = base.transform.localScale;
				UIButtonColor component = gameObject.GetComponent<UIButtonColor>();
				if (component != null)
				{
					component.defaultColor = base.GetComponent<UIButtonColor>().defaultColor;
				}
				if (this.mTouch != null && this.mTouch.pressed == base.gameObject)
				{
					this.mTouch.current = gameObject;
					this.mTouch.pressed = gameObject;
					this.mTouch.dragged = gameObject;
					this.mTouch.last = gameObject;
				}
				UIDragDropItem component2 = gameObject.GetComponent<UIDragDropItem>();
				component2.mTouch = this.mTouch;
				component2.mPressed = true;
				component2.mDragging = true;
				component2.Start();
				component2.OnDragDropStart();
				if (UICamera.currentTouch == null)
				{
					UICamera.currentTouch = this.mTouch;
				}
				this.mTouch = null;
				UICamera.Notify(base.gameObject, "OnPress", false);
				UICamera.Notify(base.gameObject, "OnHover", false);
			}
			else
			{
				this.mDragging = true;
				this.OnDragDropStart();
			}
		}
	}

	protected virtual void OnDrag(Vector2 delta)
	{
		if (!this.interactable)
		{
			return;
		}
		if (!this.mDragging || !base.enabled || this.mTouch != UICamera.currentTouch)
		{
			return;
		}
		this.OnDragDropMove(delta * this.mRoot.pixelSizeAdjustment);
	}

	protected virtual void OnDragEnd()
	{
		if (!this.interactable)
		{
			return;
		}
		if (!base.enabled || this.mTouch != UICamera.currentTouch)
		{
			return;
		}
		this.StopDragging(UICamera.hoveredObject);
	}

	public void StopDragging(GameObject go)
	{
		if (this.mDragging)
		{
			this.mDragging = false;
			this.OnDragDropRelease(go);
		}
	}

	protected virtual void OnDragDropStart()
	{
		if (!UIDragDropItem.draggedItems.Contains(this))
		{
			UIDragDropItem.draggedItems.Add(this);
		}
		if (this.mDragScrollView != null)
		{
			this.mDragScrollView.enabled = false;
		}
		if (this.mButton != null)
		{
			this.mButton.isEnabled = false;
		}
		else if (this.mCollider != null)
		{
			this.mCollider.enabled = false;
		}
		else if (this.mCollider2D != null)
		{
			this.mCollider2D.enabled = false;
		}
		this.mParent = this.mTrans.parent;
		this.mRoot = NGUITools.FindInParents<UIRoot>(this.mParent);
		this.mGrid = NGUITools.FindInParents<UIGrid>(this.mParent);
		this.mTable = NGUITools.FindInParents<UITable>(this.mParent);
		if (UIDragDropRoot.root != null)
		{
			this.mTrans.parent = UIDragDropRoot.root;
		}
		Vector3 localPosition = this.mTrans.localPosition;
		localPosition.z = 0f;
		this.mTrans.localPosition = localPosition;
		TweenPosition component = base.GetComponent<TweenPosition>();
		if (component != null)
		{
			component.enabled = false;
		}
		SpringPosition component2 = base.GetComponent<SpringPosition>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
		NGUITools.MarkParentAsChanged(base.gameObject);
		if (this.mTable != null)
		{
			this.mTable.repositionNow = true;
		}
		if (this.mGrid != null)
		{
			this.mGrid.repositionNow = true;
		}
	}

	protected virtual void OnDragDropMove(Vector2 delta)
	{
		this.mTrans.localPosition += delta;
	}

	protected virtual void OnDragDropRelease(GameObject surface)
	{
		if (!this.cloneOnDrag)
		{
			if (this.mButton != null)
			{
				this.mButton.isEnabled = true;
			}
			else if (this.mCollider != null)
			{
				this.mCollider.enabled = true;
			}
			else if (this.mCollider2D != null)
			{
				this.mCollider2D.enabled = true;
			}
			UIDragDropContainer uIDragDropContainer = (!surface) ? null : NGUITools.FindInParents<UIDragDropContainer>(surface);
			if (uIDragDropContainer != null)
			{
				this.mTrans.parent = ((!(uIDragDropContainer.reparentTarget != null)) ? uIDragDropContainer.transform : uIDragDropContainer.reparentTarget);
				Vector3 localPosition = this.mTrans.localPosition;
				localPosition.z = 0f;
				this.mTrans.localPosition = localPosition;
			}
			else
			{
				this.mTrans.parent = this.mParent;
			}
			this.mParent = this.mTrans.parent;
			this.mGrid = NGUITools.FindInParents<UIGrid>(this.mParent);
			this.mTable = NGUITools.FindInParents<UITable>(this.mParent);
			if (this.mDragScrollView != null)
			{
				base.StartCoroutine(this.EnableDragScrollView());
			}
			NGUITools.MarkParentAsChanged(base.gameObject);
			if (this.mTable != null)
			{
				this.mTable.repositionNow = true;
			}
			if (this.mGrid != null)
			{
				this.mGrid.repositionNow = true;
			}
			this.OnDragDropEnd();
		}
		else
		{
			NGUITools.Destroy(base.gameObject);
		}
	}

	protected virtual void OnDragDropEnd()
	{
		UIDragDropItem.draggedItems.Remove(this);
	}

	[DebuggerHidden]
	protected IEnumerator EnableDragScrollView()
	{
		UIDragDropItem.<EnableDragScrollView>c__Iterator109 <EnableDragScrollView>c__Iterator = new UIDragDropItem.<EnableDragScrollView>c__Iterator109();
		<EnableDragScrollView>c__Iterator.<>f__this = this;
		return <EnableDragScrollView>c__Iterator;
	}
}
