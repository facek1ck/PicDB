using PicDB.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.DataAccess
{
     interface IDAL
    {
        IList<Picture> getAllPictures();
        Picture getPictureById(Guid ID);
        void savePicture(Picture p);
        void deletePicture(Picture p);
    }
}