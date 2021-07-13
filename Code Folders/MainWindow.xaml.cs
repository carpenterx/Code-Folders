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
        private static readonly string APPLICATION_FOLDER = "Code Folders";
        private static readonly string APPLICATION_DATA_FILE = "data.json";
        private static readonly string README_FILE = "readme.md";

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
                IsFolderPicker = true,
                Multiselect = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (var fileName in dialog.FileNames)
                {
                    foldersList.Add(new CodeFolder { Path = fileName });
                }
            }
        }

        private void OpenFolderClick(object sender, RoutedEventArgs e)
        {
            if(foldersListView.SelectedItem != null)
            {
                CodeFolder selectedFolder = (CodeFolder)foldersListView.SelectedItem;
                OpenFolderPathInExplorer(selectedFolder.Path);
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
            if (item != null)
            {
                CodeFolder selectedFolder = (CodeFolder)item.DataContext;
                string selectedPath = selectedFolder.Path;
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    
                    OpenFolderPathInExplorer(selectedPath);
                }
                else if(Keyboard.IsKeyDown(Key.LeftShift))
                {
                    Clipboard.SetText(Path.GetFileName(selectedPath));
                }
            }
        }

        private void OpenFolderPathInExplorer(string path)
        {
            Process.Start("explorer.exe", path);
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

        private void AddDroppedFolders(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var path in files)
                {
                    if(Directory.Exists(path))
                    {
                        foldersList.Add(new CodeFolder { Path = path });
                    }
                }
            }
        }

        private void ListViewItemSelected(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView.SelectedItem is CodeFolder selectedFolder)
            {
                string readmePath = Path.Combine(selectedFolder.Path, README_FILE);
                if (File.Exists(readmePath))
                {
                    readmeTxt.Text = File.ReadAllText(readmePath);
                }
                else
                {
                    readmeTxt.Text = selectedFolder.Path;
                }
            }
        }

        private void OpenGithubLocalClick(object sender, RoutedEventArgs e)
        {
            if (foldersListView.SelectedItem != null)
            {
                CodeFolder selectedFolder = (CodeFolder)foldersListView.SelectedItem;
                string githubLocalPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"GitHubDesktop\bin\", "github.bat");
                RunProcess(githubLocalPath, selectedFolder.Path);
            }
        }

        private void RunProcess(string exePath, string filePath)
        {
            string directory = exePath;
            string folderName = filePath;
            if (folderName.Contains(" "))
            {
                folderName = $"\"{folderName}\"";
            }

            try
            {
                var psi = new ProcessStartInfo();
                psi.FileName = directory;
                psi.Arguments = folderName;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;

                using var process = Process.Start(psi);

                //using StreamReader reader = process.StandardOutput;
                //string data = reader.ReadToEnd();
                //File.WriteAllText(@"C:\Users\jorda\Desktop\output.txt", data);

                process.WaitForExit();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
