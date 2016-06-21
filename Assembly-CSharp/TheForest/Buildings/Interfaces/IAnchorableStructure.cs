using System;
using TheForest.Buildings.Creation;

namespace TheForest.Buildings.Interfaces
{
	public interface IAnchorableStructure
	{
		StructureAnchor Anchor1
		{
			get;
			set;
		}

		void AnchorDestroyed(StructureAnchor anchor);
	}
}
