using ExifLib;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Iptc;
using Microsoft.Win32;
using PicDB.DataAccess;
using PicDB.Model;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace PicDB.ViewModel
{
    public class PictureViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ViewImage> _listBoxThumbnails = new ObservableCollection<ViewImage>();
        DALDatabase database;
        public PictureViewModel(DALDatabase database)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.AppSettings().CreateLogger();
            Log.Information("Starting Application...");
            this.database = database;
            Images = new List<Picture>();
            InitPictures();
        }

        private void InitPictures()
        {
            Log.Information("[Picture Data] - Picture Data Initialization...");

            FillImagesList();
            CreateThumbnails(Images);
            Log.Information("[Picture Data] - Initialization complete...");

            if (ListBoxThumbnails.Count() > 0)
            {
                SelectedThumbnail = (ViewImage)ListBoxThumbnails[0];
                SelectedImageSource = FromBase64(Images[0].Image);
            }
            

        }

        private void CreateThumbnails(IList<Picture> pictures)
        {
            ObservableCollection<ViewImage> tmp = new ObservableCollection<ViewImage>();
            ThumbnailCount = pictures.Count;
            for (int i = 0; i < ThumbnailCount; i++)
            {
                ViewImage viewImage = CreateViewImage(pictures[i]);
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
            Log.Information("[Picture Data] - Fetching Data from DB...");
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
        public ViewImage SelectedThumbnail
        {
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
                    if(SelectedTab == 0)
                    {
                        Props = _selectedThumbnail.Picture.ExifProperties;
                    }
                    else
                    {
                        Props = _selectedThumbnail.Picture.IptcProperties;

                    }

                    Changed = _selectedThumbnail.Picture.Changed;
                    OnPropertyChanged("SelectedImageSource");
                    OnPropertyChanged("CurrentValue");
                    OnPropertyChanged("CurrentComment");
                    OnPropertyChanged("Props");
                    OnPropertyChanged("Changed");
                }
                else
                {
                    SelectedImageSource = null;
                    SelectedProp = null;
                    Props = null;
                    CurrentValue = "";
                    CurrentComment = "";
                    OnPropertyChanged("Props");
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
        public string CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                if (_selectedThumbnail.Picture != null)
                {


                    _currentValue = value;
                    foreach (Property prop in _selectedThumbnail.Picture.ExifProperties)
                    {
                        if (prop.ID == _selectedProp.ID)
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

        public string CurrentComment
        {
            get
            {
                return _currentComment;
            }
            set
            {
                if (_selectedThumbnail.Picture != null)
                {
                    _currentComment = value;
                    foreach (Property prop in _selectedThumbnail.Picture.ExifProperties)
                    {
                        if (prop.ID == _selectedProp.ID)
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

        private IList<Property> _props = new List<Property>();
        public IList<Property> Props { get; set; }

        private Property _selectedProp = new Property();
        public Property SelectedProp
        {
            get
            {
                return _selectedProp;
            }
            set
            {
                if (value != null)
                {
                    _selectedProp = value;
                    CurrentValue = SelectedProp.Value;
                    CurrentComment = SelectedProp.Comment;
                    OnPropertyChanged("CurrentValue");
                    OnPropertyChanged("CurrentComment");
                }
                else
                {
                    CurrentValue = "";
                    CurrentComment = "";
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
            p.IptcProperties = GetIptcPropsForNewPicture(p);

            database.SavePicture(p);
            Images.Add(p);
            CreateThumbnails(Images);
            OnPropertyChanged("ThumbnailCount");
            OnPropertyChanged("ListBoxThumbnails");



        }

        private IList<Property> GetExifPropsForNewPicture(Picture p)
        {
            IList<Property> props = new List<Property>();
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
                        Property prop = new Property();
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

        private IList<Property> GetIptcPropsForNewPicture(Picture p)
        {
            IList<Property> props = new List<Property>();
            var directories = ImageMetadataReader.ReadMetadata(p.Path);

            foreach (var directory in directories)
            {
                if (directory.Is<IptcDirectory>())
                {
                    foreach (var tag in directory.Tags)
                    {
                        Property prop = new Property();
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
            if (p != null)
            {
                database.DeletePictureById(p.ID);
            }
            Images.Remove(p);
            CreateThumbnails(Images);
            OnPropertyChanged("ThumbnailCount");
            OnPropertyChanged("ListBoxThumbnails");
        }

        public bool Changed { get; set; }

        private int _selectedTab = 0;
        public int SelectedTab
        {
            get
            {
                return _selectedTab;
            }
            set
            {
                _selectedTab = value;
                if(value == 0)
                {
                    Props = _selectedThumbnail.Picture.ExifProperties;
                    //CurrentValue = _selectedProp.Value;
                    //CurrentComment = _selectedProp.Comment;
                }
                else
                {
                    Props = _selectedThumbnail.Picture.IptcProperties;
                    //CurrentValue = _selectedProp.Value;
                    //CurrentComment = _selectedProp.Comment;
                }

                if (Props.Count == 0)
                {
                    SelectedProp = null;
                }

                OnPropertyChanged("Props");
                OnPropertyChanged("SelectedProp");
                OnPropertyChanged("CurrentValue");
                OnPropertyChanged("CurrentComment");
                OnPropertyChanged("Changed");
            }
        }
        private string _searchString = "";
        public string SearchString
        {
            get
            {
                return _searchString;
            }
            set
            {
                _searchString = value;
                if (!String.IsNullOrEmpty(_searchString))
                {
                    CreateThumbnails(FilterImages(_searchString));
                    OnPropertyChanged("ThumbnailCount");
                    OnPropertyChanged("ListBoxThumbnails");
                }
                else
                {
                    CreateThumbnails(Images);
                    OnPropertyChanged("ThumbnailCount");
                    OnPropertyChanged("ListBoxThumbnails");
                }
            }
        }

        private IList<Picture> FilterImages(string searchString)
        {
            searchString = searchString.ToUpper();
            //return Images.Where(p => /*p.Photographer != null && (
            //                        p.Photographer.FirstName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 ||
            //                        p.Photographer.LastName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 ||*/
            //                        p.ExifProperties.All(tag => tag.Value.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 ||
            //                                                    tag.Comment.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
            //                                            ) ||
            //                        p.IptcProperties.All(tag => tag.Value.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0 ||
            //                                                    tag.Comment.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0
            //                                            )
            //                        ).ToList();


            return Images.Where( i => i.ExifProperties.Any(z => z.Value.ToUpper().Contains(searchString))
            ).ToList();

        }
    }
}