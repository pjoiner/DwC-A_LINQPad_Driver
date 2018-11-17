using System.Collections.Generic;
using DwC_A;
using DwC_A.Terms;

namespace DwC_A_Driver
{
    public partial class CoreFileTemplate
    {
        private readonly HashSet<string> keywords = new HashSet<string>()
        {
            "class", "abstract"
        };

        public readonly string FileName;
        public readonly IFileReader FileReader;
        public CoreFileTemplate(string fileName, IFileReader fileReader)
        {
            FileName = fileName;
            FileReader = fileReader;
        }

        public string ModifyKeywords(string name)
        {
            var propertyName = Terms.ShortName(name);
            if (keywords.Contains(propertyName))
            {
                return $"@{propertyName}";
            }
            return propertyName;
        }
    }
}
