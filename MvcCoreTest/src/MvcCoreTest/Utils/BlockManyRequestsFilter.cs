namespace MvcCoreTest.Utils
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class BlockManyRequestsFilter : IActionFilter, IDisposable
    {
        private const int MaxRequestsIn30Seconds = 30;

        private static readonly TimeSpan CheckPeriod = TimeSpan.FromSeconds(30);

        private readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> requestsDictionary =
            new ConcurrentDictionary<string, ConcurrentQueue<DateTime>>();

        private Timer timer;

        public BlockManyRequestsFilter()
        {
            this.timer = new Timer(this.OnTimer, null, CheckPeriod, CheckPeriod);
        }
        
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrEmpty(ip))
            {
                return;
            }

            ConcurrentQueue<DateTime> queue;
            if (!this.requestsDictionary.ContainsKey(ip))
            {
                queue = new ConcurrentQueue<DateTime>();
                this.requestsDictionary.TryAdd(ip, queue);
            }
            else
            {
                this.requestsDictionary.TryGetValue(ip, out queue);
            }

            if (queue == null)
            {
                return;
            }

            queue.Enqueue(DateTime.Now);
            if (queue.Count > MaxRequestsIn30Seconds)
            {
                context.Result = new BadRequestObjectResult("Too many requests");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        private void OnTimer(object state)
        {
            var minDate = DateTime.Now - CheckPeriod;
            var ips = this.requestsDictionary.Keys.ToArray();
            foreach (var ip in ips)
            {
                var queue = this.requestsDictionary[ip];
                if (queue.IsEmpty)
                {
                    this.requestsDictionary.TryRemove(ip, out queue);
                    continue;
                }

                var remove = false;
                while (true)
                {
                    DateTime date;
                    if (queue.IsEmpty || !queue.TryPeek(out date))
                    {
                        remove = true;
                        break;
                    }

                    if (date < minDate)
                    {
                        queue.TryDequeue(out date);
                    }
                    else
                    {
                        break;
                    }
                }

                if (remove)
                {
                    this.requestsDictionary.TryRemove(ip, out queue);
                }
            }
        }

        public void Dispose()
        {
            this.timer?.Dispose();
            this.timer = null;
        }

        ~BlockManyRequestsFilter()
        {
            this.Dispose();
        }
    }
}
