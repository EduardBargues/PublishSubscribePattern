using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dawn;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace PublishSubscribePattern
{
    internal class Scheduler
    {
        readonly Queue<Func<Task>> tasks;
        readonly AsyncLock door;
        readonly string schedulerName;
        readonly ILogger logger;
        Task currentTask;

        public Scheduler(string schedulerName, ILogger logger)
        {
            this.logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            this.schedulerName = Guard.Argument(schedulerName, nameof(schedulerName))
                .NotNull()
                .NotEmpty().Value;
            Log($"Creating scheduler.");
            tasks = new Queue<Func<Task>>();
            door = new AsyncLock();
        }

        /// <summary>
        ///  Enqueues a task defined as an expresion.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public async Task Enqueue(Func<Task> task)
        {
            Log("Enqueuing task.");
            using (await door.LockAsync())
                tasks.Enqueue(task);
            await ExecuteNextTask(currentTask).ConfigureAwait(false);
        }

        async Task ExecuteNextTask(Task previousTask)
        {
            bool previousTaskCompleted = previousTask != null && previousTask.IsCompleted;
            if (previousTask == null || previousTaskCompleted)
            {
                if (previousTaskCompleted)
                    Log("Task completed.");
                Func<Task> func = await DequeueNextTask().ConfigureAwait(false);
                if (func != null)
                    using (await door.LockAsync())
                    {
                        Log("Starting next task.");
                        currentTask = func();
                        currentTask.ContinueWith(async t => await ExecuteNextTask(t).ConfigureAwait(false));
                    }
            }
        }

        async Task<Func<Task>> DequeueNextTask()
        {
            using (await door.LockAsync())
            {
                try
                {
                    Func<Task> task = tasks.Dequeue();
                    return task;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }

        void Log(string message) => logger.LogDebug($"{nameof(Scheduler)} {schedulerName} - {message}");
    }
}
