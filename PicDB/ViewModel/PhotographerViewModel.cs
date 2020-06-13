using PicDB.DataAccess;
using PicDB.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PicDB.ViewModel
{
    public class PhotographerViewModel
    {
        private ObservableCollection<Photographer> _photographers; 
        public PhotographerViewModel(DALDatabase database)
        {
            Photographers = new ObservableCollection<Photographer>(database.GetAllPhotographers());
        }

        public ObservableCollection<Photographer> Photographers { get { return _photographers; } set { _photographers = value; } }


        bool isValid
        {
            get
            {
                return false;
               // return ID != null && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName);
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
