﻿using PicDB.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace PicDB.DataAccess
{
    class DALDatabase : IDAL
    {
        
        private static readonly SqlConnection connection = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\gashe\\work\\sem4\\swe2\\PicDB\\PicDB\\Database.mdf;Integrated Security=True");

        public void addPicture(Picture p)
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

        public void deletePicture(Picture p)
        {
            throw new NotImplementedException();
        }

        public Picture getPictureById(Guid ID)
        {
            Picture p = new Picture();
            Guid photographerId;
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
                        photographerId = reader.GetGuid(3);
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

        public List<Picture> getPictures()
        {
            throw new NotImplementedException();
        }

        public void initialize()
        {
            throw new NotImplementedException();
        }

        public void savePicture(Picture p)
        {
            throw new NotImplementedException();
        }
    }
}
