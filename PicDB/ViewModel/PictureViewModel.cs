using PicDB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ExifLib;
using System.Windows.Data;

namespace PicDB.ViewModel
{
    class PictureViewModel : INotifyPropertyChanged
    {
        public PictureViewModel()
        {
            ExifProperties = new List<ExifProperty>();
            foreach (ExifTags t in Enum.GetValues(typeof(ExifTags)))
            {
                ExifProperty exifProperty = new ExifProperty();
                exifProperty.ID = (int)t;
                exifProperty.Name = t.ToString();
                ExifProperties.Add(exifProperty);
            }
            ExifProps = new CollectionView(ExifProperties);
            ExifProps.MoveCurrentTo(ExifProperties[0]);
           ExifProps.CurrentChanged += new EventHandler(props_CurrentChanged);
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public IList<ExifProperty> ExifProperties { get; set; }

        private string _currentValue="", _currentComment="";
        public string CurrentValue { 
            get
            {
                return _currentValue;
            }
            set
            {
                _currentValue = value;
                ((ExifProperty)ExifProps.CurrentItem).Value = _currentValue;
                OnPropertyChanged("CurrentValue");
            }
        }

        public string CurrentComment {
            get
            {
                return _currentComment;
            }
            set
            {
                _currentComment = value;
                ((ExifProperty)ExifProps.CurrentItem).Comment = value;
                OnPropertyChanged("CurrentComment");
            }
        }

        public CollectionView ExifProps { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        void props_CurrentChanged(object sender, EventArgs e)
        {
            CurrentValue = ((ExifProperty)ExifProps.CurrentItem).Value;
            CurrentComment = ((ExifProperty)ExifProps.CurrentItem).Comment;
        }

    }
}