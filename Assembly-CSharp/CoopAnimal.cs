using System;
using UnityEngine;

public class CoopAnimal : CoopBase<IAnimalState>
{
	[SerializeField]
	public Transform rotationTransform;

	[SerializeField]
	public Animator _animator;
}
