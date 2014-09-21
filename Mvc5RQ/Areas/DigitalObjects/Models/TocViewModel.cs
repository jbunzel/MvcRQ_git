using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

using MvcRQ.Areas.DigitalObjects.Helpers;
using RQDigitalObjects;
using RQDigitalObjects.AudioObjects.MP3;
using RQDigitalObjects.VideoObjects.M4V;
using RQDigitalObjects.GraphObjects.PDF;

namespace MvcRQ.Areas.DigitalObjects.Models
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract()]
    public class TocViewModel
    {
        [DataMember()]
        public string toc { get; set; }

        #region private methods

        ///// <summary>
        ///// Returns a string encoding the table of content (toc) of the digital object.
        ///// </summary>
        ///// <returns></returns>
        //private string GenerateToc(StructuredDigitalObject digitalObject, string serverDirectory)
        //{
        //    string result = "$$TOC$$=";

        //    digitalObject.Read();
        //    for (int i = 0; i < digitalObject.ElementCount; i++)
        //    {
        //        result += digitalObject.SelectElement(i + 1).DigitalObjectIdentifier(DigitalObject.DOIdentifier.objectname);
        //        result += "MyMusic=" + serverDirectory + "/" + digitalObject.SelectElement(i + 1).DigitalObjectIdentifier(DigitalObject.DOIdentifier.filename) + "; ";
        //    }
        //    return result;
        //}

        #endregion

        #region public constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="objectType"></param>
        public TocViewModel (string objectName, string objectType)
        {
            UserSettings.Models.ImportOptions directories = new UserSettings.UserSettingsService().GetImportOptions();

            switch (objectType)
            {
                case "mp3Text":
                case "mp3Music":
                    this.toc = new Mp3Album(directories.AudioProjectDirectory + "\\" + objectName, objectType).GenerateToc(objectName, directories.MusicServerDirectory);
                    break;
                case "mp3Audio":
                    this.toc = new Mp3Album(directories.AudioProjectDirectory + "\\" + objectName, objectType).GenerateToc(objectName, directories.AudioServerDirectory);
                    break;
                case "mv4Video":
                    this.toc = new M4vAlbum(directories.VideoProjectDirectory + "\\" + objectName).GenerateToc(objectName, directories.VideoServerDirectory);
                    break;
                case "pdfDocument":
                    this.toc = new PdfCollection(directories.DocumentProjectDirectory + "\\" + objectName).GenerateToc(objectName, directories.DocumentServerDirectory);
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}