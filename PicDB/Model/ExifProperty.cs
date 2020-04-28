using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.Model
{
    class ExifProperty
    {
        public Guid ID { get; set; }
        public int TagNumber { get; set; }
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";
        public string Comment { get; set; } = "";
    }
}
