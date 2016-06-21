using System;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player
{
	public class rockThrowerAimingReticle : MonoBehaviour
	{
		public enum DisplayModes
		{
			Trajectory,
			Target,
			GroundTarget
		}

		public float _startOffsetRadius = 0.5f;

		public int _dots = 10;

		public int _accuracyFrames = 4;

		public Rigidbody _projectileRB;

		public rockThrowerAimingReticle.DisplayModes _displayMode;

		public LayerMask _targetLayers = -1;

		public LayerMask _groundLayers = -1;

		public Vector3 _startPositionOffset;

		public float _visibilityDelayAfterThrow = 0.6f;

		public float _projectileThrowForceRange = 10f;

		public Vector3 _currentLandTarget;

		private float _nextVisibleTime;

		private void Awake()
		{
			if (!base.enabled)
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		private void OnEnable()
		{
			if (Scene.HudGui)
			{
				if (this._displayMode == rockThrowerAimingReticle.DisplayModes.Trajectory)
				{
					Scene.HudGui.RangedWeaponTrajectory.enabled = PlayerPreferences.ShowProjectileReticle;
					Scene.HudGui.RangedWeaponTrajectory.SetVertexCount(this._dots);
				}
				Scene.HudGui.RangedWeaponHitTarget.SetActive(false);
				Scene.HudGui.RangedWeaponHitGroundTarget.SendMessage("doDisableGo", SendMessageOptions.DontRequireReceiver);
				Scene.HudGui.RangedWeaponHitGroundTarget.SetActive(false);
			}
		}

		private void OnDisable()
		{
			if (Scene.HudGui)
			{
				Scene.HudGui.RangedWeaponTrajectory.enabled = false;
				Scene.HudGui.RangedWeaponHitTarget.SetActive(false);
				Scene.HudGui.RangedWeaponHitGroundTarget.SendMessage("doDisableGo", SendMessageOptions.DontRequireReceiver);
				Scene.HudGui.RangedWeaponHitGroundTarget.SetActive(false);
			}
		}

		private void Update()
		{
			Scene.HudGui.RangedWeaponHitTarget.SetActive(false);
			Scene.HudGui.RangedWeaponHitGroundTarget.SetActive(false);
			this.ShowTrajectory(this._projectileThrowForceRange * LocalPlayer.MainCamTr.forward * Time.fixedDeltaTime);
			if (PlayerPreferences.ShowProjectileReticle && this._displayMode == rockThrowerAimingReticle.DisplayModes.Trajectory)
			{
				Scene.HudGui.RangedWeaponTrajectory.enabled = true;
			}
		}

		private void ShowTrajectory(Vector3 force)
		{
			Vector3 a = force / this._projectileRB.mass;
			Vector3 b = Physics.gravity * Time.fixedDeltaTime;
			Vector3 vector = base.transform.position + base.transform.TransformVector(this._startPositionOffset);
			Vector3 vector2 = vector;
			while ((vector - vector2).sqrMagnitude < this._startOffsetRadius)
			{
				a += b;
				if (this._projectileRB.drag > 0f)
				{
					a += a.magnitude * this._projectileRB.drag * Time.fixedDeltaTime * -a.normalized;
				}
				vector += a * Time.fixedDeltaTime;
			}
			vector2 = vector;
			for (int i = 0; i < this._dots; i++)
			{
				if (this._displayMode == rockThrowerAimingReticle.DisplayModes.Trajectory)
				{
					Scene.HudGui.RangedWeaponTrajectory.SetPosition(i, vector);
				}
				for (int j = 0; j < this._accuracyFrames; j++)
				{
					a += b;
					if (this._projectileRB.drag > 0f)
					{
						a += a.magnitude * this._projectileRB.drag * Time.fixedDeltaTime * -a.normalized;
					}
					vector += a * Time.fixedDeltaTime;
				}
				if (this._displayMode == rockThrowerAimingReticle.DisplayModes.Target || this._displayMode == rockThrowerAimingReticle.DisplayModes.GroundTarget)
				{
					RaycastHit raycastHit;
					if (Physics.Raycast(vector2, vector - vector2, out raycastHit, Vector3.Distance(vector, vector2), this._targetLayers))
					{
						this.ShowHitTargetAt(raycastHit.point);
						this._currentLandTarget = raycastHit.point;
						break;
					}
					if (i + 1 == this._dots)
					{
						this.ShowHitTargetAt(vector2);
						this._currentLandTarget = vector2;
					}
				}
				vector2 = vector;
			}
		}

		private void ShowHitTargetAt(Vector3 position)
		{
			if (this._displayMode == rockThrowerAimingReticle.DisplayModes.Target)
			{
				Vector3 position2 = LocalPlayer.MainCam.WorldToViewportPoint(position);
				if (position2.z < 2f)
				{
					position2.z = 2f;
				}
				else if (position2.z > 20f)
				{
					position2.z = 20f;
				}
				Scene.HudGui.RangedWeaponHitTarget.transform.position = Scene.HudGui.ActionIconCams.ViewportToWorldPoint(position2);
				Scene.HudGui.RangedWeaponHitTarget.SetActive(true);
			}
			else if (this._displayMode == rockThrowerAimingReticle.DisplayModes.GroundTarget)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(position + Vector3.up, Vector3.down, out raycastHit, 100f, this._groundLayers))
				{
					float t = Mathf.Lerp(0.01f, 0.4f, Vector3.Distance(Scene.HudGui.RangedWeaponHitGroundTarget.transform.position, raycastHit.point) / 2f);
					Scene.HudGui.RangedWeaponHitGroundTarget.transform.position = Vector3.Lerp(Scene.HudGui.RangedWeaponHitGroundTarget.transform.position, raycastHit.point + new Vector3(0f, 0.2f, 0f), t);
					Scene.HudGui.RangedWeaponHitGroundTarget.transform.LookAt(Scene.HudGui.RangedWeaponHitGroundTarget.transform.position + raycastHit.normal);
				}
				else
				{
					float t2 = Mathf.Lerp(0.01f, 0.4f, Vector3.Distance(Scene.HudGui.RangedWeaponHitGroundTarget.transform.position, raycastHit.point) / 2f);
					Scene.HudGui.RangedWeaponHitGroundTarget.transform.position = Vector3.Lerp(Scene.HudGui.RangedWeaponHitGroundTarget.transform.position, position, t2);
					Scene.HudGui.RangedWeaponHitGroundTarget.transform.LookAt(Vector3.up);
				}
				Scene.HudGui.RangedWeaponHitGroundTarget.SetActive(true);
				Scene.HudGui.RangedWeaponHitGroundTarget.SendMessage("doEnableGo", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
