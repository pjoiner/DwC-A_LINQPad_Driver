using DwC_A;
using DwC_A.Terms;
using System.IO;

namespace DwC_A_Driver
{
    public partial class MyArchiveDbTemplate
    {
        public readonly ArchiveReader archive;

        public MyArchiveDbTemplate(ArchiveReader archive)
        {
            this.archive = archive;
        }

        public string CoreFileName()
        {
            return Path.GetFileNameWithoutExtension(archive.CoreFile.FileMetaData.FileName);
        }
    }
}
