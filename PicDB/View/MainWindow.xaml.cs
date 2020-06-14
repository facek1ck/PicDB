using System.Windows;
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
    }
}
