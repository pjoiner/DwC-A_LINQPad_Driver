using DwC_A.Terms;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;

namespace DwC_A_Driver
{
    class CodeDomUtils
    {
        public static string GenerateSourceFromCodeDom(CodeNamespace DwCArchive)
        {
            CodeCompileUnit compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(DwCArchive);
            CodeDomProvider csc = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions
            {
                BracingStyle = "C"
            };
            StringBuilder code = new StringBuilder();
            using (StringWriter sourceWriter = new StringWriter(code))
            {
                csc.GenerateCodeFromCompileUnit(
                    compileUnit, sourceWriter, options);
            }
            return code.ToString();
        }

        public static string ExtractClassName(string fileName, bool capitalize = false)
        {
            var className = Path.GetFileNameWithoutExtension(fileName);
            return ModifyKeywords(className, capitalize);
        }

        public static string ModifyKeywords(string name, bool capitalize = false)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
            var propertyName = Terms.ShortName(name);
            if (capitalize)
            {
                return char.ToUpper(propertyName[0]) + propertyName.Substring(1);
            }
            if (!provider.IsValidIdentifier(propertyName))
            {
                return $"@{propertyName}";
            }
            return propertyName;
        }

    }
}
