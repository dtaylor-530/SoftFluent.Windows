namespace SoftFluent.Windows
{
    public static class DecamelizationHelper
    {
        public static string Decamelize(string text)
        {
            return Decamelize(text, null);
        }

        public static string Decamelize(string text, DecamelizeOptions options)
        {
            return ServiceProvider.Current.GetService<IDecamelizer>().Decamelize(text, options);
        }
    }
}
