using System;
using TheForest.Utils;
using UnityEngine;

public class nudgeTree : MonoBehaviour
{
	private Rigidbody rb;

	public float nudgeForce;

	private void Start()
	{
		this.rb = base.transform.GetComponent<Rigidbody>();
		Vector3 centerOfMass = this.rb.centerOfMass;
		centerOfMass.y += 1f;
		this.rb.centerOfMass = centerOfMass;
		this.addPerpForce();
	}

	private void addRandomForce()
	{
		Vector3 position = base.transform.position;
		Vector2 vector = this.Circle(this.nudgeForce);
		Vector3 force = new Vector3(vector.x, 0f, vector.y);
		position.y += 15f;
		this.rb.AddForceAtPosition(force, position);
	}

	private void addPerpForce()
	{
		Vector3 force = LocalPlayer.Transform.forward * this.nudgeForce;
		if (BoltNetwork.isRunning)
		{
			GameObject gameObject = null;
			float num = float.PositiveInfinity;
			foreach (GameObject current in Scene.SceneTracker.allPlayers)
			{
				if (current)
				{
					float sqrMagnitude = (base.transform.position - current.transform.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						gameObject = current;
						num = sqrMagnitude;
					}
				}
			}
			if (gameObject)
			{
				force = gameObject.transform.forward * this.nudgeForce;
			}
		}
		Vector3 position = base.transform.position;
		position.y += 15f;
		this.rb.AddForceAtPosition(force, position);
	}

	private Vector2 Circle(float radius)
	{
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return insideUnitCircle * radius;
	}

	private void OnDestroy()
	{
		this.rb = null;
	}
}
