using GalaSoft.MvvmLight.Command;
using PicDB.DataAccess;
using PicDB.Model;
using PicDB.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace PicDB.ViewModel
{
    public class PhotographerViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Photographer> _photographers;
        DALDatabase database;
        public PhotographerViewModel(DALDatabase database)
        {
            this.database = database;
            Photographers = new ObservableCollection<Photographer>(database.GetAllPhotographers());
        }


        public ObservableCollection<Photographer> Photographers { get { return _photographers; } set { _photographers = value; } }

        private Photographer _currentPhotographer = new Photographer();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }


        public Photographer CurrentPhotographer { get { return _currentPhotographer;} set { _currentPhotographer = value; } }

        public string CurrentPhotographerFirstName 
        {
            get
            {
                return _currentPhotographer.FirstName;
            }

            set
            {
                _currentPhotographer.FirstName = value;
            }
        }
        public string CurrentPhotographerLastName 
        {
            get
            {
                return _currentPhotographer.LastName;
            }

            set
            {
                _currentPhotographer.LastName = value;
            }
        }
        public DateTime? CurrentPhotographerBirthday
        {
            get
            {
                return _currentPhotographer.Birthday;
            }

            set
            {
                _currentPhotographer.Birthday = value;
            }
        }

 

        public void AddPhotographer()
        {
            if (isValid)
            {
                database.SavePhotographer(_currentPhotographer);
                Photographers = new ObservableCollection<Photographer>(database.GetAllPhotographers());
                OnPropertyChanged("Photographers");
                _currentPhotographer = new Photographer();
            }
            else
            {
                System.Windows.MessageBox.Show(ValidationSummary);
            }
            
        }

        public void EditPhotographer()
        {
            if (isValid)
            {
                database.UpdatePhotographer(_currentPhotographer);
                Photographers = new ObservableCollection<Photographer>(database.GetAllPhotographers());
                OnPropertyChanged("Photographers");
                _currentPhotographer = new Photographer();
            }
            else
            {
                System.Windows.MessageBox.Show(ValidationSummary);
            }

        }

        public void DeletePhotographer()
        {
            database.DeletePhotographerById(_currentPhotographer.ID);
            Photographers = new ObservableCollection<Photographer>(database.GetAllPhotographers());
            OnPropertyChanged("Photographers");
            _currentPhotographer = new Photographer();

        }

        public void clearCurrentPhotographer()
        {
            _currentPhotographer = new Photographer();
        }

        public void updateList()
        {
            Photographers = new ObservableCollection<Photographer>(database.GetAllPhotographers());
        }

        bool isValid
        {
            get
            {
                return !string.IsNullOrEmpty(_currentPhotographer.LastName) && _currentPhotographer.Birthday < DateTime.Now; ;
            }
        }
         string ValidationSummary 
        { 
            get
            {
                if (isValid)
                {
                    return "Photographer is valid!";
                }
                return "Photographer is not valid!";
            }
        }
    }
}
