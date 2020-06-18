using System.Configuration;
using System.Diagnostics;
using System.Windows;
using PicDB.Helpers;
using PicDB.View;
using PicDB.ViewModel;
using Serilog;

namespace PicDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel _mainWindowViewModel;
        PhotographerWindowViewModel _photographerWindowViewModel;
        PhotographerViewModel _photographerViewModel;

        public MainWindow(MainWindowViewModel mainWindowViewModel, PhotographerWindowViewModel photographerWindowViewModel, PhotographerViewModel photographerViewModel)
        {
            DataContext = mainWindowViewModel;
            _mainWindowViewModel = mainWindowViewModel;
            _photographerWindowViewModel = photographerWindowViewModel;
            _photographerViewModel = photographerViewModel;
            InitializeComponent(); 
        }

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.PictureViewModel.AddNewPicture();
        }

        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.PictureViewModel.DeleteCurrentPicture();
        }

        private void MenuItemShowPhotographers_Click(object sender, RoutedEventArgs e)
        {
            var popup = new PhotographerWindow(_photographerWindowViewModel);
            popup.ShowDialog();
        }
        
        private void MenuItemAddPhotographer_Click(object sender, RoutedEventArgs e)
        {
            var popup = new PhotographerAddWindow(_photographerViewModel, false);
            popup.ShowDialog();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.PictureViewModel.SavePropertiesForPicture();
        }

        private void MenuItemAssignPhotographer_Click(object sender, RoutedEventArgs e)
        {
            var popup = new AssignPhotographerWindow(_mainWindowViewModel, _photographerViewModel, _mainWindowViewModel.PictureViewModel.SelectedThumbnail.Picture);
            popup.ShowDialog();
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MenuItemPictureReport_Click(object sender, RoutedEventArgs e)
        {
            Reporting.printPicture(_mainWindowViewModel.PictureViewModel.SelectedThumbnail.Picture);
        }

        private void MenuItemTagReport_Click(object sender, RoutedEventArgs e)
        {
            Reporting.printTags(_mainWindowViewModel.PictureViewModel);
        }

        private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ConfigurationManager.AppSettings["documentation:location"]);
        }
    }
}
