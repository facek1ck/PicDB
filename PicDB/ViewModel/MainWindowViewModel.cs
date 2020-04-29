using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.ViewModel
{
    public class MainWindowViewModel
    {
        PictureViewModel _pictureViewModel;
        public MainWindowViewModel(PictureViewModel pictureViewModel)
        {
            _pictureViewModel = pictureViewModel;
        }

        public PictureViewModel PictureViewModel { get { return _pictureViewModel; } }
    }
}
