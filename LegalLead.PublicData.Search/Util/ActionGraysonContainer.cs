using StructureMap;

namespace LegalLead.PublicData.Search.Util
{
    public static class ActionGraysonContainer
    {
        private static Container _container;

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>
        /// The container of registered interfaces.
        /// </value>
        public static Container GetContainer
        {
            get
            {
                return _container ??= new Container(new ActionGraysonRegistry());
            }
        }
    }
}