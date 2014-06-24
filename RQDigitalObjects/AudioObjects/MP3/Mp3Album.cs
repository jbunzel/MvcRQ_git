using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using RQDigitalObjects;
using RQDigitalObjects.AudioObjects; 

namespace RQDigitalObjects.AudioObjects.MP3
{
	
    /// <summary>
	/// Diese Klasse liest und schreibt Verzeichnis, Dateinamen und ID3v1- bzw. 
	/// ID3v2-Titelinformationen eines MP3-Albums (Verzeichnis mit MP3-Dateien)
	/// </summary>
	public class Mp3Album : StructuredDigitalObject
	{
        
#region genre array

		/// <summary>
		/// Dieses Array dient der Zuordnung des Genre-Bytes aus
		/// den Dateiinformationen zu einem Klarnamen. Es enthält
		/// die Genrenamen im Wortlaut der Spezifikation plus WinAmp-
		/// Erweiterungen, einzusehen unter http://www.id3.org/id3v2-00.txt,
		/// Abschnitt A3.
		/// </summary>
		private static String[] m_genreArray = {
											"Blues",
											"Classic Rock",
											"Country",
											"Dance",
											"Disco",
											"Funk",
											"Grunge",
											"Hip-Hop",
											"Jazz",
											"Metal",
											"New Age",
											"Oldies",
											"Other",
											"Pop",
											"R&B",
											"Rap",
											"Reggae",
											"Rock",
											"Techno",
											"Industrial",
											"Alternative",
											"Ska",
											"Death Metal",
											"Pranks",
											"Soundtrack",
											"Euro-Techno",
											"Ambient",
											"Trip-Hop",
											"Vocal",
											"Jazz+Funk",
											"Fusion",
											"Trance",
											"Classical",
											"Instrumental",
											"Acid",
											"House",
											"Game",
											"Sound Clip",
											"Gospel",
											"Noise",
											"Alternative Rock",
											"Bass",
											"Soul",
											"Punk",
											"Space",
											"Meditative",
											"Instrumental Pop",
											"Instrumental Rock",
											"Ethnic",
											"Gothic",
											"Darkwave",
											"Techno-Industrial",
											"Electronic",
											"Pop-Folk",
											"Eurodance",
											"Dream",
											"Southern Rock",
											"Comedy",
											"Cult",
											"Gangsta",
											"Top 40",
											"Christian Rap",
											"Pop/Funk",
											"Jungle",
											"Native US",
											"Cabaret",
											"New Wave",
											"Psychadelic",
											"Rave",
											"Showtunes",
											"Trailer",
											"Lo-Fi",
											"Tribal",
											"Acid Punk",
											"Acid Jazz",
											"Polka",
											"Retro",
											"Musical",
											"Rock & Roll",
											"Hard Rock",
											"Folk",
											"Folk-Rock",
											"National Folk",
											"Swing",
											"Fast Fusion",
											"Bebob",
											"Latin",
											"Revival",
											"Celtic",
											"Bluegrass",
											"Avantgarde",
											"Gothic Rock",
											"Progressive Rock",
											"Psychedelic Rock",
											"Symphonic Rock",
											"Slow Rock",
											"Big Band",
											"Chorus",
											"Easy Listening",
											"Acoustic",
											"Humour",
											"Speech",
											"Chanson",
											"Opera",
											"Chamber Music",
											"Sonata",
											"Symphony",
											"Booty Bass",
											"Primus",
											"Porn Groove",
											"Satire",
											"Slow Jam",
											"Club",
											"Tango",
											"Samba",
											"Folklore",
											"Ballad",
											"Power Ballad",
											"Rhytmic Soul",
											"Freestyle",
											"Duet",
											"Punk Rock",
											"Drum Solo",
											"Acapella",
											"Euro-House",
											"Dance Hall",
										};
#endregion


#region private members

		private ID3v2Info MP3Info;
		private String m_filename;
		private String m_title;
		private String m_artist;
		private String m_composer;
		private String m_album;
		private String m_year;
		private String m_comment;
		private byte m_genre;
		private String m_track;
		private bool m_containsID3tags;

        private string albumDocNr;
        private int actlTrack = 0;

#endregion


#region public properties

		/// <summary>
		/// Gibt an, ob das MP3-Album ID3v1-Informationen beinhaltet. 
        /// Dieses Attribut ist ReadOnly.
		/// </summary>
		public bool ContainsID3Tags
		{
			get
			{
				return this.m_containsID3tags;
			}
		}

