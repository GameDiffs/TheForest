using System;
using TheForest.Buildings.Creation;

namespace TheForest.Buildings.Interfaces
{
	public interface IAnchorableStructureX2 : IAnchorableStructure
	{
		StructureAnchor Anchor2
		{
			get;
			set;
		}
	}
}
