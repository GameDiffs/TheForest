using Ceto.Common.Threading.Tasks;
using System;

namespace Ceto.Common.Threading.Scheduling
{
	public interface IScheduler
	{
		int ScheduledTasks();

		int RunningTasks();

		int WaitingTasks();

		int FinishingTasks();

		int Tasks();

		bool HasTasks();

		void Cancel(IThreadedTask task);

		bool Contains(IThreadedTask task);

		bool IsScheduled(IThreadedTask task);

		bool IsRunning(IThreadedTask task);

		bool IsWaiting(IThreadedTask task);

		bool IsFinishing(IThreadedTask task);

		void Add(IThreadedTask task);

		void Run(IThreadedTask task);

		void AddWaiting(IThreadedTask task);

		void StopWaiting(IThreadedTask task, bool run);
	}
}