		/// <summary>
		/// Titel des MP3-Albums (aus den ID3v1-Informationen).
		/// </summary>
		public String Title
		{
			get
			{
				if (m_title != MP3Info.Title)
				{
					m_title = MP3Info.Title;
				}
				return m_title;
			}
		}

		/// <summary>
		/// Name des Komponisten aus den ID3v2-Informationen.
		/// </summary>
		public String Composer
		{
			get
			{
				if (m_composer != MP3Info.Composer)
				{
					m_composer = MP3Info.Composer;
				}
				return m_artist;
			}
		}

		/// <summary>
		/// Name des Künstler aus den ID3v1-Informationen.
		/// </summary>
		public String Artist
		{
			get
			{
				if (m_artist != MP3Info.Artist)
				{
					m_artist = MP3Info.Artist;
				}
				return m_artist;
			}
		}

		/// <summary>
		/// Name des Albums aus den ID3v1-Informationen.
		/// </summary>
		public String Album
		{
			get
			{
				if (m_album != MP3Info.Album)
				{
					m_album = MP3Info.Album;
				}
				return m_album;
			}
		}

		/// <summary>
		/// Erscheinungsjahr aus den ID3v1-Informationen.
		/// </summary>
		public String Year
		{
			get
			{
				if (m_year != MP3Info.Year)
				{
					m_year = MP3Info.Year;
				}
				return m_year;
			}
		}

		/// <summary>
		/// Kommentar aus den ID3v1-Informationen.
		/// </summary>
		public String Comment
		{
			get
			{
				return m_comment;
			}
		}

		/// <summary>
		/// Genre aus den ID3v1-Informationen.
		/// </summary>
		public String Genre
		{
			//Intern wird im File nur ein Byte-Wert als Genre-Info
			//gespeichert. Die Übersetzung in Klarnamen erfolgt mit einer
			//internen Hash-Tabelle.
			get
			{
				return ""; //GetGenre(m_genre);
			}
		}

		/// <summary>
		/// CD-Track-Nummer aus den ID3v1-Informationen.
		/// </summary>
		public String Track
		{
			get
			{
				if (m_track != MP3Info.Track)
				{
					m_track = MP3Info.Track;
				}
				return m_track;
			}
		}

#endregion


#region public constructors

        /// <summary>
        /// Constructs a MP3-Album
        /// </summary>
        /// <param name="sourcePath">
        /// Path to the source folder of the MP3-Album  
        /// </param>
        /// <param name="targetFolder">
        /// Name of the subfolder on the MP3-media server (default: \\DISKSTATION/music), where the digital object shall be stored.
        /// (This subfolder is assumed to be either ArcAudioMP3 for text audios or ArcMusicMP3 for music audios)
        /// </param>
        public Mp3Album(string sourcePath, string targetFolder)
            : base(sourcePath, targetFolder)
		{
			MP3Info = new ID3v2Info();
			//this.m_albumdirectory = new DirectoryInfo(path);
		}

#endregion


#region private methods

		private void SaveTrackID3(String path)
		{
			//m_filename = path;
			//this.MP3Info.SaveFile(path);
			//ucID3v1.SaveFile(path);
			//this.ReadTrackID3(path);
		}

