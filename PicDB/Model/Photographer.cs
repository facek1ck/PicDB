using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.Model
{
    class Photographer
    {
        public int ID { get; private set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime? Birthday { get; set; }
        public List<Picture> Pictures { get; }
    }
}
