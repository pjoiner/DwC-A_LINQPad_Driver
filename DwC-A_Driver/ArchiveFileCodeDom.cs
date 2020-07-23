using DwC_A;
using DwC_A.Meta;
using System.CodeDom;

namespace DwC_A_Driver
{
    class ArchiveFileCodeDom
    {
        private readonly bool capitalize;

        public ArchiveFileCodeDom(bool capitalize = false)
        {
            this.capitalize = capitalize;
        }

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
            var classType = new CodeTypeDeclaration(CodeDomUtils.ExtractClassName(fileName, capitalize) + "Type")
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
                Name = CodeDomUtils.ModifyKeywords(field.Term, capitalize),
                HasGet = true
            };
            fieldProperty.GetStatements.Add(new CodeSnippetExpression($"return row[\"{field.Term}\"]"));
            return fieldProperty;
        }

    }
}