        /// <summary>
        /// Liest alle im Album-Verzeichnis enthaltenen Dateien, die dem Suchmuster entsprechen,
        /// ein und liest Dateiname plus evtl. vorhandener ID3-Tags aus.
        /// </summary>
        /// <param name="search">Suchpattern für die Dateinamen.</param>
        private void ReadAlbumID3(String search)
        {
            try
            {
                FileInfo[] allFiles = base.m_objectdirectory.GetFiles(search);

                for (int i = 0; i < allFiles.Length; i++)
                {
                    this.MP3Info.LoadFile(allFiles[i]);
                    if (string.IsNullOrEmpty(this.MP3Info.Filename))
                        this.MP3Info.Tags["filename"] = allFiles[i].Name;
                    base.m_elementarray.Add(this.MP3Info.Tags);
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }

        }

        private string GetTagValue(int track, string key)
        {
            if (track > 0)
                track = track - 1;

            StringDictionary tags = GetTagList(track);

            if ((tags != null) && (tags.ContainsKey(key)))
                return tags[key];
            else
                return "";
        }

        private StringDictionary GetTagList(int track)
        {
            if (track < base.m_elementarray.Count)
                return (StringDictionary)base.m_elementarray[track];
            else
                return null;
        }

        private void SetTagValue(int track, string key, string value)
        {
            if (track > 0)
                track = track - 1;

            StringDictionary tags = GetTagList(track);

            if ((tags != null) && (tags.ContainsKey(key)))
                tags[key] = value;
        }

        private void SaveTagList(int track)
        {
            if (track > 0)
                track = track - 1;
            if (track < base.m_elementarray.Count)
            {
                FileInfo[] trackfile = base.m_objectdirectory.GetFiles(this.GetTagValue(track + 1, "MP3FileName"));

                if ((trackfile != null) && (trackfile.Length > 0))
                {
                    this.MP3Info.Tags = (StringDictionary)base.m_elementarray[track];
                    this.MP3Info.SaveFile(trackfile[0]);
                }
            }
        }

        private void RenameAlbum(string newName)
        {
            String newpath = base.m_objectdirectory.FullName.Replace(base.m_objectdirectory.Name, newName);

            base.m_objectdirectory.MoveTo(newpath);
        }

        private void GenerateM3UFiles(string name)
        {
            GenerateM3UFiles(name, 0, null);
        }

        private void GenerateM3UFiles(string name, int type)
        {
            GenerateM3UFiles(name, type, null);
        }

        private void GenerateM3UFiles(string name, bool[] trackincluded)
        {
            GenerateM3UFiles(name, 0, trackincluded);
        }

        private void GenerateM3UFiles(string name, int type, bool[] trackincluded)
        {
            string[] m3utext = new string[base.m_elementarray.Count];
            int j = 0;

            try
            {
                for (int i = 0; i < base.m_elementarray.Count; i++)
                    if (trackincluded != null)
                    {
                        if (trackincluded[i] == true)
                        {
                            if (type != 0)
                            {
                                m3utext[0] = GetTagValue(i + 1, "MP3FileName");
                                File.WriteAllLines(base.m_objectdirectory.FullName + name + (i + 1).ToString() + ".m3u", m3utext);
                            }
                            else
                                m3utext[j++] = GetTagValue(i + 1, "MP3FileName");
                        }
                    }
                    else
                        m3utext[i] = GetTagValue(i + 1, "MP3FileName");
                if (type == 0)
                    File.WriteAllLines(base.m_objectdirectory.FullName + name + ".m3u", m3utext);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void GenerateAccessStructure(string resultItemID, bool[] includeElements)
        {
            if (this.actlTrack == 0)
            {
                this.RenameAlbum(resultItemID + "_" + base.m_olddirectoryname);
                this.GenerateM3UFiles(resultItemID, 0, includeElements);
                this.GenerateM3UFiles("TRACK", 1, includeElements);
                this.albumDocNr = resultItemID;
            }
            else
            {
                this.GenerateM3UFiles(resultItemID, 0, includeElements);
            }
        }

        private void ChangeDescriptionElements(StringDictionary itemDescription, bool[] includeElements)
        {
            //base.ChangeDescriptionElements(itemDescription, includeElements);
            int i, k;

            if (this.actlTrack == 0)
                k = 1;
            else
                k = this.actlTrack;
            for (i = k; i <= this.ElementCount; i++)
            {
                if (includeElements[i - 1])
                {
                    // if containertype != music
                    if (this.actlTrack == 0)
                        this.SetTagValue(i, "Album", itemDescription["Title"]);
                    if (itemDescription["Authors"] != "")
                    {
                        if (itemDescription["Authors"].EndsWith(";"))
                            itemDescription["Authors"] += " ";
                        if (!itemDescription["Authors"].EndsWith("; "))
                            itemDescription["Authors"] += "; ";
                    }
                    string[] authors = itemDescription["Authors"].Split(new string[] { "; " }, StringSplitOptions.None);
                    string conductor = "";
                    string composer = "";
                    string artist = "";
                    string lyricist = "";
                    int j = 0;
                    string delim = i > 1 ? "; " : "$=";
                    int to = itemDescription["Abstract"].IndexOf("TRACK" + i);
                    int start = itemDescription["Abstract"].LastIndexOf(delim, to) + delim.Length;
                    string str1 = itemDescription["Abstract"].Substring(start, to - start + 1);

                    this.SetTagValue(i, "Title", str1.Substring(0, str1.IndexOf("MyMusic")));
                    for (j = 0; j < authors.Length; j++)
                    {
                        if (authors[j].Contains("Mitw."))
                        {
                            if ((j < authors.Length - 1) && (authors[j].Contains("Dirigent")))
                                conductor += authors[j];
                            else
                                artist += authors[j];
                        }
                        else if (authors[j].Contains("Libr."))
                            lyricist += authors[j];
                        //else if (authors[j].Contains("Rolle"))
                        //nop
                        else
                            // if containertype not = music
                            lyricist += authors[j];
                        //else
                        // composer += authors[j];
                    }
                    this.SetTagValue(i, "Conductor", conductor);
                    this.SetTagValue(i, "Composer", composer);
                    this.SetTagValue(i, "Lyricist", lyricist);
                    if (itemDescription["Institutions"] != "")
                    {
                        if (itemDescription["Institutions"].EndsWith(";"))
                            itemDescription["Institutions"] += " ";
                        if (!itemDescription["Institutions"].EndsWith("; "))
                            itemDescription["Institutions"] += "; ";
                    }

                    string[] institutions = itemDescription["institutions"].Split(new string[] { "; " }, StringSplitOptions.None);
                    string accompaniment = "";

                    for (j = 0; j < institutions.Length; j++)
                    {
                        if (institutions[j].Contains("Mitw."))
                            accompaniment += institutions[j];
                    }
                    this.SetTagValue(i, "Accompaniment", accompaniment);
                    this.SetTagValue(i, "Artist", itemDescription["Authors"] + accompaniment);
                    this.SetTagValue(i, "Publisher", itemDescription["Publisher"]);
                    this.SetTagValue(i, "Year", itemDescription["PublTime"]);
                    this.SetTagValue(i, "TrackNumber", itemDescription["Issue"]);
                    this.SetTagValue(i, "DiscNumber", itemDescription["Volume"]);
                    this.SetTagValue(i, "Genre", itemDescription["WorkType"]);
                    this.SetTagValue(i, "Recording Dates", itemDescription["CreateTime"]);
                    this.SaveTagList(i);
                }
            }
        }

        private string GenerateAccessLink(string resultItemID)
        {
            if (resultItemID != "")
            {
                if (this.actlTrack == 0)
                {
                    this.albumDocNr = resultItemID;
                }
                return this.m_localContainer + "/" + this.albumDocNr + "_" + base.m_olddirectoryname + "/" + resultItemID + ".m3u";
            }
            else
                return this.m_localContainer + "/" + this.albumDocNr + "_" + base.m_olddirectoryname + "/" + "TRACK" + (this.actlTrack).ToString() + ".m3u";
        }

#endregion


#region public methods

        /// <summary>
        /// Reads the digital object from its source folder into memory
        /// </summary>
        public override void Read()
        {
            ReadAlbumID3("*.mp3");
        }

        /// <summary>
        /// Binds mp3 album to RQItem  
        /// </summary>
        /// <param name="itemId">
        /// Id of the RQItem to bind to
        /// </param>
        /// <param name="tocString">
        /// IN:     Toc from RQItem.      
        /// OUT:    Toc string to be stored in RQItem containing access structures to the tracks of the mp3 album.
        /// </param>
        /// <param name="signatureStr">
        /// IN:     Signature from RQItem.
        /// OUT:    Signature string to be stored in RQItem containing the primary (i. e. all tracks) access link to the digital object
        /// </param>
        /// <param name="itemDescription">
        /// String dictionary with field names (keys) and field values (values) of the RQItem description.
        /// </param>
        /// <returns>
        /// True if tocString or signatureStr has been changed.
        /// </returns>
        public override bool BindTo(string itemId, ref string tocString, ref string signatureStr, StringDictionary itemDescription)
        {
            bool result = false;

            if (tocString.Contains("XXXXX_"))
            {
                this.Read();
                if (this.ElementCount > 0)
                {
                    bool[] includeElements = new bool[this.ElementCount];
                    string newSigStr = "JB: MyMusic=" + this.GenerateAccessLink(itemId) + "; ";
                    string newTocStr = tocString.Replace("XXXXX_", itemId + "_");

                    for (int i = 0; i < this.ElementCount; i++)
                    {
                        string theFileName = this.SelectElement(i + 1).DigitalObjectIdentifier(DigitalObject.DOIdentifier.filename);

                        if (includeElements[i] = newTocStr.Contains(theFileName))
                            newTocStr = newTocStr.Replace(theFileName, "TRACK" + (i + 1) + ".m3u");
                    }
                    if (itemDescription.ContainsKey("Abstract"))
                        itemDescription["Abstract"] = newTocStr;
                    tocString = newTocStr;
                    signatureStr += signatureStr.Contains(newSigStr) ? "" : " " + newSigStr;
                    try
                    {
                        this.GenerateAccessStructure(itemId, includeElements);
                        this.ChangeDescriptionElements(itemDescription, includeElements);
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

        /// <summary>
        /// Returns a digital object identifier
        /// </summary>
        /// <param name="IdType">
        /// Type of the requested digital object identifier.
        /// </param>
        /// <returns>
        /// Digital object identifier string.
        /// </returns>
        public override string DigitalObjectIdentifier(DigitalObject.DOIdentifier IdType)
        {
            switch (IdType)
            {
                case DOIdentifier.filename:
                    return this.GetTagValue(actlTrack, "MP3FileName");
                case DOIdentifier.objectname:
                    return this.GetTagValue(actlTrack, "title");
            }
            return base.DigitalObjectIdentifier(IdType);
        }

        public override String InsertPrimaryAccessLink(string fromToc, string toTarget)
        {
            return toTarget;
        }

        /// <summary>
        /// Setzt das aktuelle Element (einer Kopie) des digitalen Objekts auf einen vorgegebenen Wert. 
        /// </summary>
        /// <param name="index">
        /// Ganzzahl, die die 0-basierte Position des neuen aktuellen Elements (in der zurück gegebenen Kopie des digitalen Objekts) angibt.
        /// </param>
        /// <returns>
        /// Gibt eine Kopie des aktuellen digitalen Objekts zurück, bei der das Element an der Psoition index das aktuelle Element ist.
        /// </returns>
        public override DigitalObject SelectElement(int index)
        {
            Mp3Album newdo = (Mp3Album)this.MemberwiseClone();
            newdo.actlTrack = index;
            return newdo;
        }

        public override string GenerateToc(string objectName, string serverDirectory)
        {
            string result = "$$TOC$$=";
            string dummyId = "";
            string strRegex = @"(?<res>\d{5})_";
            RegexOptions myRegexOptions = RegexOptions.None;
            Regex myRegex = new Regex(strRegex, myRegexOptions);
            Match MyMatch = myRegex.Match(objectName);

            if (!MyMatch.Success)
                dummyId = "XXXXX_";
            objectName = dummyId + objectName;
            this.Read();
            for (int i = 0; i < this.ElementCount; i++)
            {
                result += this.SelectElement(i + 1).DigitalObjectIdentifier(DigitalObject.DOIdentifier.objectname);
                result += serverDirectory + objectName + "/" + dummyId + this.SelectElement(i + 1).DigitalObjectIdentifier(DigitalObject.DOIdentifier.filename) + "; ";
            }
            return result;
        }

        /// <summary>
        /// Maps suitable attributes of the digital object onto the description element with "elementName" of a result item description 
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public String MapDescriptionElement(string elementName)
        {
            string descr = "";

            switch (elementName)
            {
                case "Title":
                    if (this.actlTrack == 0) 
                        descr = this.GetTagValue(0, "Album");
                    else
                        descr = this.GetTagValue(this.actlTrack,"Title");
                    break;
                case "Authors":
                    string autStr = this.GetTagValue(this.actlTrack, "Artist");
                    string comStr = this.GetTagValue(this.actlTrack, "Composer");

                    if (autStr != "") {
                        if ((this.GetTagValue(this.actlTrack, "Accompaniment") != "") && (autStr.Contains(this.GetTagValue(this.actlTrack, "Accompaniment"))))
                            autStr = autStr.Replace(this.GetTagValue(this.actlTrack, "Accompaniment"),"");
                        descr += autStr;
                    }
                    if (comStr != "")
                        if (! autStr.Contains(comStr))
                            descr += comStr + "; ";
                    comStr = this.GetTagValue(this.actlTrack,"Lyricist");
                    if (comStr != "")
                        if (! autStr.Contains(comStr))
                            descr += comStr + "; ";
                    comStr = this.GetTagValue(this.actlTrack,"Conductor");
                    if (comStr != "")
                        if (! autStr.Contains(comStr))
                            descr += comStr + "; ";
                    break;
                case "Institutions":
                    descr = this.GetTagValue(this.actlTrack, "Accompaniment");
                    break;
                case "Series":
                    if (this.actlTrack != 0)
                        descr = this.GetTagValue(this.actlTrack, "Album");
                    break;
                case "Publisher":
                    descr = this.GetTagValue(this.actlTrack, "Publisher");
                    break;
                case "PublTime":
                    descr = this.GetTagValue(this.actlTrack, "Year");
                    break;
                case "Issue":
                    descr = this.GetTagValue(this.actlTrack, "TrackNumber");
                    break;
                case "Volume":
                    descr = this.GetTagValue(this.actlTrack, "DiscNumber");
                    break;
                case "WorkType":
                    descr = this.GetTagValue(this.actlTrack, "Genre");
                    break;
                case "CreateTime":
                    descr = this.GetTagValue(this.actlTrack, "DateRecorded");
                    break;
                    
            }
            return descr;
        }

#endregion

    }

}
