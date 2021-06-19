namespace Holism.Framework
{
    public static class StringExtensions
    {
        public static bool IsNothing(this string text)
        {
            var isNothing = string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text);
            return isNothing;
        }

        public static bool IsSomething(this string text)
        {
            var isSomething = !text.IsNothing();
            return isSomething;
        }
    }
}