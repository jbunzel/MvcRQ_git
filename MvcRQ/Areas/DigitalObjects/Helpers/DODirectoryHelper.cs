using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

using MvcRQ.Areas.UserSettings;
using MvcRQ.Areas.UserSettings.Models;

using RQDigitalObjects;
using RQDigitalObjects.AudioObjects.MP3;


namespace MvcRQ.Areas.DigitalObjects.Helpers
{
    public class DODirectoryHelper
    {
        public static void CreateDigitalObjectDirectory( string tocString)
        {

        }

        public static bool CheckDigitalObjectDirectory(string itemId, ref string tocString, ref string signatureStr, StringDictionary itemDescriptors )
        {
            bool result = false;

            if ((tocString.Contains("$$TOC$$=")) && (tocString.Contains("XXXXX_")))
            {
                ImportOptions directories = new UserSettingsService().GetImportOptions();
                string objectName = tocString.Substring(tocString.IndexOf("XXXXX_") + "XXXXX_".Length,tocString.IndexOf("/",tocString.IndexOf("XXXXX_")) - tocString.IndexOf("XXXXX_") -"XXXXX_".Length); 
                string objectType = tocString.Contains("ArcMusicMp3") ? "ArcMusicMp3" : "ArcAudioMp3";
                Mp3Album digObj = new Mp3Album(directories.AudioProjectDirectory + "\\" + objectName, objectType);

                digObj.Read();
                if (digObj.ElementCount > 0)
                {
                    bool[] includeElements = new bool[digObj.ElementCount];
                    string newSigStr = "JB: MyMusic=" + digObj.GenerateAccessLink(itemId) + "; ";
                    string newTocStr = tocString.Replace("XXXXX_", itemId + "_");

                    for (int i = 0; i < digObj.ElementCount; i++)
                    {
                        string theFileName = digObj.SelectElement(i + 1).DigitalObjectIdentifier(DigitalObject.DOIdentifier.filename);

                        if (includeElements[i] = newTocStr.Contains(theFileName))
                            newTocStr = newTocStr.Replace(theFileName, "TRACK" + (i+1) + ".m3u");
                    }
                    if (itemDescriptors.ContainsKey("Abstract"))
                        itemDescriptors["Abstract"] = newTocStr;
                    tocString = newTocStr;
                    signatureStr += signatureStr.Contains(newSigStr) ? "" : " " + newSigStr;
                    try
                    {
                        digObj.GenerateAccessStructure(itemId, includeElements);
                        digObj.ChangeDescriptionElements(itemDescriptors, includeElements);
                        result = true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Directory for digital object could not be created. " + ex.Message, ex.InnerException);  
                    }
                }
            }
            return result;
        }
    }
}