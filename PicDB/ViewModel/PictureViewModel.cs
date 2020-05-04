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
using Serilog;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Win32;
using System.Drawing;
using Image = System.Windows.Controls.Image;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;

namespace PicDB.ViewModel
{
    public class PictureViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ViewImage> _listBoxThumbnails =  new ObservableCollection<ViewImage>();
        DALDatabase database;
        public PictureViewModel(DALDatabase database)
        {
            this.database = database;
            Images = new List<Picture>();
            InitPictures();
        }

        private void InitPictures()
        {
            Log.Information("[Picture Data] - Picture Data Initialization...");
            //string filepath = "C:/Users/gashe/Pictures/temp/";
            //DirectoryInfo directoryInfo = new DirectoryInfo(filepath);

            FillImagesList(/*directoryInfo*/);
            CreateThumbnails();

            if (ListBoxThumbnails.Count() > 0)
            {
                SelectedThumbnail = (ViewImage)ListBoxThumbnails[0];
                SelectedImageSource = FromBase64(Images[0].Image);
            }
        }

        private void CreateThumbnails()
        {
            ObservableCollection<ViewImage> tmp = new ObservableCollection<ViewImage>();
            ThumbnailCount = Images.Count;
            for (int i = 0; i < ThumbnailCount; i++)
            {
                ViewImage viewImage = CreateViewImage(Images[i]);
                tmp.Add(viewImage);
            }
            ListBoxThumbnails = tmp;
        }

        private ViewImage CreateViewImage(Picture p)
        {
            Image image = new Image() { Source = FromBase64(p.Image), Stretch = Stretch.Uniform };
            ViewImage viewImage = new ViewImage();
            viewImage.Picture = p;
            viewImage.Image = image.Source;
            return viewImage;
        }

        private BitmapImage FromBase64(string base64)
        {
            using (var stream = new MemoryStream(Convert.FromBase64String(base64)))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }

        private void FillImagesList()
        {
            Images = database.GetAllPictures();
        }

        public IList<Picture> Images;

        public ImageSource _selectedImageSource;
        public ImageSource SelectedImageSource 
        { 
            get 
            { 
                return _selectedImageSource; 
            } 
            set
            {
                _selectedImageSource = value;
                OnPropertyChanged("SelectedImageSource");
            } 
        }

        public ViewImage _selectedThumbnail = new ViewImage();
        public ViewImage SelectedThumbnail { 
            get 
            {
                return _selectedThumbnail;
            } 
            set 
            {
                if (value != null)
                {
                    _selectedThumbnail = value;
                    SelectedImageSource = _selectedThumbnail.Image;
                    ExifProps = _selectedThumbnail.Picture.ExifProperties;
                    Changed = _selectedThumbnail.Picture.Changed;
                    OnPropertyChanged("SelectedImageSource");
                    OnPropertyChanged("CurrentValue");
                    OnPropertyChanged("CurrentComment");
                    OnPropertyChanged("ExifProps");
                    OnPropertyChanged("Changed");
                }
                else
                {
                    SelectedImageSource = null;
                    SelectedExifProp = null;
                    ExifProps = null;
                    CurrentValue = "";
                    CurrentComment = "";
                    OnPropertyChanged("ExifProps");
                }

            }
        }

        public ObservableCollection<ViewImage> ListBoxThumbnails 
        { 
            get 
            {
                return _listBoxThumbnails; 
            } 
            set 
            {
                _listBoxThumbnails = value;
            } 
        }

        public int ThumbnailCount { get; set; }

        private string _currentValue = "", _currentComment = "";
        public string CurrentValue {
            get
            {
                return _currentValue;
            }
            set
            {
                if (_selectedThumbnail.Picture != null)
                {


                    _currentValue = value;
                    foreach (ExifProperty prop in _selectedThumbnail.Picture.ExifProperties)
                    {
                        if (prop.ID == _selectedExifProp.ID)
                        {
                            prop.Value = _currentValue;
                            if (!prop.Changed)
                            {
                                prop.Changed = true;
                            }
                        }
                    }
                    _selectedThumbnail.Picture.Changed = true;
                    Changed = true;
                    OnPropertyChanged("CurrentValue");
                    OnPropertyChanged("Changed");
                }
            }
        }

