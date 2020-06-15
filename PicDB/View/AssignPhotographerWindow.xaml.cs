using PicDB.Model;
using PicDB.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PicDB.View
{
    public partial class AssignPhotographerWindow : Window
    {
        MainWindowViewModel _mainWindowViewModel;
        PhotographerViewModel _photographerViewModel;
        Picture _pic;
        public AssignPhotographerWindow(MainWindowViewModel mainWindowViewModel, PhotographerViewModel photographerViewModel, Picture pic)
        {
            _mainWindowViewModel = mainWindowViewModel;
            _photographerViewModel = photographerViewModel;
            _pic = pic;
            DataContext = photographerViewModel;
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            _photographerViewModel.AssignPhotographer(_pic);
            _mainWindowViewModel.PictureViewModel.reloadImages();
            _photographerViewModel.clearCurrentPhotographer();
            this.Close();
        }

        private void btnDialogCancel_Click(object sender, RoutedEventArgs e)
        {
            _photographerViewModel.clearCurrentPhotographer();
        }
    }
}
