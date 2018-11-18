using DwC_A;
using DwC_A.Meta;
using DwC_A.Terms;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DwC_A_Driver
{
    public class ArchiveFileCodeDom
    {
        private readonly HashSet<string> keywords = new HashSet<string>()
        {
            "class", "abstract"
        };

        public string GenerateSource(string fileName, IFileMetaData fileMetaData)
        {
            var DwCArchive = CreateNamespace();
            var classType = CreateClass(fileName, fileMetaData);
            DwCArchive.Types.Add(classType);

            return GenerateSourceFromCodeDom(DwCArchive);
        }

        private string GenerateSourceFromCodeDom(CodeNamespace DwCArchive)
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

        private CodeNamespace CreateNamespace()
        {
            CodeNamespace DwCArchive = new CodeNamespace("DwCArchive");
            DwCArchive.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            DwCArchive.Imports.Add(new CodeNamespaceImport("DwC_A"));
            DwCArchive.Imports.Add(new CodeNamespaceImport("DwC_A.Terms"));
            return DwCArchive;
        }

        private CodeTypeDeclaration CreateClass(string fileName, IFileMetaData fileMetaData)
        {
            var classType = new CodeTypeDeclaration(ExtractClassName(fileName))
            {
                IsClass = true
            };
            CodeConstructor constructor = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(IRow), "row"));
            constructor.Statements.Add(new CodeSnippetExpression("this.row = row"));
            classType.Members.Add(constructor);
            var row = new CodeMemberField()
            {
                Type = new CodeTypeReference(typeof(IRow)),
                Attributes = MemberAttributes.Private | MemberAttributes.Final,
                Name = "row",
            };
            classType.Members.Add(row);
            foreach (var field in fileMetaData.Fields)
            {
                classType.Members.Add(CreateProperty(field));
            }
            return classType;
        }

        private CodeMemberProperty CreateProperty(FieldType field)
        {
            var fieldProperty = new CodeMemberProperty()
            {
                Type = new CodeTypeReference(typeof(string)),
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = ModifyKeywords(field.Term),
                HasGet = true
            };
            fieldProperty.GetStatements.Add(new CodeSnippetExpression($"return row[\"{field.Term}\"]"));
            return fieldProperty;
        }

        private string ExtractClassName(string fileName)
        {
            return Path.GetFileNameWithoutExtension(fileName) + "Type";
        }

        private string ModifyKeywords(string name)
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
