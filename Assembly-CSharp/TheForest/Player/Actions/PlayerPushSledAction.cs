using HutongGames.PlayMaker;
using System;
using TheForest.Buildings.World;
using TheForest.Items.Inventory;
using TheForest.Utils;
using UnityEngine;

namespace TheForest.Player.Actions
{
	public class PlayerPushSledAction : MonoBehaviour
	{
		public Transform sledCollider;

		public PlayerInventory _player;

		public float ropeAttachOffset;

		public float ropeAttachTopOffsetZ;

		public float ropeAttachTopOffsetX;

		public bool doingAction;

		private bool fullyAttached;

		public Transform currentSled;

		public Transform currentSledRoot;

		private FsmBool fsmPushSledBool;

		private simpleIkSolver ik;

		private MonoBehaviour holderScript;

		private SphereCollider holderCollider;

		private lookAtTerrain lookAtScript;

		private Vector3 initLookAtPos;

		private float initLookatRotX;

		private float prevMouseXSpeed;

		private int layerMask;

		private void Awake()
		{
			this.lookAtScript = this._player.GetComponentInChildren<lookAtTerrain>();
			this.initLookAtPos = this.lookAtScript.transform.localPosition;
			this.initLookatRotX = this.lookAtScript.transform.localEulerAngles.x;
			this.layerMask = 103948288;
		}

		private void enterPushSled(Transform trn)
		{
			this.fsmPushSledBool = this._player.PM.FsmVariables.GetFsmBool("pushSledBool");
			if (!this.fsmPushSledBool.Value)
			{
				LocalPlayer.Animator.SetBoolReflected("jumpBool", false);
				this.currentSled = trn;
				this.currentSledRoot = trn.root;
				this.holderScript = this.currentSled.root.GetComponentInChildren<MultiHolder>();
				if (!this.holderScript)
				{
					this.holderScript = this.currentSled.root.GetComponentInChildren<LogHolder>();
				}
				this.currentSledRoot.GetComponent<Rigidbody>().useGravity = false;
				this.currentSledRoot.gameObject.layer = 29;
				LocalPlayer.Animator.SetTriggerReflected("resetTrigger");
				LocalPlayer.FpCharacter.OnSled();
				LocalPlayer.FpCharacter.Locked = true;
				LocalPlayer.GameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
				LocalPlayer.MainRotator.lockRotation = true;
				this.prevMouseXSpeed = LocalPlayer.MainRotator.rotationSpeed;
				LocalPlayer.MainRotator.rotationSpeed = 0f;
				LocalPlayer.CamRotator.rotationRange = new Vector2(110f, 130f);
				this._player.transform.rotation = trn.rotation;
				this._player.transform.localEulerAngles = new Vector3(0f, this._player.transform.localEulerAngles.y, 0f);
				this.currentSled.root.rotation = this._player.transform.rotation;
				Vector3 position = this.currentSledRoot.transform.position;
				position.y += 5f;
				RaycastHit raycastHit;
				if (Physics.Raycast(position, Vector3.down, out raycastHit, 20f, this.layerMask))
				{
					this.currentSledRoot.rotation = Quaternion.LookRotation(Vector3.Cross(this.currentSledRoot.right, raycastHit.normal), raycastHit.normal);
				}
				Vector3 vector = this.currentSled.position - this.currentSled.forward * this.ropeAttachOffset;
				float playerTerrainPos = this.getPlayerTerrainPos(vector);
				vector.y = playerTerrainPos;
				this._player.transform.position = vector;
				this._player.HideAllEquiped(false);
				base.Invoke("connectRigidBody", 1f);
				this._player.PM.FsmVariables.GetFsmGameObject("actionGo").Value = trn.gameObject;
				this.currentSled.gameObject.SetActive(false);
				this.fsmPushSledBool.Value = true;
				this._player.transform.position = new Vector3(this._player.transform.position.x, playerTerrainPos, this._player.transform.position.z);
				this.currentSledRoot.gameObject.layer = 29;
				this.doingAction = true;
			}
		}

		public void forceExitSled()
		{
			if (this.currentSled && this.fullyAttached)
			{
				this.currentSled.SendMessage("disableSled", SendMessageOptions.DontRequireReceiver);
				this.fullyAttached = false;
			}
		}

