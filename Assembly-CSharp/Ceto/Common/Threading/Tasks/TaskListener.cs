using System;

namespace Ceto.Common.Threading.Tasks
{
	public class TaskListener
	{
		private ThreadedTask m_task;

		private volatile int m_waiting;

		public ThreadedTask ListeningTask
		{
			get
			{
				return this.m_task;
			}
		}

		public int Waiting
		{
			get
			{
				return this.m_waiting;
			}
			set
			{
				this.m_waiting = value;
			}
		}

		public TaskListener(ThreadedTask task)
		{
			this.m_task = task;
		}

		public void OnFinish()
		{
			this.m_waiting--;
			if (this.m_waiting == 0 && !this.m_task.Cancelled)
			{
				this.m_task.StopWaiting();
			}
		}
	}
}
