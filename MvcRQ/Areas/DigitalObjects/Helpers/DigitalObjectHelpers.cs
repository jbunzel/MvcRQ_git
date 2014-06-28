using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

using MvcRQ.Areas.UserSettings;
using MvcRQ.Areas.UserSettings.Models;
using RQDigitalObjects;
using RQDigitalObjects.AudioObjects.MP3;
using RQDigitalObjects.VideoObjects.M4V;
using RQDigitalObjects.GraphObjects.PDF;


namespace MvcRQ.Areas.DigitalObjects.Helpers
{
    /// <summary>
    /// Static helper functions to handle tables of content of digital objects
    /// </summary>
    public class DigitalObjectHelpers
    {
        #region private methods

        private static string GetObjectName( string fromTocString)
        {
            string strRegex = @"=[_\w/]*(?<res>(\d|X){5}_?\w*)/";
            RegexOptions myRegexOptions = RegexOptions.None;
            Regex myRegex = new Regex(strRegex, myRegexOptions);
            Match MyMatch = myRegex.Match(fromTocString);

            if (MyMatch.Success)
                return MyMatch.Groups["res"].Value;
            else
                return "";
        }

        #endregion

        #region public methods

        /// <summary>
        /// Extracts the primary media-adress from toc And inserts it into the target string.
        /// </summary>
        /// <param name="fromTocString">
        /// The toc string from which the primary media-address will be extracted. 
        /// </param>
        /// <param name="toTargetStr">
        /// The string into which the primary media-adress will be inserted.
        /// </param>
        public static string UpdatePrimaryAccessLink(string itemID, string fromToc, string toTarget)
        {
            ImportOptions directories = new UserSettingsService().GetImportOptions();
            string objectName = GetObjectName(fromToc);
            string toc = fromToc.ToLower();

            if (toc.Contains("arcmusicmp3") || toc.Contains("arcaudiomp3"))
                return new Mp3Album(directories.AudioProjectDirectory + "\\" + objectName, toc.Contains("arcmusicmp3") ? "ArcMusicMP3" : "ArcAudioMP3").InsertPrimaryAccessLink(fromToc, toTarget);
            if (toc.Contains("myvideo"))
                return new M4vAlbum(directories.AudioProjectDirectory + "\\" + objectName).InsertPrimaryAccessLink(fromToc, toTarget);
            if (toc.Contains("mydocs"))
                return new PdfCollection(directories.DocumentProjectDirectory + "\\" + objectName).InsertPrimaryAccessLink(fromToc, toTarget);
            return toTarget;
        }

        /// <summary>
        /// Checks and updates the digital object directory and writes changes to the toc and signature strings.
        /// Checks and updates concern the directory name and the names of media and access structure files (f. e. playlist files)
        /// </summary>
        /// <param name="itemId">
        /// Id of the RQItem record of the digital object.
        /// </param>
        /// <param name="tocString">
        /// The toc string (normally stored in the RQItem record of the digital object).
        /// </param>
        /// <param name="signatureStr">
        /// The signature string (normally stored in the RQItem record of the digital object).</param>
        /// <param name="itemDescriptors">
        /// A string dictionary of field names (keys) and field contents (values) of the RQItem record of the digital object.
        /// </param>
        /// <returns>
        /// True if update was successful.
        /// Updated values in tocString and signatureStr.
        /// </returns>
        public static bool UpdateDigitalObjectDirectory(string itemId, ref string tocString, ref string signatureStr, StringDictionary itemDescriptors)
        {
            bool result = false;
            string toc = tocString.ToLower();

            if (toc.Contains("$$toc$$=")) 
            {
                ImportOptions directories = new UserSettingsService().GetImportOptions();
                string objectName = GetObjectName(tocString);

                if (toc.Contains("arcmusicmp3") || toc.Contains("arcaudiomp3") )
                    return new Mp3Album(directories.AudioProjectDirectory + "\\" + objectName.Replace("XXXXX_", ""), toc.Contains("arcmusicmp3") ? "ArcMusicMP3" : "ArcAudioMP3").BindTo(itemId, ref tocString, ref signatureStr, itemDescriptors);
                if (toc.Contains("myvideo"))
                    return new M4vAlbum(directories.VideoProjectDirectory + "\\" + objectName).BindTo(itemId, ref tocString, ref signatureStr, itemDescriptors);
                if (toc.Contains("mydocs"))
                    return new PdfCollection(directories.DocumentProjectDirectory + "\\" + objectName).BindTo(itemId, ref tocString, ref signatureStr, itemDescriptors);
            }
            return result;
        }

        #endregion
    }

}