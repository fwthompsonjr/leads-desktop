using StructureMap;

namespace Thompson.RecordSearch.Utility.Tools
{
    /// <summary>
    /// Class definition to supply DI container needed 
    /// to access web driver automation objects
    /// </summary>
    public static class ExcelLayoutContainer
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
                  new Container(new LayoutRegistry()));
            }
        }
    }
}
