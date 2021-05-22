using Code_Folders.Models;
using System.Collections.Generic;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

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

        private void OpenFolderClick(object sender, RoutedEventArgs e)
        {
            if(foldersListView.SelectedItem != null)
            {
                CodeFolder selectedFolder = (CodeFolder)foldersListView.SelectedItem;
                OpenFolderPathInExplorer(selectedFolder);
            }
        }

        private void DeleteFolderClick(object sender, RoutedEventArgs e)
        {
            if(foldersListView.SelectedIndex != -1)
            {
                foldersList.RemoveAt(foldersListView.SelectedIndex);
            }
        }

        private void ListViewItemClickPreview(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item != null && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                CodeFolder selectedFolder = (CodeFolder) item.DataContext;
                OpenFolderPathInExplorer(selectedFolder);
            }
        }

        private void OpenFolderPathInExplorer(CodeFolder codeFolder)
        {
            Process.Start("explorer.exe", codeFolder.Path);
        }
    }
}
