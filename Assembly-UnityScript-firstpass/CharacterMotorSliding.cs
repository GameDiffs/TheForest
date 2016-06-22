using System;

[Serializable]
public class CharacterMotorSliding
{
	public bool enabled;

	public float slidingSpeed;

	public float sidewaysControl;

	public float speedControl;

	public CharacterMotorSliding()
	{
		this.enabled = true;
		this.slidingSpeed = (float)15;
		this.sidewaysControl = 1f;
		this.speedControl = 0.4f;
	}
}
