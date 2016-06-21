using System;
using System.Diagnostics;
using UnityEngine;

namespace Pathfinding
{
	public class Profile
	{
		private const bool PROFILE_MEM = false;

		private const bool dontCountFirst = false;

		public readonly string name;

		private readonly Stopwatch watch;

		private int counter;

		private long mem;

		private long smem;

		private int control = 1073741824;

		public Profile(string name)
		{
			this.name = name;
			this.watch = new Stopwatch();
		}

		public int ControlValue()
		{
			return this.control;
		}

		[Conditional("PROFILE")]
		public void Start()
		{
			this.watch.Start();
		}

		[Conditional("PROFILE")]
		public void Stop()
		{
			this.counter++;
			this.watch.Stop();
		}

		[Conditional("PROFILE")]
		public void Log()
		{
			UnityEngine.Debug.Log(this.ToString());
		}

		[Conditional("PROFILE")]
		public void ConsoleLog()
		{
			Console.WriteLine(this.ToString());
		}

		[Conditional("PROFILE")]
		public void Stop(int control)
		{
			this.counter++;
			this.watch.Stop();
			if (this.control == 1073741824)
			{
				this.control = control;
			}
			else if (this.control != control)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Control numbers do not match ",
					this.control,
					" != ",
					control
				}));
			}
		}

		[Conditional("PROFILE")]
		public void Control(Profile other)
		{
			if (this.ControlValue() != other.ControlValue())
			{
				throw new Exception(string.Concat(new object[]
				{
					"Control numbers do not match (",
					this.name,
					" ",
					other.name,
					") ",
					this.ControlValue(),
					" != ",
					other.ControlValue()
				}));
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.name,
				" #",
				this.counter,
				" ",
				this.watch.Elapsed.TotalMilliseconds.ToString("0.0 ms"),
				" avg: ",
				(this.watch.Elapsed.TotalMilliseconds / (double)this.counter).ToString("0.00 ms")
			});
		}
	}
}
