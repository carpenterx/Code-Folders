using Code_Folders.Models;
using System.Collections.Generic;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;

namespace Code_Folders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<CodeFolder> foldersList = new();

        public MainWindow()
        {
            InitializeComponent();

            foldersList.Add( new CodeFolder { Path = @"C:\Users\jorda\source\repos\Code Folders" } );
            foldersList.Add( new CodeFolder { Path = @"C:\Users\jorda\source\repos" } );

            foldersListView.ItemsSource = foldersList;
        }

        private void BrowseToFolderClick(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new()
            {
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foldersList.Add(new CodeFolder { Path = dialog.FileName });
            }
        }
    }
}
