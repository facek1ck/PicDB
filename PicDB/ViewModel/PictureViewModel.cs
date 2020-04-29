using PicDB.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using ExifLib;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media.Imaging;
using PicDB.DataAccess;

namespace PicDB.ViewModel
{
    class PictureViewModel : INotifyPropertyChanged
    {
        public PictureViewModel()
        {
            Images = new List<Picture>();
            ListBoxThumbnails = new DataGrid().Items;
            InitExifData();
            InitPictures();
            //TestMethod();
        }

        private void TestMethod()
        {
            IList<ExifProperty> testProps = new List<ExifProperty>();
            ExifProperty prop1 = new ExifProperty();
            prop1.Name = "testprop";
            prop1.TagNumber = 1234;
            prop1.Value = "testval";
            prop1.Comment = "";
            ExifProperty prop2 = new ExifProperty();
            prop2.Name = "testprop2";
            prop2.TagNumber = 4321;
            prop2.Value = "";
            prop2.Comment = "testComment";
            testProps.Add(prop1);
            testProps.Add(prop2);
            Picture p = new Picture();
            p.Name = "TestName";
            p.Image = "TestImage";
            p.ExifProperties = testProps;

            Photographer photographer1 = new Photographer();
            photographer1.FirstName = "Max";
            photographer1.LastName = "Mustermann";
            photographer1.Birthday = DateTime.Now;
            Photographer photographer2 = new Photographer();
            photographer2.FirstName = "Erika";
            photographer2.LastName = "Mustermann";

            DALDatabase dALDatabase = new DALDatabase();
            // dALDatabase.addPicture(p);
            //dALDatabase.getPictureById(new Guid("09F37028-CA20-4D69-89FD-0C9BCC3A7A88"));
            //dALDatabase.getAllPictures();
            //dALDatabase.SavePhotographer(photographer1);
            //dALDatabase.SavePhotographer(photographer2);
            //dALDatabase.GetAllPhotographers();
            //dALDatabase.GetPhotographerById(new Guid("212839F0-ED3C-43B3-BE5E-446159D94DF1"));
        }

        private void InitPictures()
        {
            string filepath = "C:/Users/gashe/Pictures/temp/";
            DirectoryInfo directoryInfo = new DirectoryInfo(filepath);

            FillImagesList(directoryInfo);

            ThumbnailCount = Images.Count;
            for(int i=0; i< ThumbnailCount; i++)
            {
                Image image = new Image() { Source = new BitmapImage(new Uri(Images[i].Path, UriKind.Absolute)), Stretch = Stretch.Uniform };
                ListBoxThumbnails.Add(image);
            }

            ImageProps = new CollectionView(ListBoxThumbnails);
            ImageProps.MoveCurrentTo(ListBoxThumbnails[0]);
            ImageProps.CurrentChanged += new EventHandler(imageProps_CurrentChanged);

            SelectedImagePath = Images[0].Path;
            SelectedImageSource = new BitmapImage(new Uri(SelectedImagePath, UriKind.Absolute));
        }

        private void FillImagesList(DirectoryInfo directoryInfo)
        {
            int k = 0;
            foreach(var file in directoryInfo.GetFiles("*.jpg"))
            {
                Picture picture = new Picture();
                //picture.ID = k;
                picture.Name = file.Name;
                picture.Path = directoryInfo.FullName + file.Name;
                k++;
                Images.Add(picture);
            }
        }

        private void InitExifData()
        {
            ExifProperties = new List<ExifProperty>();
            foreach (ExifTags t in Enum.GetValues(typeof(ExifTags)))
            {
                ExifProperty exifProperty = new ExifProperty();
                exifProperty.TagNumber = (int)t;
                exifProperty.Name = t.ToString();
                ExifProperties.Add(exifProperty);
            }
            ExifProps = new CollectionView(ExifProperties);
            ExifProps.MoveCurrentTo(ExifProperties[0]);
            ExifProps.CurrentChanged += new EventHandler(props_CurrentChanged);
        }

        public List<Picture> Images;

        public ImageSource _selectedImageSource;
        public ImageSource SelectedImageSource { get { return _selectedImageSource; } set
            {
                _selectedImageSource = value;
                OnPropertyChanged("SelectedImageSource");
            } }

        public string _selectedImagePath = "";
        public string SelectedImagePath
        {get;set;
            //get
            //{
            //    return _selectedImagePath;
            //}
            //set
            //{
            //    _selectedImagePath = value;
            //    ((Picture)ImageProps.CurrentItem).Path = _selectedImagePath;
            //    OnPropertyChanged("SelectedImagePath");
            //    OnPropertyChanged("SelectedImageSource");
            //}
        }
        public CollectionView ImageProps { get; private set; }

        public ItemCollection ListBoxThumbnails { get; set; }

        public int ThumbnailCount { get; set; }

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
        
        void imageProps_CurrentChanged(object sender, EventArgs e)
        {
            SelectedImagePath = (((Image)ImageProps.CurrentItem).Source as BitmapImage).UriSource.OriginalString;
            SelectedImageSource = new BitmapImage(new Uri(SelectedImagePath, UriKind.Absolute));
            
        }



    }
}