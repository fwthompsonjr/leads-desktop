using Harris.Criminal.Db.Entities;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Db
{
    public abstract class BaseAction
    {
        private string _name;

        public BaseAction(HccProcess process)
        {
            Current = process;
        }

        public virtual IWebDriver WebDriver { get; set; }

        public virtual HccProcess Current { get; }

        protected virtual IProgress<HccProcess> ReportProgress { get; set; }

        public DateTime? StartTime { get; private set; }

        public DateTime? EndTime { get; private set; }

        public abstract TimeSpan EstimatedDuration { get; }

        public TimeSpan Elapsed => DateTime.Now.Subtract(StartTime.GetValueOrDefault(DateTime.Now));

        public async Task ExecuteAsync(IProgress<HccProcess> progress)
        {
            await Task.Run(() => Execute(progress)).ConfigureAwait(true);
        }

        public abstract void Execute(IProgress<HccProcess> progress);

        protected string Name => _name ?? (_name = GetType().Name.Replace("Action", ""));

        protected void Start()
        {
            if (!StartTime.HasValue)
            {
                StartTime = DateTime.Now;
            }

            if (Current == null)
            {
                return;
            }

            if (Current.Messages == null)
            {
                Current.Messages = new List<HccMessage>();
            }

            Current.Messages.Add(new HccMessage
            {
                Date = DateTime.Now,
                Comment = $"{Name} sub-process starting.",
                Progress = new HccProgress { Count = 0, Total = 1 },
                StatusId = HccStatus.Information
            });
            Update();
        }

        protected void End()
        {
            if (!EndTime.HasValue)
            {
                EndTime = DateTime.Now;
            }

            if (Current == null)
            {
                return;
            }

            if (Current.Messages == null)
            {
                Current.Messages = new List<HccMessage>();
            }

            Current.Messages.Add(new HccMessage
            {
                Date = DateTime.Now,
                Comment = $"{Name} sub-process completed.",
                Progress = new HccProgress { Count = 1, Total = 1 },
                StatusId = HccStatus.Information
            });
            Update();
        }

        protected void Update()
        {
            HccProcess.Update(Current);
            if (ReportProgress == null)
            {
                return;
            }

            ReportProgress.Report(Current);
        }

        protected void Information(string message)
        {
            WriteLog(HccStatus.Information, message);
        }

        protected void Verbose(string message)
        {
            WriteLog(HccStatus.Verbose, message);
        }

        protected void Warning(string message)
        {
            WriteLog(HccStatus.Warning, message);
        }

        protected void Error(string message)
        {
            WriteLog(HccStatus.Error, message);
        }

        private void WriteLog(int statusId, string message)
        {
            if (Current == null)
            {
                return;
            }

            if (Current.Messages == null)
            {
                Current.Messages = new List<HccMessage>();
            }

            var nextMessage = Current.Messages.LastOrDefault() ?? new HccMessage();
            Current.Messages.Add(new HccMessage
            {
                Date = DateTime.Now,
                Comment = message,
                Progress = nextMessage.Progress,
                StatusId = statusId
            });
            Update();
        }
    }
}
