using DwC_A;
using DwC_A.Terms;
using LINQPad.Extensibility.DataContext;
using System.Collections.Generic;
using System.IO;

namespace DwC_A_Driver
{
    class LinQPadSchemaGenerator
    {
        public List<ExplorerItem> GenerateSchema(string fileName)
        {
            List<ExplorerItem> explorerItems = new List<ExplorerItem>();
            var explorerItem = new ExplorerItem(fileName, ExplorerItemKind.Schema,
                ExplorerIcon.LinkedDatabase)
            {
                Children = new List<ExplorerItem>()
            };

            using (var archive = new ArchiveReader(fileName))
            {
                explorerItem.Children.Add(GetFileReaderItem(archive.CoreFile));
                foreach(var extension in archive.Extensions)
                {
                    explorerItem.Children.Add(GetFileReaderItem(extension));
                }
            }
            explorerItems.Add(explorerItem);
            return explorerItems;
        }

        ExplorerItem GetFileReaderItem(IFileReader fileReader)
        {
            var explorerItem = new ExplorerItem(Path.GetFileNameWithoutExtension(fileReader.FileName),
                ExplorerItemKind.QueryableObject, ExplorerIcon.Table)
            {
                Children = new List<ExplorerItem>(),
                IsEnumerable = true
            };
            foreach(var field in fileReader.FileMetaData.Fields)
            {
                var fieldItem = new ExplorerItem(Terms.ShortName(field.Term),
                    ExplorerItemKind.Property, ExplorerIcon.Column);
                explorerItem.Children.Add(fieldItem);
            }
            return explorerItem;
        }
    }
}
