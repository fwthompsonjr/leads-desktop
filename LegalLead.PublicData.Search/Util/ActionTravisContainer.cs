using StructureMap;

namespace LegalLead.PublicData.Search.Util
{
    public static class ActionTravisContainer
    {
        private static Container _container;
        private static Container _alternateContainer;

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
                return _container ?? (_container =
                  new Container(new ActionTravisRegistry()));
            }
        }

        /// <summary>
        /// Gets the alternate container.
        /// </summary>
        /// <value>
        /// The container of registered interfaces.
        /// </value>
        public static Container GetNonJusticeContainer
        {
            get
            {
                return _alternateContainer ?? (_alternateContainer =
                  new Container(new ActionZeroRegistry()));
            }
        }
    }
}