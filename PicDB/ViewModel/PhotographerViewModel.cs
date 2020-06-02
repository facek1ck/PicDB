using PicDB.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.ViewModel
{
    class PhotographerViewModel
    {
        public PhotographerViewModel(Photographer photographer)
        {
            _photographer = photographer;
        }

        private Photographer _photographer { get; set; }
        public Guid ID { get { return _photographer.ID; } }
        public string FirstName { get { return _photographer.FirstName; } }
        public string LastName { get { return _photographer.LastName; } }

        public DateTime? Birthday { get { return _photographer.Birthday; } }

        public bool ShowBirthday { get { return Birthday.HasValue; } }


        bool isValid
        {
            get
            {
                return ID != null && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName);
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
