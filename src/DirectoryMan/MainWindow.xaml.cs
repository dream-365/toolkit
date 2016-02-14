using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DirectoryMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public UserInteractionVM UserInteraction { get; set; }

        public ObservableCollection<PreviewItemVM> PreviewItems { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            UserInteraction = new UserInteractionVM();

            PreviewItems = new ObservableCollection<PreviewItemVM>();

            VisualStateManager.GoToElementState(stage, "oinit", true);

            UserInteraction.SelectFolder = true;

            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToElementState(stage, "preview", true);

            PreviewItems.Clear();

            var service = new FileStorageService();

            if(UserInteraction.SelectFile)
            {
                var files = service.QueryFiles(UserInteraction.RootDirectory, UserInteraction.Regex);

                foreach (var file in files)
                {
                    var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(file);

                    var reanmeTo = service.ReanmePreview(fileNameWithoutExtension, UserInteraction.Regex, UserInteraction.TargetExpression);

                    PreviewItems.Add(new PreviewItemVM { Name = fileNameWithoutExtension, Action = "重命名[文件]", Result = reanmeTo });
                }
            }


            if(UserInteraction.SelectFolder)
            {
                var folders = service.QueryFolders(UserInteraction.RootDirectory, UserInteraction.Regex);

                foreach(var folder in folders)
                {
                    var reanmeTo = service.ReanmePreview(folder, UserInteraction.Regex, UserInteraction.TargetExpression);

                    PreviewItems.Add(new PreviewItemVM { Name = folder, Action = "重命名[文件夹]", Result = reanmeTo });
                }
            } 
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToElementState(stage, "oinit", true);

            PreviewItems.Clear();
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToElementState(stage, "oinit", true);

            PreviewItems.Clear();

            var service = new FileStorageService();

            if (UserInteraction.SelectFile)
            {
                var files = service.QueryFiles(UserInteraction.RootDirectory, UserInteraction.Regex);

                foreach (var file in files)
                {
                    service.RenameFile(System.IO.Path.Combine(UserInteraction.RootDirectory, file), UserInteraction.Regex, UserInteraction.TargetExpression);
                }
            }


            if (UserInteraction.SelectFolder)
            {
                var folders = service.QueryFolders(UserInteraction.RootDirectory, UserInteraction.Regex);

                foreach (var folder in folders)
                {
                    service.RenameFolder(System.IO.Path.Combine(UserInteraction.RootDirectory, folder), UserInteraction.Regex, UserInteraction.TargetExpression);
                }
            }


        }
    }
}
