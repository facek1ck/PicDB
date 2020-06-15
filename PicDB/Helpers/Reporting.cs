using LinqToDB.Common;
using Microsoft.SqlServer.Server;
using Microsoft.Win32;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PicDB.Model;
using PicDB.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PicDB.Helpers
{
    class Reporting
    {
        public static void printPicture(Picture p)
        {
            Document doc = new Document();
            Section section1 = doc.AddSection();

            MigraDoc.DocumentObjectModel.Shapes.Image image = section1.AddImage(Conversion.MigrDocFilenameFromBase64(p.Image));
            image.Width = "16cm";
            image.LockAspectRatio = true;
            Paragraph paragraph1 = section1.AddParagraph();
            paragraph1.AddFormattedText("Photographer: ", TextFormat.Bold);
            if(p.Photographer != null)
            {
                paragraph1.AddText(p.Photographer.ToString() + "\n\n");
            }
            else
            {
                paragraph1.AddText("-" + "\n\n");
            }
            Paragraph paragraph2 = section1.AddParagraph();
            paragraph2.AddFormattedText("EXIF Properties:\n", TextFormat.Bold);
            paragraph2.AddText(PrintableExifProperties(p));
            paragraph2.AddFormattedText("IPTC Properties:\n", TextFormat.Bold);
            paragraph2.AddText(PrintableIptcProperties(p));
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false);
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = false;
            var result = dialog.ShowDialog();
            if (result == false)
            {
                return;
            }
            pdfRenderer.PdfDocument.Save(dialog.FileName);
            
        }

        private static string PrintableIptcProperties(Picture p)
        {
            StringBuilder result = new StringBuilder();
            foreach (Property prop in p.IptcProperties)
            {
                result.Append(prop.ToString());
                result.Append("\n");
            }
            result.Append("\n");
            return result.ToString();
        }

        private static string PrintableExifProperties(Picture p)
        {
            StringBuilder result = new StringBuilder();
            foreach (Property prop in p.ExifProperties)
            {
                result.Append(prop.ToString());
                result.Append("\n");
            }
            result.Append("\n");
            return result.ToString();
        }

        public static void printTags(PictureViewModel pvm)
        {
            Dictionary<string, int> exifCount = CountExifTags(pvm);
            Dictionary<string, int> iptcCount = CountIptcTags(pvm);

            Document doc = new Document();
            Section section1 = doc.AddSection();
            Paragraph paragraph1 = section1.AddParagraph();
            paragraph1.AddFormattedText("EXIF Properties:\n", TextFormat.Bold);
            paragraph1.AddText(PrintableTags(exifCount));
            paragraph1.AddFormattedText("IPTC Properties:\n", TextFormat.Bold);
            paragraph1.AddText(PrintableTags(iptcCount));
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(false);
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.CheckFileExists = false;
            var result = dialog.ShowDialog();
            if (result == false)
            {
                return;
            }
            pdfRenderer.PdfDocument.Save(dialog.FileName);
        }

        private static string PrintableTags(Dictionary<string, int> tags)
        {
            StringBuilder result = new StringBuilder();
            foreach(string s in tags.Keys)
            {
                result.Append(s);
                result.Append(": ");
                result.Append(tags[s]);
                result.Append(" \n");
            }
            result.Append("\n");
            return result.ToString();
        }

        private static Dictionary<string, int> CountExifTags(PictureViewModel pvm)
        {
            Dictionary<string,int> tags = new Dictionary<string, int>();
            foreach(Picture p in pvm.Images)
            {
                foreach(Property prop in p.ExifProperties)
                {
                    if (tags.IsNullOrEmpty())
                    {
                        tags.Add(prop.Name, 1);
                    }
                    else
                    {
                        if (tags.ContainsKey(prop.Name))
                        {
                            tags[prop.Name] += 1;
                        }
                        else
                        {
                            tags.Add(prop.Name, 1);
                        }
                    }

                }
            }
            return tags;
        }

        private static Dictionary<string, int> CountIptcTags(PictureViewModel pvm)
        {
            Dictionary<string, int> tags = new Dictionary<string, int>();
            foreach(Picture p in pvm.Images)
            {
                foreach (Property prop in p.IptcProperties)
                {
                    if (tags.IsNullOrEmpty())
                    {
                        tags.Add(prop.Name, 1);
                    }
                    else
                    {
                        if (tags.ContainsKey(prop.Name))
                        {
                            tags[prop.Name] += 1;
                        }
                        else
                        {
                            tags.Add(prop.Name, 1);
                        }
                    }
                }
            }
            return tags;
        }

    }
}
