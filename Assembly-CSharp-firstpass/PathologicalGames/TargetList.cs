using System;
using System.Collections.Generic;

namespace PathologicalGames
{
	public class TargetList : List<Target>
	{
		public override string ToString()
		{
			string[] names = new string[base.Count];
			int i = 0;
			base.ForEach(delegate(Target target)
			{
				if (target.transform == null)
				{
					return;
				}
				names[i] = target.transform.name;
				i++;
			});
			return string.Join(", ", names);
		}
	}
}
