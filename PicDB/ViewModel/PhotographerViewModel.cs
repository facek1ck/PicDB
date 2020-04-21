using PicDB.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.ViewModel
{
    class PhotographerViewModel
    {
        public int ID { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime? Birthday { get; set; }

        public List<Picture> Pictures { get; }

        public bool ShowBirthday { get { return Birthday.HasValue; } }

        public int NumberOfPictures
        {
            get
            {
                return Pictures.Count;
            }
        }

        bool isValid { get; }
        string ValidationSummary { get; }
    }
}
