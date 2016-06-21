using Ceto.Common.Threading.Scheduling;
using System;
using System.Collections;

namespace Ceto.Common.Threading.Tasks
{
	public interface IThreadedTask
	{
		float RunTime
		{
			get;
			set;
		}

		bool IsThreaded
		{
			get;
		}

		bool Ran
		{
			get;
		}

		bool Done
		{
			get;
		}

		bool NoFinish
		{
			get;
			set;
		}

		bool Waiting
		{
			get;
		}

		bool RunOnStopWaiting
		{
			get;
			set;
		}

		bool Started
		{
			get;
		}

		bool Cancelled
		{
			get;
		}

		IScheduler Scheduler
		{
			set;
		}

		void Reset();

		void Start();

		IEnumerator Run();

		void End();

		void Cancel();

		void WaitOn(ThreadedTask task);
	}
}
