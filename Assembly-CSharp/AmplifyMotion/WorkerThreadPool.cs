using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace AmplifyMotion
{
	internal class WorkerThreadPool
	{
		private const int ThreadStateQueueCapacity = 1024;

		internal Queue<MotionState>[] m_threadStateQueues;

		internal object[] m_threadStateQueueLocks;

		private int m_threadPoolSize;

		private ManualResetEvent m_threadPoolTerminateSignal;

		private AutoResetEvent[] m_threadPoolContinueSignals;

		private Thread[] m_threadPool;

		private bool m_threadPoolFallback;

		internal object m_threadPoolLock;

		internal int m_threadPoolIndex;

		internal void InitializeAsyncUpdateThreads(int threadCount, bool systemThreadPool)
		{
			if (systemThreadPool)
			{
				this.m_threadPoolFallback = true;
				return;
			}
			try
			{
				this.m_threadPoolSize = threadCount;
				this.m_threadStateQueues = new Queue<MotionState>[this.m_threadPoolSize];
				this.m_threadStateQueueLocks = new object[this.m_threadPoolSize];
				this.m_threadPool = new Thread[this.m_threadPoolSize];
				this.m_threadPoolTerminateSignal = new ManualResetEvent(false);
				this.m_threadPoolContinueSignals = new AutoResetEvent[this.m_threadPoolSize];
				this.m_threadPoolLock = new object();
				this.m_threadPoolIndex = 0;
				for (int i = 0; i < this.m_threadPoolSize; i++)
				{
					this.m_threadStateQueues[i] = new Queue<MotionState>(1024);
					this.m_threadStateQueueLocks[i] = new object();
					this.m_threadPoolContinueSignals[i] = new AutoResetEvent(false);
					this.m_threadPool[i] = new Thread(new ParameterizedThreadStart(WorkerThreadPool.AsyncUpdateThread));
					this.m_threadPool[i].Start(new KeyValuePair<object, int>(this, i));
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning("[AmplifyMotion] Non-critical error while initializing WorkerThreads. Falling back to using System.Threading.ThreadPool().\n" + ex.Message);
				this.m_threadPoolFallback = true;
			}
		}

		internal void FinalizeAsyncUpdateThreads()
		{
			if (!this.m_threadPoolFallback)
			{
				this.m_threadPoolTerminateSignal.Set();
				for (int i = 0; i < this.m_threadPoolSize; i++)
				{
					if (this.m_threadPool[i].IsAlive)
					{
						this.m_threadPoolContinueSignals[i].Set();
						this.m_threadPool[i].Join();
						this.m_threadPool[i] = null;
					}
					object obj = this.m_threadStateQueueLocks[i];
					lock (obj)
					{
						while (this.m_threadStateQueues[i].Count > 0)
						{
							this.m_threadStateQueues[i].Dequeue().AsyncUpdate();
						}
					}
				}
				this.m_threadStateQueues = null;
				this.m_threadStateQueueLocks = null;
				this.m_threadPoolSize = 0;
				this.m_threadPool = null;
				this.m_threadPoolTerminateSignal = null;
				this.m_threadPoolContinueSignals = null;
				this.m_threadPoolLock = null;
				this.m_threadPoolIndex = 0;
			}
		}

		internal void EnqueueAsyncUpdate(MotionState state)
		{
			if (!this.m_threadPoolFallback)
			{
				object obj = this.m_threadStateQueueLocks[this.m_threadPoolIndex];
				lock (obj)
				{
					this.m_threadStateQueues[this.m_threadPoolIndex].Enqueue(state);
				}
				this.m_threadPoolContinueSignals[this.m_threadPoolIndex].Set();
				this.m_threadPoolIndex++;
				if (this.m_threadPoolIndex >= this.m_threadPoolSize)
				{
					this.m_threadPoolIndex = 0;
				}
			}
			else
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerThreadPool.AsyncUpdateCallback), state);
			}
		}

		private static void AsyncUpdateCallback(object obj)
		{
			MotionState motionState = (MotionState)obj;
			motionState.AsyncUpdate();
		}

		private static void AsyncUpdateThread(object obj)
		{
			KeyValuePair<object, int> keyValuePair = (KeyValuePair<object, int>)obj;
			WorkerThreadPool workerThreadPool = (WorkerThreadPool)keyValuePair.Key;
			int value = keyValuePair.Value;
			while (true)
			{
				try
				{
					workerThreadPool.m_threadPoolContinueSignals[value].WaitOne();
					if (workerThreadPool.m_threadPoolTerminateSignal.WaitOne(0))
					{
						break;
					}
					while (true)
					{
						MotionState motionState = null;
						object obj2 = workerThreadPool.m_threadStateQueueLocks[value];
						lock (obj2)
						{
							if (workerThreadPool.m_threadStateQueues[value].Count > 0)
							{
								motionState = workerThreadPool.m_threadStateQueues[value].Dequeue();
							}
						}
						if (motionState == null)
						{
							break;
						}
						motionState.AsyncUpdate();
					}
				}
				catch (Exception ex)
				{
					if (ex.GetType() != typeof(ThreadAbortException))
					{
						Debug.LogWarning(ex);
					}
				}
			}
		}
	}
}
