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
            InitializeComponent();
        }
    }
}
