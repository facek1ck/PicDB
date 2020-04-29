using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.Model
{
    class Photographer
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public DateTime? Birthday { get; set; }
    }
}
