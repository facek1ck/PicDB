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
        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.AppSettings().CreateLogger();
            Log.Information("Starting Application...");
            DataContext = mainWindowViewModel;
            InitializeComponent();
            
        }


    }
}
