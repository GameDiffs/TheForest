using Bolt;
using System;

internal class CoopEnemyHealthProxy : EntityBehaviour
{
	private int hitDir;

	private void takeDamage(int direction)
	{
		this.hitDir = direction;
	}

	private void Hit(int damage)
	{
	}
}
