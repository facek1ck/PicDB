using System.Windows;

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
        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            Log.Information("Starting Application...");
            DataContext = mainWindowViewModel;
            _mainWindowViewModel = mainWindowViewModel;
            InitializeComponent(); 
        }

        private void MenuItemNew_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.PictureViewModel.AddNewPicture();
        }
    }
}
