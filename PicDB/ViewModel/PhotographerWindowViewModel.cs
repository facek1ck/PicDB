using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.ViewModel
{
    public class PhotographerWindowViewModel
    {
        PhotographerViewModel _photographerViewModel;
        public PhotographerWindowViewModel(PhotographerViewModel photographerViewModel)
        {
            _photographerViewModel = photographerViewModel;
        }

        public PhotographerViewModel PhotographerViewModel { get { return _photographerViewModel; } }
    }
}
