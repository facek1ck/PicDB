﻿using PicDB.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace PicDB.DataAccess
{
    class DALDatabase : IDAL
    {
        
        private static readonly SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\gashe\\work\\sem4\\swe2\\PicDB\\PicDB\\Database.mdf;Integrated Security=True;MultipleActiveResultSets=true");

        public void SavePicture(Picture p)
        {
            string query = "INSERT INTO Picture (Id, Name, Image) OUTPUT INSERTED.Id " + "VALUES (NEWID(), @name, @image); ";
            connection.Open();

            SqlCommand cmd = getCommand(query);
            cmd.Parameters.AddWithValue("@name",p.Name);
            cmd.Parameters.AddWithValue("@image",p.Image);

            p.ID = (Guid)cmd.ExecuteScalar(); //get ID generated by DB

            addExifPropertiesForPicture(p);
            addExifPropertiesToPictureMatchers(p);

            connection.Close();
        }

        private void addExifPropertiesForPicture(Picture p)
        {
            string query = "INSERT INTO ExifData (Id, TagNumber, Value, Comment) OUTPUT INSERTED.Id " + "VALUES (NEWID(), @tagNumber, @value, @comment)";

            foreach (ExifProperty property in p.ExifProperties)
            {
                SqlCommand cmd = getCommand(query);
                cmd.Parameters.AddWithValue("@tagNumber", property.TagNumber);
                cmd.Parameters.AddWithValue("@value", property.Value);
                cmd.Parameters.AddWithValue("@comment", property.Comment);
                property.ID = (Guid) cmd.ExecuteScalar(); //get ID generated by DB
         
            }
        }

        private void addExifPropertiesToPictureMatchers(Picture p)
        {
            string query = "INSERT INTO PictureExifData (PictureID, ExifDataID) " + "VALUES (@pictureID, @exifDataID)";
            foreach (ExifProperty property in p.ExifProperties)
            {
                SqlCommand cmd = getCommand(query);
                cmd.Parameters.AddWithValue("@pictureID", p.ID);
                cmd.Parameters.AddWithValue("@exifDataID", property.ID);
                cmd.ExecuteNonQuery();
            }
        }

        private SqlCommand getCommand(string query)
        {
           return new SqlCommand(query, connection);
        }

        public void DeletePictureById(Guid ID)
        {
            Picture p = new Picture();
            p.ID = ID;
            IList<Guid> exifPropIds = new List<Guid>();
            string query = "delete from Picture WHERE Id = " + "@id";
            connection.Open();
            foreach (var exifProp in GetExifPropertiesForPicture(p))
            {
                exifPropIds.Add(exifProp.ID);
            }
            deleteExifPropertiesToPictureMatchers(ID);
            deleteExifPropertiesForPicture(exifPropIds);
            SqlCommand cmd = getCommand(query);
            cmd.Parameters.AddWithValue("@id", ID);
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        private void deleteExifPropertiesToPictureMatchers(Guid ID)
        {
            string query = "delete from PictureExifData WHERE PictureID = " + "@id";
            SqlCommand cmd = getCommand(query);
            cmd.Parameters.AddWithValue("@id", ID);
            cmd.ExecuteNonQuery();
        }

        private void deleteExifPropertiesForPicture(IList<Guid> Ids)
        {
            string query = "delete from ExifData WHERE Id in ({@id})";
            SqlCommand cmd = getCommand(query);
            cmd.AddArrayParameters("@id", Ids);
            cmd.ExecuteNonQuery();
        }

        public Picture GetPictureById(Guid ID)
        {
            Picture p = new Picture();
            string query = "SELECT * from Picture WHERE Id = " + "@id";
            connection.Open();

            SqlCommand cmd = getCommand(query);
            cmd.Parameters.AddWithValue("@id", ID);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    p.ID = reader.GetGuid(0);
                    p.Name = reader.GetString(1);
                    p.Image = reader.GetString(2);
                    if (!reader.IsDBNull(3))
                    {
                        p.Photographer = GetPhotographerById(reader.GetGuid(3));
                    }
                }
            }
            p.ExifProperties = GetExifPropertiesForPicture(p);
            connection.Close();
            return p;
        }

        private IList<ExifProperty> GetExifPropertiesForPicture(Picture p)
        {
            IList<ExifProperty> properties = new List<ExifProperty>();
            IList<Guid> propIds = new List<Guid>();

            string matcherQuery = "SELECT ExifDataID from PictureExifData WHERE PictureID = " + "@id";
            SqlCommand matcherCmd = getCommand(matcherQuery);
            matcherCmd.Parameters.AddWithValue("@id", p.ID);

            using (var matcherReader = matcherCmd.ExecuteReader())
            {
                while (matcherReader.Read())
                {
                    propIds.Add(matcherReader.GetGuid(0));
                }
            }

            string query = "SELECT * from ExifData WHERE Id IN ({@propIds})";
            SqlCommand cmd = getCommand(query);
            cmd.AddArrayParameters("@propIds", propIds);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    ExifProperty tempProp = new ExifProperty();
                    tempProp.ID = reader.GetGuid(0);
                    tempProp.TagNumber = reader.GetInt32(1);
                    tempProp.Value = reader.GetString(2);
                    tempProp.Comment = reader.GetString(3);
                    properties.Add(tempProp);
                }
            }
                return properties;
        }

        public IList<Picture> GetAllPictures()
        {
            IList<Picture> pictures = new List<Picture>();
            string query = "select * from Picture";

            connection.Open();
            SqlCommand cmd = getCommand(query);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Picture p = new Picture();
                    p.ID = reader.GetGuid(0);
                    p.Name = reader.GetString(1);
                    p.Image = reader.GetString(2);
                    if (!reader.IsDBNull(3))
                    {
                        p.Photographer = GetPhotographerById(reader.GetGuid(3));
                    }
                    p.ExifProperties = GetExifPropertiesForPicture(p);
                    pictures.Add(p);
                }
            }
            connection.Close();
            return pictures;
        }

        public IList<Photographer> GetAllPhotographers()
        {
            IList<Photographer> photographers = new List<Photographer>();
            string query = "select * from Photographer";

            connection.Open();

            SqlCommand cmd = getCommand(query);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Photographer p = new Photographer();
                    p.ID = reader.GetGuid(0);
                    p.FirstName = reader.GetString(1);
                    p.LastName = reader.GetString(2);
                    if (!reader.IsDBNull(3))
                    {
                        p.Birthday = reader.GetDateTime(3);
                    }
                    photographers.Add(p);
                }
            }

            connection.Close();
            return photographers;
        }

        public Photographer GetPhotographerById(Guid ID)
        {
            Photographer p = new Photographer();
            string query = "SELECT * from Photographer WHERE Id = " + "@id";
            connection.Open();

            SqlCommand cmd = getCommand(query);
            cmd.Parameters.AddWithValue("@id", ID);
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    p.ID = reader.GetGuid(0);
                    p.FirstName = reader.GetString(1);
                    p.LastName = reader.GetString(2);
                    if (!reader.IsDBNull(3))
                    {
                        p.Birthday = reader.GetDateTime(3);
                    }
                }
            }
            connection.Close();
            return p;
        }

        public void SavePhotographer(Photographer p)
        {
            string query = "INSERT INTO Photographer (Id, FirstName, LastName, Birthday) " + "VALUES (NEWID(), @firstname, @lastname, @birthday); ";
            connection.Open();

            SqlCommand cmd = getCommand(query);
            cmd.Parameters.AddWithValue("@firstname", p.FirstName);
            cmd.Parameters.AddWithValue("@lastname", p.LastName);
            if(p.Birthday != null)
            {
                cmd.Parameters.AddWithValue("@birthday", p.Birthday);
            }
            else
            {
                cmd.Parameters.AddWithValue("@birthday", DBNull.Value);

            }

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public void DeletePhotographerById(Guid ID)
        {
            string query = "delete from Photographer WHERE Id = " + "@id";
            connection.Open();
            SqlCommand cmd = getCommand(query);
            cmd.Parameters.AddWithValue("@id", ID);
            cmd.ExecuteNonQuery();
            connection.Close();
        }
    }
}
