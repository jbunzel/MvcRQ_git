using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RQDigitalObjects.AudioObjects;

namespace RQDigitalObjects.AudioObjects.MP3
{
    public class M3uPlayList
    {
        private string[] _rows;
        private string _uri_stem;

        public M3uPlayList(string uri)
        {
            System.Text.StringBuilder playList = new System.Text.StringBuilder();

            this._uri_stem = uri.Substring(0,uri.LastIndexOf("/")) + "/";
            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    _rows = webClient.DownloadString(uri).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                }
            }
            catch (Exception ex) {}
        }

        public AudioPlaylistItem GetItem(int index)
        {
            if (index <= _rows.Count())
                return new AudioPlaylistItem("Track" + (index + 1), "", this._uri_stem + _rows[index]);
            else
                return null;
        }

        public int Count()
        {
            if (_rows != null)
                return _rows.Count();
            else
                return 0;
        }
    }
}