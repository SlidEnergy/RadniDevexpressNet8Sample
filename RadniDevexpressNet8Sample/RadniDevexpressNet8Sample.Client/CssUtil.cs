namespace CommonBlazor.UI.Util
{
    public class CssUtil
    {
        public static string CombineCssClasses(params string[] value)
            => CombineCssClasses((IEnumerable<string>)value);
        public static string CombineCssClasses(IEnumerable<string> value)
        {
            var cssClass = String.Join(" ", value.Where(v => !string.IsNullOrWhiteSpace(v))).Trim();
            return String.IsNullOrWhiteSpace(cssClass) ? null : cssClass;
        }
    }
}
