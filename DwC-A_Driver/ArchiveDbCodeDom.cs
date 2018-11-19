using DwC_A;
using System;
using System.CodeDom;

namespace DwC_A_Driver
{
    public class ArchiveDbCodeDom
    {
        public string GenerateSource(ArchiveReader archiveReader)
        {
            var DwCArchive = CreateNamespace();
            var classType = CreateClass(archiveReader);
            DwCArchive.Types.Add(classType);

            return CodeDomUtils.GenerateSourceFromCodeDom(DwCArchive);
        }

        private CodeNamespace CreateNamespace()
        {
            CodeNamespace DwCArchive = new CodeNamespace("DwCArchive");
            DwCArchive.Imports.Add(new CodeNamespaceImport("System"));
            DwCArchive.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
            DwCArchive.Imports.Add(new CodeNamespaceImport("DwC_A"));
            DwCArchive.Imports.Add(new CodeNamespaceImport("System.Linq"));
            DwCArchive.Imports.Add(new CodeNamespaceImport("System.Configuration"));
            return DwCArchive;
        }

        private CodeTypeDeclaration CreateClass(ArchiveReader archiveReader)
        {
            var classType = new CodeTypeDeclaration("ArchiveDb")
            {
                IsClass = true,
            };
            classType.BaseTypes.Add(typeof(IDisposable));
            CodeConstructor constructor = new CodeConstructor()
            {
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };
            constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "fileName"));
            constructor.Statements.Add(new CodeSnippetExpression("this.archive = new ArchiveReader(fileName)"));
            classType.Members.Add(constructor);
            var archive = new CodeMemberField()
            {
                Type = new CodeTypeReference(typeof(ArchiveReader)),
                Attributes = MemberAttributes.Private | MemberAttributes.Final,
                Name = "archive",
            };
            classType.Members.Add(archive);

            classType.Members.Add(CreateCoreFileProperty(archiveReader.CoreFile));
            foreach (var extension in archiveReader.Extensions)
            {
                classType.Members.Add(CreateExtensionFileProperty(extension));
            }
            classType.Members.Add(CreateDisposeMethod());
            return classType;
        }

        private CodeMemberProperty CreateCoreFileProperty(IFileReader file)
        {
            var propertyName = CodeDomUtils.ExtractClassName(file.FileName);
            var propertyTypeName = propertyName + "Type";
            var fieldProperty = new CodeMemberProperty()
            {
                Type = new CodeTypeReference($"IEnumerable<{propertyTypeName}>"),
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = propertyName,
                HasGet = true
            };
            fieldProperty.GetStatements.Add(new CodeSnippetExpression($"return archive.CoreFile.DataRows.Select( row => new {propertyTypeName}(row) )"));
            return fieldProperty;
        }

        private CodeMemberProperty CreateExtensionFileProperty(IFileReader file)
        {
            var propertyName = CodeDomUtils.ExtractClassName(file.FileName);
            var propertyTypeName = propertyName + "Type";
            var fieldProperty = new CodeMemberProperty()
            {
                Type = new CodeTypeReference($"IEnumerable<{propertyTypeName}>"),
                Attributes = MemberAttributes.Public | MemberAttributes.Final,
                Name = propertyName,
                HasGet = true
            };
            fieldProperty.GetStatements.Add(new CodeSnippetExpression($"return archive.Extensions.GetFileReaderByFileName(\"{file.FileMetaData.FileName}\").DataRows.Select( row => new {propertyTypeName}(row) )"));
            return fieldProperty;
        }

        private CodeMemberMethod CreateDisposeMethod()
        {
            var codeMemberMethod = new CodeMemberMethod()
            {
                Name = "Dispose",
                Attributes = MemberAttributes.Public | MemberAttributes.Final
            };
            codeMemberMethod.ImplementationTypes.Add(typeof(IDisposable));
            codeMemberMethod.Statements.Add(new CodeSnippetExpression("archive.Dispose()"));
            return codeMemberMethod;
        }

    }
}
