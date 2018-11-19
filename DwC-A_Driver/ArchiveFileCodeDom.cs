using DwC_A;
using DwC_A.Meta;
using DwC_A.Terms;
using System.CodeDom;
using System.Collections.Generic;

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

            return CodeDomUtils.GenerateSourceFromCodeDom(DwCArchive);
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
            var classType = new CodeTypeDeclaration(CodeDomUtils.ExtractClassName(fileName) + "Type")
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
