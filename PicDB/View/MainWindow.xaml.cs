using System.Windows;

using PicDB.ViewModel;

namespace PicDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private PictureViewModel pictureViewModel = new PictureViewModel();
        public MainWindow()
        {
            DataContext = pictureViewModel;
            InitializeComponent();
            
        }


    }
}
