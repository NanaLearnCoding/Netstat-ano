namespace Netstat_ano.Models
{
    public class LanguageInfo
    {
        public string? Name { get; set; }

        public string? CultureName { get; set; }

        public string? ResourceOriginal { get; set; }

        public LanguageInfo(string? name, string? lang, string? resourceOriginal)
        {
            Name = name;
            CultureName = lang;
            ResourceOriginal = resourceOriginal;
        }
    }
}
