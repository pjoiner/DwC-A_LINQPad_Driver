using System.Windows;

namespace DwC_A_Driver
{
    /// <summary>
    /// Interaction logic for FolderWindow.xaml
    /// </summary>
    public partial class FolderWindow : Window
    {
        public FolderWindow()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