		private void exitPushSled()
		{
			if (this.doingAction)
			{
				this.currentSledRoot.parent = null;
				float y = LocalPlayer.Transform.localEulerAngles.y;
				this._player.PM.SendEvent("toExitPushSled");
				this._player.ShowAllEquiped();
				LocalPlayer.MainRotator.resetOriginalRotation = true;
				LocalPlayer.MainRotator.lockRotation = false;
				LocalPlayer.MainRotator.rotationSpeed = this.prevMouseXSpeed;
				LocalPlayer.MainRotator.rotationRange = new Vector2(0f, 999f);
				LocalPlayer.CamRotator.rotationRange = new Vector2(170f, 0f);
				LocalPlayer.AnimControl.sledHinge.connectedBody = null;
				LocalPlayer.FpCharacter.OffSled();
				LocalPlayer.FpCharacter.CanJump = true;
				LocalPlayer.FpCharacter.Locked = false;
				LocalPlayer.AnimControl.exitPushMode();
				this.currentSled.SendMessage("resetTrigger");
				Vector3 position = this.currentSledRoot.transform.position;
				position.y += 5f;
				RaycastHit raycastHit;
				if (Physics.Raycast(position, Vector3.down, out raycastHit, 20f, this.layerMask))
				{
					float y2 = raycastHit.point.y + 0.3f;
					Vector3 position2 = new Vector3(this.currentSledRoot.transform.position.x, y2, this.currentSledRoot.transform.position.z);
					this.currentSledRoot.transform.position = position2;
					this.currentSledRoot.rotation = Quaternion.LookRotation(Vector3.Cross(this.currentSledRoot.right, raycastHit.normal), raycastHit.normal);
				}
				this.currentSledRoot.gameObject.layer = 0;
				Rigidbody rigidbody = this.currentSledRoot.gameObject.AddComponent<Rigidbody>();
				if (!rigidbody)
				{
					rigidbody = this.currentSledRoot.gameObject.GetComponent<Rigidbody>();
				}
				rigidbody.mass = 50f;
				this.lookAtScript.transform.parent = this._player.transform;
				this.lookAtScript.transform.localPosition = this.initLookAtPos;
				this.lookAtScript.transform.localEulerAngles = new Vector3(this.initLookatRotX, 0f, 0f);
				this.lookAtScript.resetSledCollider();
				this.lookAtScript.enabled = false;
				this.doingAction = false;
				this.fullyAttached = false;
				this._player.PM.FsmVariables.GetFsmBool("pushSledBool").Value = false;
			}
		}

		public void enableSledTrigger()
		{
			this.currentSled.gameObject.SetActive(true);
		}

		private void connectRigidBody()
		{
			this.currentSled.root.rotation = this._player.transform.rotation;
			Vector3 vector = this.currentSled.position - this.currentSled.forward * this.ropeAttachOffset;
			vector.y = this._player.transform.position.y;
			this._player.PM.FsmVariables.GetFsmVector3("attachPos").Value = vector;
			this._player.transform.position = vector;
			UnityEngine.Object.Destroy(this.currentSled.root.GetComponent<Rigidbody>());
			this.lookAtScript.transform.parent = LocalPlayer.AnimControl.sledHinge.transform.parent.transform;
			this.lookAtScript.transform.localPosition = new Vector3(0f, -0.41f, 0.912f);
			this.currentSledRoot.parent = this.lookAtScript.transform;
			this.currentSledRoot.transform.localPosition = new Vector3(-1.32f, -1.3f, 3.155f);
			this.currentSledRoot.transform.localEulerAngles = new Vector3(-26.5f, -0.29f, -0.51f);
			this.lookAtScript.setSledCollider(this.currentSledRoot.GetComponent<BoxCollider>());
			this.lookAtScript.enabled = true;
			LocalPlayer.FpCharacter.CanJump = false;
			LocalPlayer.FpCharacter.enabled = true;
			LocalPlayer.FpCharacter.Locked = false;
			float playerTerrainPos = this.getPlayerTerrainPos(LocalPlayer.Transform.position);
			this._player.transform.position = new Vector3(this._player.transform.position.x, playerTerrainPos, this._player.transform.position.z);
			LocalPlayer.AnimControl.enterPushMode();
			this.fullyAttached = true;
			base.Invoke("fixSledPosition", 0.25f);
		}

		private void fixSledPosition()
		{
			if (this.doingAction)
			{
				this.currentSledRoot.transform.localPosition = new Vector3(-1.32f, -1.3f, 3.155f);
			}
		}

		private float getPlayerTerrainPos(Vector3 pos)
		{
			Vector3 origin = pos;
			origin.y += 5f;
			RaycastHit raycastHit;
			if (Physics.Raycast(origin, Vector3.down, out raycastHit, 20f, this.layerMask))
			{
				return raycastHit.point.y + LocalPlayer.AnimControl.playerCollider.height / 2f;
			}
			return LocalPlayer.Transform.position.y;
		}

		private void forceDisableSled()
		{
			if (this.currentSled)
			{
				this.currentSled.SendMessage("disableSled", SendMessageOptions.DontRequireReceiver);
			}
			base.CancelInvoke("connectRigidBody");
			base.CancelInvoke("fixSledPosition");
			this._player.PM.FsmVariables.GetFsmBool("pushSledBool").Value = false;
			LocalPlayer.Animator.SetBoolReflected("pushSledBool", false);
		}
	}
}
