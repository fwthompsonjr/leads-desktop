using Harris.Criminal.Db.Entities;
using System;

namespace Harris.Criminal.Db.Prc
{
    public abstract class BaseDataProcess : IDataProcess
    {
        private string _name;
        public virtual string Name => _name ?? (_name = GetType().Name.Replace("DataProcess", ""));


        public virtual void Process(IProgress<HccProcess> progress)
        {
            HccProcess proc = GetProcessByName(Name.ToLower());
            for (int i = 0; i != 100; ++i)
            {
                //Thread.Sleep(100); // CPU-bound work
                //if (progress != null)
                //    progress.Report(i);
            }
        }
    }
}
