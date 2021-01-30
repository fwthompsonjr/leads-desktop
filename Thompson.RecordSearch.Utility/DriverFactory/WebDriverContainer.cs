
using StructureMap;

namespace Thompson.RecordSearch.Utility.DriverFactory
{
    /// <summary>
    /// Class definition to supply DI container needed 
    /// to access web driver automation objects
    /// </summary>
    public static class WebDriverContainer
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
            get
            {
                return _container ?? (_container =
                  new Container(new WebDriverRegistry()));
            }
        }
    }
}