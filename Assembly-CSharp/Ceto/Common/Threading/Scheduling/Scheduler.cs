using Ceto.Common.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Ceto.Common.Threading.Scheduling
{
	public class Scheduler : IScheduler
	{
		private LinkedList<IThreadedTask> m_scheduledTasks;

		private LinkedList<IThreadedTask> m_finishedTasks;

		private LinkedList<IThreadedTask> m_runningTasks;

		private LinkedList<IThreadedTask> m_waitingTasks;

		private ICoroutine m_coroutine;

		private Exception m_exception;

		private readonly object m_lock = new object();

		private LinkedList<IThreadedTask> m_haveRan;

		private volatile bool m_shutingDown;

		public int TasksRanThisUpdate
		{
			get;
			private set;
		}

		public int TasksFinishedThisUpdate
		{
			get;
			private set;
		}

		public int MaxTasksPerUpdate
		{
			get;
			private set;
		}

		public int MaxFinishPerUpdate
		{
			get;
			private set;
		}

		public float MaxWaitTime
		{
			get;
			set;
		}

		public float MinWaitTime
		{
			get;
			set;
		}

		public bool DisableMultithreading
		{
			get;
			set;
		}

		public bool ShutingDown
		{
			set
			{
				this.m_shutingDown = value;
			}
		}

		public Scheduler()
		{
			this.MaxWaitTime = 1000f;
			this.MinWaitTime = 100f;
			this.m_coroutine = null;
			this.MaxTasksPerUpdate = 100;
			this.MaxFinishPerUpdate = 100;
			this.m_scheduledTasks = new LinkedList<IThreadedTask>();
			this.m_finishedTasks = new LinkedList<IThreadedTask>();
			this.m_runningTasks = new LinkedList<IThreadedTask>();
			this.m_waitingTasks = new LinkedList<IThreadedTask>();
			this.m_haveRan = new LinkedList<IThreadedTask>();
		}

		public Scheduler(int maxTasksPerUpdate, int maxFinishPerUpdate, ICoroutine coroutine)
		{
			this.MaxWaitTime = 1000f;
			this.MinWaitTime = 100f;
			this.m_coroutine = coroutine;
			this.MaxTasksPerUpdate = Math.Max(1, maxTasksPerUpdate);
			this.MaxFinishPerUpdate = Math.Max(1, maxFinishPerUpdate);
			this.m_scheduledTasks = new LinkedList<IThreadedTask>();
			this.m_finishedTasks = new LinkedList<IThreadedTask>();
			this.m_runningTasks = new LinkedList<IThreadedTask>();
			this.m_waitingTasks = new LinkedList<IThreadedTask>();
			this.m_haveRan = new LinkedList<IThreadedTask>();
		}

		public void Update()
		{
			this.TasksRanThisUpdate = 0;
			this.TasksFinishedThisUpdate = 0;
			this.FinishTasks();
			while (this.TasksRanThisUpdate < this.MaxTasksPerUpdate)
			{
				if (this.ScheduledTasks() > 0)
				{
					IThreadedTask value = this.m_scheduledTasks.First.Value;
					this.m_scheduledTasks.RemoveFirst();
					this.RunTask(value);
				}
				this.CheckForException();
				if (this.ScheduledTasks() == 0)
				{
					break;
				}
			}
			this.FinishTasks();
		}

		private void RunTask(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.TasksRanThisUpdate++;
				if (!task.IsThreaded || this.DisableMultithreading)
				{
					task.Start();
					IEnumerator enumerator = task.Run();
					if (enumerator != null)
					{
						if (this.m_coroutine == null)
						{
							throw new InvalidOperationException("Scheduler trying to run a coroutine task when coroutine interface is null");
						}
						this.m_coroutine.RunCoroutine(enumerator);
						if (!this.IsFinishing(task))
						{
							this.m_runningTasks.AddLast(task);
						}
					}
					else
					{
						task.End();
					}
				}
				else
				{
					task.Start();
					this.m_runningTasks.AddLast(task);
					ThreadPool.QueueUserWorkItem(new WaitCallback(this.RunThreaded), task);
				}
			}
		}

		private void RunThreaded(object o)
		{
			IThreadedTask threadedTask = o as IThreadedTask;
			if (threadedTask == null)
			{
				this.Throw(new InvalidCastException("Object is not a ITask or is null"));
			}
			else
			{
				try
				{
					threadedTask.Run();
				}
				catch (Exception e)
				{
					this.Throw(e);
				}
			}
		}

		public void FinishTasks()
		{
			if (this.TasksFinishedThisUpdate >= this.MaxFinishPerUpdate)
			{
				return;
			}
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_haveRan.Clear();
				LinkedList<IThreadedTask>.Enumerator enumerator = this.m_runningTasks.GetEnumerator();
				while (enumerator.MoveNext())
				{
					IThreadedTask current = enumerator.Current;
					if (current.Ran)
					{
						this.m_haveRan.AddLast(current);
					}
				}
				LinkedList<IThreadedTask>.Enumerator enumerator2 = this.m_haveRan.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					this.Finished(enumerator2.Current);
				}
				if (this.m_finishedTasks.Count != 0)
				{
					IThreadedTask threadedTask = this.m_finishedTasks.First.Value;
					this.m_finishedTasks.RemoveFirst();
					while (threadedTask != null)
					{
						threadedTask.End();
						this.TasksFinishedThisUpdate++;
						if (this.m_finishedTasks.Count == 0 || this.TasksFinishedThisUpdate >= this.MaxFinishPerUpdate)
						{
							threadedTask = null;
						}
						else
						{
							threadedTask = this.m_finishedTasks.First.Value;
							this.m_finishedTasks.RemoveFirst();
						}
					}
					this.m_haveRan.Clear();
				}
			}
		}

		public bool HasTasks()
		{
			return this.ScheduledTasks() > 0 || this.RunningTasks() > 0 || this.FinishingTasks() > 0 || this.WaitingTasks() > 0;
		}

		public void Cancel(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_scheduledTasks.Contains(task))
				{
					task.Cancel();
					this.m_scheduledTasks.Remove(task);
				}
				else if (this.m_waitingTasks.Contains(task))
				{
					task.Cancel();
					this.m_waitingTasks.Remove(task);
				}
			}
		}

		public void CancelAllTasks()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_scheduledTasks.Clear();
				this.m_waitingTasks.Clear();
				foreach (IThreadedTask current in this.m_runningTasks)
				{
					current.Cancel();
				}
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				bool flag = false;
				while (!flag && (float)stopwatch.ElapsedMilliseconds < this.MaxWaitTime)
				{
					flag = true;
					foreach (IThreadedTask current2 in this.m_runningTasks)
					{
						if (!current2.Ran)
						{
							flag = false;
							break;
						}
					}
				}
				while ((float)stopwatch.ElapsedMilliseconds < this.MinWaitTime)
				{
				}
				this.m_runningTasks.Clear();
				this.m_finishedTasks.Clear();
			}
		}

		public bool Contains(IThreadedTask task)
		{
			return this.IsScheduled(task) || this.IsWaiting(task) || this.IsRunning(task) || this.IsFinishing(task);
		}

		public int ScheduledTasks()
		{
			int result = 0;
			object @lock = this.m_lock;
			lock (@lock)
			{
				result = this.m_scheduledTasks.Count;
			}
			return result;
		}

		public int RunningTasks()
		{
			int result = 0;
			object @lock = this.m_lock;
			lock (@lock)
			{
				result = this.m_runningTasks.Count;
			}
			return result;
		}

		public int WaitingTasks()
		{
			int result = 0;
			object @lock = this.m_lock;
			lock (@lock)
			{
				result = this.m_waitingTasks.Count;
			}
			return result;
		}

		public int FinishingTasks()
		{
			int result = 0;
			object @lock = this.m_lock;
			lock (@lock)
			{
				result = this.m_finishedTasks.Count;
			}
			return result;
		}

		public int Tasks()
		{
			return this.RunningTasks() + this.ScheduledTasks() + this.WaitingTasks() + this.FinishingTasks();
		}

		public bool IsScheduled(IThreadedTask task)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = this.m_scheduledTasks.Contains(task);
			}
			return result;
		}

		public bool IsRunning(IThreadedTask task)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = this.m_runningTasks.Contains(task);
			}
			return result;
		}

		public bool IsWaiting(IThreadedTask task)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = this.m_waitingTasks.Contains(task);
			}
			return result;
		}

		public bool IsFinishing(IThreadedTask task)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				result = this.m_finishedTasks.Contains(task);
			}
			return result;
		}

		public void Add(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!this.m_shutingDown)
				{
					task.Scheduler = this;
					this.m_scheduledTasks.AddLast(task);
				}
			}
		}

		public void Run(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!this.m_shutingDown)
				{
					task.Scheduler = this;
					if (this.TasksRanThisUpdate >= this.MaxTasksPerUpdate)
					{
						this.Add(task);
					}
					else
					{
						this.RunTask(task);
					}
				}
			}
		}

		public void AddWaiting(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!this.m_shutingDown)
				{
					task.Scheduler = this;
					this.m_waitingTasks.AddLast(task);
				}
			}
		}

		public void Finished(IThreadedTask task)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_runningTasks.Remove(task);
				if (!this.m_shutingDown && !task.NoFinish && !task.Cancelled)
				{
					this.m_finishedTasks.AddLast(task);
				}
			}
		}

		public void CheckForException()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_exception != null)
				{
					Exception exception = this.m_exception;
					this.m_exception = null;
					throw exception;
				}
			}
		}

		public void Throw(Exception e)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_exception = e;
			}
		}

		public void StopWaiting(IThreadedTask task, bool run)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!this.m_shutingDown)
				{
					this.m_waitingTasks.Remove(task);
					if (run)
					{
						this.RunTask(task);
					}
					else
					{
						this.m_scheduledTasks.AddLast(task);
					}
				}
			}
		}

		public void Clear()
		{
			if (this.RunningTasks() > 0)
			{
				throw new InvalidOperationException("Can not clear the scheduler when there are running tasks.");
			}
			this.m_scheduledTasks.Clear();
			this.m_runningTasks.Clear();
			this.m_finishedTasks.Clear();
			this.m_waitingTasks.Clear();
			this.m_exception = null;
		}
	}
}
