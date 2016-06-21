using System;

namespace TheForest.Buildings.Interfaces
{
	public interface IRepairableStructure
	{
		int RepairLogs
		{
			get;
		}

		int CollapsedLogs
		{
			get;
		}

		int RepairMaterial
		{
			get;
		}

		int CalcMissingRepairLogs();

		int CalcMissingRepairMaterial();

		int CalcTotalRepairMaterial();

		void AddRepairMaterial(bool isLog);
	}
}
