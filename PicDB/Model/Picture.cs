using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.Model
{
    class Picture
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IList<ExifProperty> ExifProperties { get; set; }

        //public SortedList<string, string> IptcTags{ get; set; }
    }
}
