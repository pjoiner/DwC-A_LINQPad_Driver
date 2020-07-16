using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;

namespace DwC_A_Driver
{
    class FolderViewModel : INotifyPropertyChanged
    {
        const string FileStr = "File...";
        const string FolderStr = "Folder...";

        private FolderParams folderParams = new FolderParams();
        private string buttonText = "Folder...";

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        public bool Folder
        {
            get
            {
                return folderParams.Folder;
            }
            set
            {
                ButtonText = value ? FolderStr : FileStr;
                folderParams.Folder = value;
                OnPropertyChanged("Folder");
            }
        }

        public string Path
        {
            get
            {
                return folderParams.Path;
            }
            set
            {
                folderParams.Path = value;
                OnPropertyChanged("Path");
                OnPropertyChanged("IsPathNotEmpty");
            }
        }

        public string ButtonText
        {
            get
            {
                return buttonText;
            }
            set
            {
                buttonText = value;
                OnPropertyChanged("ButtonText");
            }
        }

        public bool IsPathNotEmpty
        {
            get
            {
                return !string.IsNullOrEmpty(Path);
            }
        }

        public ICommand BrowseCommand { get; set; }

        public FolderViewModel()
        {
            BrowseCommand = new RelayCommand(new Action<object>(Browse));
        }

        public void Browse(object obj)
        {
            if (folderParams.Folder)
            {
                BrowseFolder();
            }
            else
            {
                BrowseArchive();
            }
        }

        private void BrowseFolder()
        {
            using (var folderBrowser = new FolderBrowserDialog())
            {
                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Path = folderBrowser.SelectedPath;
                }
            }
        }

        private void BrowseArchive()
        {
            using (var fileDialog = new OpenFileDialog())
            {
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Path = fileDialog.FileName;
                }
            }
        }

    }
}
