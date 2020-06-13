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

        public MainWindow(MainWindowViewModel mainWindowViewModel, PhotographerWindowViewModel photographerWindowViewModel)
        {
            DataContext = mainWindowViewModel;
            _mainWindowViewModel = mainWindowViewModel;
            _photographerWindowViewModel = photographerWindowViewModel;
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

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.PictureViewModel.SavePropertiesForPicture();
        }
    }
}
