using LinqToDB.SqlQuery;
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
    /// <summary>
    /// Interaktionslogik für PhotographerWindow.xaml
    /// </summary>
    public partial class PhotographerWindow : Window
    {
        PhotographerWindowViewModel _photographerWindowViewModel;
        public PhotographerWindow(PhotographerWindowViewModel photographerWindowViewModel)
        {
            _photographerWindowViewModel = photographerWindowViewModel;
            DataContext = photographerWindowViewModel;
            _photographerWindowViewModel.PhotographerViewModel.clearCurrentPhotographer();
            InitializeComponent();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var popup = new PhotographerAddWindow(_photographerWindowViewModel.PhotographerViewModel, true);
            popup.ShowDialog();
            _photographerWindowViewModel.PhotographerViewModel.clearCurrentPhotographer();
            this.Close();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _photographerWindowViewModel.PhotographerViewModel.DeletePhotographer();
            _photographerWindowViewModel.PhotographerViewModel.clearCurrentPhotographer();

        }
    }
}
