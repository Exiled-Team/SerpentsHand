// This was made by probe4aiur on GitHub. You can access the latest version here: 
// https://gist.github.com/probe4aiur/fc74510ea216d30cbb0b6b884c4ba84c

using UnityEngine;
using Smod2.EventHandlers;
using Smod2.Events;

using System;
using System.Collections.Generic;
using System.Linq;

namespace scp4aiur
{
    /// <summary>
    /// Module for easy and efficient frame-based timers
    /// </summary>
    internal class Timing : IEventHandlerUpdate, IEventHandlerRoundRestart
    {
        private static Action<string> log;

        private static int jobId;
        private static Dictionary<int, QueueItem> jobs;

        public static void Init(Smod2.Plugin plugin, Priority priority = Priority.Normal)
        {
            log = plugin.Info;
            plugin.AddEventHandlers(new Timing(), priority);

            jobId = int.MinValue;
            jobs = new Dictionary<int, QueueItem>();
        }

        /// <summary>
        /// Queues a job.
        /// </summary>
        /// <param name="item">Job to queue.</param>
        private static int Queue(QueueItem item)
        {
            int id = jobId++;
            jobs.Add(id, item);

            return id;
        }

        /// <summary>
        /// Removes a job from the queue.
        /// </summary>
        /// <param name="id">ID of the job to remove.</param>
        public static bool Remove(int id)
        {
            return jobs.Remove(id);
        }

        /// <summary>
        /// Queues a job for the next tick.
        /// </summary>
        /// <param name="action">Job to execute.</param>
        public static int Next(Action action, bool persist = false)
        {
            return Queue(new NextTickQueue(action, persist));
        }

        /// <summary>
        /// Queues a job to run in a certain amount of ticks.
        /// </summary>
        /// <param name="action">Job to execute.</param>
        /// <param name="ticks">Number of ticks to wait.</param>
        public static int InTicks(Action action, int ticks, bool persist = false)
        {
            return Queue(new AfterTicksQueue(action, ticks, persist));
        }

        /// <summary>
        /// Queues a job to run in a certain amount of seconds
        /// </summary>
        /// <param name="action">Job to execute.</param>
        /// <param name="seconds">Number of seconds to wait.</param>
        public static int In(Action<float> action, float seconds, bool persist = false)
        {
            return Queue(new TimerQueue(action, seconds, persist));
        }

        /// <summary>
        /// <para>DO NOT USE</para>
        /// <para>This is an event for Smod2 and as such should not be called by any external code </para>
        /// </summary>
        /// <param name="ev"></param>
        public void OnUpdate(UpdateEvent ev)
        {
            foreach (KeyValuePair<int, QueueItem> job in jobs.Where(x => x.Value.RunThisTick()).ToArray())
            {
                job.Value.Run();
                jobs.Remove(job.Key);
            }
        }

        /// <summary>
        /// <para>DO NOT USE</para>
        /// <para>This is an event for Smod2 and as such should not be called by any external code </para>
        /// </summary>
        /// <param name="ev"></param>
        public void OnRoundRestart(RoundRestartEvent ev)
        {
            foreach (KeyValuePair<int, QueueItem> job in jobs.Where(x => !x.Value.RoundPersist).ToArray())
            {
                jobs.Remove(job.Key);
            }
        }

        private abstract class QueueItem
        {
            private readonly string name;

            protected Action action;

            public Exception Exception { get; protected set; }
            public bool RoundPersist { get; }

            protected QueueItem(bool persist, string jobName)
            {
                RoundPersist = persist;
                name = jobName;
            }

            public abstract bool RunThisTick();

            public void Run()
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    log($"Exception was thrown by {name} job:\n{e}");
                }
            }
        }

        private class NextTickQueue : QueueItem
        {
            public NextTickQueue(Action jobAction, bool persist) : base(persist, "next-tick")
            {
                action = jobAction;
            }

            public override bool RunThisTick()
            {
                return true;
            }
        }

        private class AfterTicksQueue : QueueItem
        {
            private int ticksLeft;

            public AfterTicksQueue(Action jobAction, int ticks, bool persist) : base(persist, "after-ticks")
            {
                action = jobAction;
                ticksLeft = ticks;
            }

            public override bool RunThisTick()
            {
                return --ticksLeft < 1;
            }
        }

        private class TimerQueue : QueueItem
        {
            private float timeLeft;

            public TimerQueue(Action<float> jobAction, float time, bool persist) : base(persist, "timer")
            {
                action = () => jobAction(timeLeft);
                timeLeft = time;
            }

            public override bool RunThisTick()
            {
                return (timeLeft -= Time.deltaTime) <= 0;
            }
        }
    }
}
