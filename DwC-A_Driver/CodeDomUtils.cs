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

        public static string ExtractClassName(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName);
        }

    }
}
