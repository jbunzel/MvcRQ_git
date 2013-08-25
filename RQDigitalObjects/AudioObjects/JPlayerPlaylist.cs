using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RQDigitalObjects.AudioObjects.MP3;

namespace RQDigitalObjects.AudioObjects
{
    public class AudioPlaylistItem
    {
        public string title {get; set; }
        public string artist {get; set; }
        public string mp3 {get; set; }

        public AudioPlaylistItem(string Title, string Artist, string Mp3)
        {
            this.title = Title;
            this.artist = Artist;
            this.mp3 = Mp3;
        }
    }
    
    public class JPlayerPlaylist
    {
        #region public properties

        public AudioPlaylistItem[] Tracks { get; set; }

        #endregion

        #region public constructors

        public JPlayerPlaylist(M3uPlayList m3uPlayList) {
            this.Tracks = new AudioPlaylistItem[m3uPlayList.Count()];
            for (int i = 0; i < m3uPlayList.Count(); i++)
                this.Tracks[i] = m3uPlayList.GetItem(i);
        }

        public JPlayerPlaylist(M3uPlayList m3uPlayList, string toc)    
            : this(m3uPlayList) 
        {
            string[] rows = toc.Split(new string[]  {";", "; "}, StringSplitOptions.RemoveEmptyEntries);
            if (rows.Count() == this.Count())
                for (int i = 0; i < this.Count(); i++)
                    this.Tracks[i].title = rows[i].Substring(0, rows[i].IndexOf("My"));
        }
        
        #endregion

        #region public methods

        public int Count()
        {
            return this.Tracks.Count();
        }
        #endregion
    }
}
