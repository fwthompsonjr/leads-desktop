using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thompson.RecordSearch.Utility.Web
{
    /// <summary>
    /// Class definition to supply DI container needed 
    /// to access action element automation objects
    /// </summary>
    public class ActionElementContainer
    {
        private static Container _container;

        /// <summary>
        /// Gets the get container.
        /// </summary>
        /// <value>
        /// The container of registered interfaces.
        /// </value>
        public static Container GetContainer
        {
            get { return _container ?? (_container = 
                    new Container(new ActionElementRegistry())); }
        }
    }
}
