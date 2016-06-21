using System;
using TheForest.Buildings.World;
using TheForest.Items.World;
using UnityEngine;

[DoNotSerializePublic]
public class rockThrowerAnimEvents : MonoBehaviour
{
	public BoltEntity entity;

	public MultiThrowerItemHolder itemHolder;

	public MultiThrowerProjectile projectilePrefab;

	public Transform throwPos;

	public Transform releasePos;

	public Vector3 landTarget;

	[SerializeThis]
	public int ammoCount;

	public GameObject[] rockAmmo;

	private void throwRocks()
	{
		if (BoltNetwork.isClient)
		{
			return;
		}
		for (int i = 0; i < this.ammoCount; i++)
		{
			Transform child = this.rockAmmo[i].transform.GetChild(0);
			Vector3 position = this.releasePos.position + this.releasePos.up * 2f + (this.releasePos.forward * UnityEngine.Random.Range(-0.6f, 0.6f) + this.releasePos.right * UnityEngine.Random.Range(-0.6f, 0.6f));
			MultiThrowerProjectile multiThrowerProjectile = (MultiThrowerProjectile)UnityEngine.Object.Instantiate(this.projectilePrefab, position, Quaternion.identity);
			Rigidbody rigidbody = multiThrowerProjectile._rigidbody;
			Vector2 vector = this.randomCircle2(2.2f);
			Vector3 target = new Vector3(this.landTarget.x + vector.x, this.landTarget.y, this.landTarget.z + vector.y);
			Vector3 force = this.calculateBestThrowSpeed(this.releasePos.position, target, UnityEngine.Random.Range(2.4f, 2.6f));
			rigidbody.AddForce(force, ForceMode.VelocityChange);
			rigidbody.AddTorque(base.transform.right * UnityEngine.Random.Range(20f, 35f), ForceMode.VelocityChange);
			multiThrowerProjectile.InitProjectile(this.itemHolder.AmmoLoaded[i], child);
			if (BoltNetwork.isRunning)
			{
				BoltEntity component = multiThrowerProjectile.GetComponent<BoltEntity>();
				if (component)
				{
					BoltNetwork.Attach(multiThrowerProjectile.gameObject);
					IMultiThrowerProjectileState state = component.GetState<IMultiThrowerProjectileState>();
					state.Thrower = this.entity;
					state.AmmoId = i + 1;
				}
			}
		}
		GameObject[] array = this.rockAmmo;
		for (int j = 0; j < array.Length; j++)
		{
			GameObject gameObject = array[j];
			gameObject.SetActive(false);
		}
	}

	private Vector3 calculateBestThrowSpeed(Vector3 origin, Vector3 target, float timeToTarget)
	{
		Vector3 vector = target - origin;
		Vector3 vector2 = vector;
		vector2.y = 0f;
		float y = vector.y;
		float magnitude = vector2.magnitude;
		float y2 = y / timeToTarget + 0.5f * Physics.gravity.magnitude * timeToTarget;
		float d = magnitude / timeToTarget;
		Vector3 vector3 = vector2.normalized;
		vector3 *= d;
		vector3.y = y2;
		return vector3;
	}

	private Vector2 randomCircle2(float radius)
	{
		Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
		return normalized * radius;
	}

	private void OnDeserialized()
	{
		for (int i = 0; i < this.ammoCount; i++)
		{
			this.rockAmmo[i].SetActive(true);
		}
	}
}
