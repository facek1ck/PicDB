using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.Model
{
    public class Photographer
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public DateTime? Birthday { get; set; }
        public String Notes { get; set; }
        public override string ToString()
        {
            return LastName + " " + FirstName;
        }
    }
}
