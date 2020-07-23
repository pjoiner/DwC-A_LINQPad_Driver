using DwC_A.Meta;
using LINQPad.Extensibility.DataContext;
using System.Collections.Generic;

namespace DwC_A_Driver
{
    class LINQPadSchemaGenerator
    {
        private readonly bool capitalize;

        public LINQPadSchemaGenerator(bool capitalize = false)
        {
            this.capitalize = capitalize;
        }

        public List<ExplorerItem> GenerateSchema(string archiveFileName, 
            IFileMetaData coreFileMetaData, 
            IEnumerable<IFileMetaData> extensionFileMetaData)
        {
            List<ExplorerItem> explorerItems = new List<ExplorerItem>();
            var explorerItem = new ExplorerItem(archiveFileName, ExplorerItemKind.Schema,
                ExplorerIcon.LinkedDatabase)
            {
                Children = new List<ExplorerItem>()
            };

            explorerItem.Children.Add(GetFileReaderItem(coreFileMetaData, true));
            foreach(var extension in extensionFileMetaData)
            {
                explorerItem.Children.Add(GetFileReaderItem(extension, false));
            }
            explorerItems.Add(explorerItem);
            return explorerItems;
        }

        private ExplorerItem GetFileReaderItem(IFileMetaData fileMetaData, bool isCoreFile)
        {
            var explorerIcon = isCoreFile ? ExplorerIcon.View : ExplorerIcon.Table;
            var explorerItem = new ExplorerItem(CodeDomUtils.ExtractClassName(fileMetaData.FileName, capitalize),
                ExplorerItemKind.QueryableObject, explorerIcon)
            {
                Children = new List<ExplorerItem>(),
                IsEnumerable = true
            };
            foreach(var field in fileMetaData.Fields)
            {
                var icon = GetFieldIcon(fileMetaData, field);
                var fieldItem = new ExplorerItem(CodeDomUtils.ModifyKeywords(field.Term, capitalize),
                    ExplorerItemKind.Property, icon);
                explorerItem.Children.Add(fieldItem);
            }
            return explorerItem;
        }

        private ExplorerIcon GetFieldIcon(IFileMetaData fileMetaData, FieldType field)
        {
            if (fileMetaData.Id.IndexSpecified && fileMetaData.Id.Index == field.Index)
            {
                return ExplorerIcon.Key;
            }
            return ExplorerIcon.Column;
        }
    }
}
