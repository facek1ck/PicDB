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
    /// Interaktionslogik für PhotographerAddWindow.xaml
    /// </summary>
    public partial class PhotographerAddWindow : Window
    {
        PhotographerViewModel _photographerViewModel;
        bool _edit;
        public PhotographerAddWindow(PhotographerViewModel photographerViewModel, bool edit)
        {
            _photographerViewModel = photographerViewModel;
            _edit = edit;
            DataContext = photographerViewModel;
            _photographerViewModel.clearCurrentPhotographer();
            if (edit)
            {
                Title = "Edit Photographer";
            }
            else
            {
                Title = "Add Photographer";
            }
            InitializeComponent();
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            if (!_edit)
            {
                _photographerViewModel.AddPhotographer();
            }
            else
            {
                _photographerViewModel.EditPhotographer();
            }
            
            this.Close();
        }
    }
}
