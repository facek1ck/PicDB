



using System;
using System.Collections.Generic;

namespace PicDB.Model
{
    public class Picture
    {
        public Guid ID { get; set; }

        public string Name { get; set; } = "";

        public IList<ExifProperty> ExifProperties { get; set; }
        public string Path { get; set; }

        public string Image { get; set; } = "";

        public Photographer Photographer { get; set; }

    }
}
