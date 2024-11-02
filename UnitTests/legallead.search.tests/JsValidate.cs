using Jint;

namespace legallead.search.tests
{
    internal static class JsValidate
    {
        public static bool IsValid(string value)
        {
            using var engine = new Engine();
            _ = engine.Evaluate(value);
            return true;
        }
    }
}
