using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Win32;
using PicDB.Model;
using System.Drawing;
using MetadataExtractor;
using System.Linq;
using MetadataExtractor.Formats.Exif;

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
