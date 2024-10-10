using LegalLead.PublicData.Search.Classes;

namespace legallead.search.tests.classes
{
    public class ShortcutBuilderTests
    {
        [Fact]
        public void ShortcutBuilder_Generate()
        {
            for (int i = 0; i < 3; i++)
            {
                var error = Record.Exception(() => ShortcutGenerator.Generate());
                Assert.Null(error); 
            }
        }
    }
}
