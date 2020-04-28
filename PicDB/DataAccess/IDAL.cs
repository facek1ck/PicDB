using PicDB.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.DataAccess
{
     interface IDAL
    {
        void initialize();
        List<Picture> getPictures();
        //List<Picture> getPhotographer();

        Picture getPictureById(Guid ID);
        void savePicture(Picture p);
        void deletePicture(Picture p);
        void addPicture(Picture p);
    }
}