        public string CurrentComment {
            get
            {
                return _currentComment;
            }
            set
            {
                if (_selectedThumbnail.Picture != null)
                {
                    _currentComment = value;
                    foreach (ExifProperty prop in _selectedThumbnail.Picture.ExifProperties)
                    {
                        if (prop.ID == _selectedExifProp.ID)
                        {
                            prop.Comment = _currentComment;
                            if (!prop.Changed)
                            {
                                prop.Changed = true;
                            }
                        }
                    }
                    _selectedThumbnail.Picture.Changed = true;
                    Changed = true;
                    OnPropertyChanged("CurrentComment");
                    OnPropertyChanged("Changed");
                }
            }
        }

        public IList<ExifProperty> ExifProps { get; set; }

        private ExifProperty _selectedExifProp = new ExifProperty();
        public ExifProperty SelectedExifProp {
            get 
            { 
                return _selectedExifProp; 
            }
            set 
            {
                if(value != null)
                {
                _selectedExifProp = value;
                _currentValue = SelectedExifProp.Value;
                _currentComment = SelectedExifProp.Comment;
                OnPropertyChanged("CurrentValue");
                OnPropertyChanged("CurrentComment");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public void AddNewPicture()
        {
            Picture p = new Picture();
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files(*.JPG)|*.JPG;";
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;
            var result = dialog.ShowDialog();
            if (result == false)
            {
                return;
            }
                Bitmap image = new Bitmap(dialog.FileName);
                p.Path = dialog.FileName;
                p.Name = dialog.FileName.Substring(dialog.FileName.LastIndexOf('\\') + 1);
                byte[] imageArray = File.ReadAllBytes(dialog.FileName);
                p.Image = Convert.ToBase64String(imageArray);
                p.ExifProperties = GetExifPropsForNewPicture(p);
            
                database.SavePicture(p);
                Images.Add(p);
                CreateThumbnails();
                OnPropertyChanged("ThumbnailCount");
                OnPropertyChanged("ListBoxThumbnails");
            
            

        }

        private IList<ExifProperty> GetExifPropsForNewPicture(Picture p)
        {
            IList<ExifProperty> props = new List<ExifProperty>();
            var directories = ImageMetadataReader.ReadMetadata(p.Path);

            foreach (var directory in directories)
            {
                if (directory
                    .Is<ExifIfd0Directory>()
                    .Or<ExifImageDirectory>()
                    .Or<ExifInteropDirectory>()
                    .Or<ExifSubIfdDirectory>()
                    .Or<ExifThumbnailDirectory>())
                {
                    foreach (var tag in directory.Tags)
                    {
                        ExifProperty prop = new ExifProperty();
                        prop.Name = tag.Name;
                        prop.TagNumber = tag.Type;
                        prop.Value = tag.Description;
                        props.Add(prop);
                    }

                    if (directory.HasError)
                    {
                        foreach (var error in directory.Errors)
                            Console.WriteLine($"ERROR: {error}");
                    }
                }
            }
            props.OrderBy(x => x.Name);
            return props;
        }

        public void SavePropertiesForPicture()
        {
            Picture p = SelectedThumbnail.Picture;
            database.UpdatePicture(p);
            SelectedThumbnail.Picture.Changed = false;
            Changed = false;
            OnPropertyChanged("Changed");
        }

        public void DeleteCurrentPicture()
        {
            Picture p = SelectedThumbnail.Picture;
            if(p != null)
            {
                database.DeletePictureById(p.ID);
            }
            Images.Remove(p);
            CreateThumbnails();
            OnPropertyChanged("ThumbnailCount");
            OnPropertyChanged("ListBoxThumbnails");
        }

        public bool Changed { get; set; }
    }
}