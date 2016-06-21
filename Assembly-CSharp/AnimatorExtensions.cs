using System;
using UnityEngine;

public static class AnimatorExtensions
{
	public static void CopyParamsFrom(this Animator self, Animator from)
	{
		AnimatorControllerData.ControllerData data = AnimatorControllerData.Instance.GetData(from.runtimeAnimatorController.name.GetHashCode());
		AnimatorControllerData.Parameter[] parameters = data.Parameters;
		for (int i = 0; i < parameters.Length; i++)
		{
			AnimatorControllerData.Parameter parameter = parameters[i];
			switch (parameter.ParamType)
			{
			case AnimatorControllerData.ParamType.Bool:
				self.SetBoolReflected(parameter.Name, from.GetBool(parameter.Name));
				break;
			case AnimatorControllerData.ParamType.Int:
				self.SetIntegerReflected(parameter.Name, from.GetInteger(parameter.Name));
				break;
			case AnimatorControllerData.ParamType.Float:
				self.SetFloatReflected(parameter.Name, from.GetFloat(parameter.Name));
				break;
			}
		}
	}
}
