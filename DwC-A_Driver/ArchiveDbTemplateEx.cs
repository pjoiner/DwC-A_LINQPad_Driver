using DwC_A;
using System.IO;

namespace DwC_A_Driver
{
    public partial class ArchiveDbTemplate
    {
        public readonly ArchiveReader archive;

        public ArchiveDbTemplate(ArchiveReader archive)
        {
            this.archive = archive;
        }

        public string CoreFileName()
        {
            return Path.GetFileNameWithoutExtension(archive.CoreFile.FileMetaData.FileName);
        }
    }
}
