using PicDB.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PicDB.DataAccess
{
     interface IDAL
    {
        IList<Picture> GetAllPictures();
        Picture GetPictureById(Guid ID);
        void SavePicture(Picture p);
        void DeletePictureById(Guid ID);
        IList<Photographer> GetAllPhotographers();
        Photographer GetPhotographerById(Guid id);
        void SavePhotographer(Photographer p);
        void DeletePhotographerById(Guid ID);

    }
}