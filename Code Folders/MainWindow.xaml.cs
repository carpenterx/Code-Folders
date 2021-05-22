using Code_Folders.Models;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using System.IO;
using System;

namespace Code_Folders
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<CodeFolder> foldersList = new();
        private static string APPLICATION_FOLDER = "Code Folders";
        private static string APPLICATION_DATA_FILE = "data.json";

        public MainWindow()
        {
            InitializeComponent();

            LoadFoldersList();

            foldersListView.ItemsSource = foldersList;
        }

        private void LoadFoldersList()
        {
            string dataFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_FOLDER, APPLICATION_DATA_FILE);
            if (File.Exists(dataFilePath))
            {
                foldersList = JsonConvert.DeserializeObject<ObservableCollection<CodeFolder>>(File.ReadAllText(dataFilePath));
            }
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

        private void SaveListOnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string json = JsonConvert.SerializeObject(foldersList, Formatting.Indented);
            string dataFileRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_FOLDER);
            if(!Directory.Exists(dataFileRoot))
            {
                Directory.CreateDirectory(dataFileRoot);
            }
            string dataFilePath = Path.Combine(dataFileRoot, APPLICATION_DATA_FILE);
            File.WriteAllText(dataFilePath, json);
        }
    }
}
