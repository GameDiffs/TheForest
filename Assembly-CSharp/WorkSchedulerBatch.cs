using System;
using System.Collections.Generic;
using System.Diagnostics;

public class WorkSchedulerBatch
{
	public delegate int DoWorkDelegate(long maxTicks, bool autoUnregister = false);

	private List<WorkScheduler.Task> tasks = new List<WorkScheduler.Task>();

	private int iterator;

	private Stopwatch stopwatch = new Stopwatch();

	public WorkSchedulerBatch.DoWorkDelegate DoWork;

	public int LastNearRefreshFrame
	{
		get;
		set;
	}

	public int LastFarRefreshFrame
	{
		get;
		set;
	}

	public int WorkUnityCount
	{
		get
		{
			return this.tasks.Count;
		}
	}

	public WorkSchedulerBatch()
	{
		this.DoWork = new WorkSchedulerBatch.DoWorkDelegate(this.DoWorkNoTry);
	}

	public bool Contains(WorkScheduler.Task task)
	{
		return this.tasks.Contains(task);
	}

	public void Register(WorkScheduler.Task task, bool force = false)
	{
		if (force || !this.tasks.Contains(task))
		{
			this.tasks.Add(task);
		}
	}

	public void Unregister(WorkScheduler.Task task)
	{
		int num = this.tasks.LastIndexOf(task);
		if (num >= 0)
		{
			this.tasks.RemoveAt(num);
		}
	}

	public int DoWorkTryCatch(long maxTicks, bool autoUnregister = false)
	{
		int count = this.tasks.Count;
		int num = 0;
		this.stopwatch.Reset();
		this.stopwatch.Start();
		while (num < count && (this.stopwatch.ElapsedTicks < maxTicks || WorkScheduler.FullCycle))
		{
			int count2 = this.tasks.Count;
			if (this.iterator < 0)
			{
				this.iterator += count2;
			}
			this.iterator = (this.iterator + count2) % count2;
			try
			{
				this.tasks[this.iterator]();
			}
			catch
			{
			}
			num++;
			if (autoUnregister)
			{
				this.tasks.RemoveAt(this.iterator);
			}
			this.iterator--;
		}
		this.stopwatch.Stop();
		return num;
	}

	public int DoWorkNoTry(long maxTicks, bool autoUnregister = false)
	{
		int count = this.tasks.Count;
		int num = 0;
		this.stopwatch.Reset();
		this.stopwatch.Start();
		while (num < count && (this.stopwatch.ElapsedTicks < maxTicks || WorkScheduler.FullCycle))
		{
			int count2 = this.tasks.Count;
			if (this.iterator < 0)
			{
				this.iterator += count2;
			}
			this.iterator = (this.iterator + count2) % count2;
			this.tasks[this.iterator]();
			num++;
			if (autoUnregister)
			{
				this.tasks.RemoveAt(this.iterator);
			}
			this.iterator--;
		}
		this.stopwatch.Stop();
		return num;
	}
}
