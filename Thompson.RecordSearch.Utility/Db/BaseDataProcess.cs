using Harris.Criminal.Db.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Thompson.RecordSearch.Utility.Db
{
    public abstract class BaseDataProcess
    {
        private string _name;
        private string LoweredName => Name.ToLower(CultureInfo.CurrentCulture);
        public virtual string Name => _name ?? (_name = GetType().Name.Replace("DataProcess", ""));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exception is handled and logged")]
        public virtual void Process(IProgress<HccProcess> progress)
        {
            var process = HccProcess.LastOrDefault(LoweredName);
            if (process != null) return;
            try
            {
                process = HccProcess.Begin(LoweredName);
                Execute(progress, process);
                HccProcess.End(LoweredName);
            }
            catch (Exception exception)
            {
                // exception is handled and logged
                HccProcess.End(LoweredName, exception);
            }
        }

        protected abstract HccProcess Execute(IProgress<HccProcess> progress, HccProcess process);


        protected static IEnumerable<T> GetEnumerableOfType<T>(string processName, params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            Type actionable = typeof(DataActionAttribute);
            var collection = Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)))
                .Where(x => x.GetCustomAttributes(actionable) != null)
                .Select(y => new
                {
                    DataType = y,
                    CustomInfo = y.GetCustomAttributes(actionable).First() as DataActionAttribute
                })
                .Where(z => z.CustomInfo.IsShared || z.CustomInfo.Name.Equals(processName, StringComparison.OrdinalIgnoreCase))
                .ToList();
            collection.Sort((a, b) => a.CustomInfo.ProcessId.CompareTo(b.CustomInfo.ProcessId));
            foreach (var type in collection)
            {
                objects.Add((T)Activator.CreateInstance(type.DataType, constructorArgs));
            }
            return objects;
        }
    }
}
