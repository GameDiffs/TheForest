using System;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Property Binding"), ExecuteInEditMode]
public class PropertyBinding : MonoBehaviour
{
	public enum UpdateCondition
	{
		OnStart,
		OnUpdate,
		OnLateUpdate,
		OnFixedUpdate
	}

	public enum Direction
	{
		SourceUpdatesTarget,
		TargetUpdatesSource,
		BiDirectional
	}

	public PropertyReference source;

	public PropertyReference target;

	public PropertyBinding.Direction direction;

	public PropertyBinding.UpdateCondition update = PropertyBinding.UpdateCondition.OnUpdate;

	public bool editMode = true;

	private object mLastValue;

	private void Start()
	{
		this.UpdateTarget();
		if (this.update == PropertyBinding.UpdateCondition.OnStart)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (this.update == PropertyBinding.UpdateCondition.OnUpdate)
		{
			this.UpdateTarget();
		}
	}

	private void LateUpdate()
	{
		if (this.update == PropertyBinding.UpdateCondition.OnLateUpdate)
		{
			this.UpdateTarget();
		}
	}

	private void FixedUpdate()
	{
		if (this.update == PropertyBinding.UpdateCondition.OnFixedUpdate)
		{
			this.UpdateTarget();
		}
	}

	private void OnValidate()
	{
		if (this.source != null)
		{
			this.source.Reset();
		}
		if (this.target != null)
		{
			this.target.Reset();
		}
	}

	[ContextMenu("Update Now")]
	public void UpdateTarget()
	{
		if (this.source != null && this.target != null && this.source.isValid && this.target.isValid)
		{
			if (this.direction == PropertyBinding.Direction.SourceUpdatesTarget)
			{
				this.target.Set(this.source.Get());
			}
			else if (this.direction == PropertyBinding.Direction.TargetUpdatesSource)
			{
				this.source.Set(this.target.Get());
			}
			else if (this.source.GetPropertyType() == this.target.GetPropertyType())
			{
				object obj = this.source.Get();
				if (this.mLastValue == null || !this.mLastValue.Equals(obj))
				{
					this.mLastValue = obj;
					this.target.Set(obj);
				}
				else
				{
					obj = this.target.Get();
					if (!this.mLastValue.Equals(obj))
					{
						this.mLastValue = obj;
						this.source.Set(obj);
					}
				}
			}
		}
	}
}
