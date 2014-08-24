using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RQDigitalObjects.AudioObjects;

namespace MvcRQ.Areas.ItemViewer.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AudioModel : ItemViewerModel
    {
        #region public properties

        /// <summary>
        /// 
        /// </summary>
        public JPlayerPlaylist PlayList { get; set; }

        #endregion

        #region public constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rqitemId"></param>
        /// <param name="M3uURI"></param>
        public AudioModel(string rqitemId, string M3uURI)
            :base(rqitemId, M3uURI)
        {
            string toc = this.viewItem.GetToC();
            PlayList = new RQDigitalObjects.AudioObjects.JPlayerPlaylist(new RQDigitalObjects.AudioObjects.MP3.M3uPlayList(M3uURI), toc); 
        }

        /// <summary>
        /// 
        /// </summary>
        //public AudioModel()
        //    :base()
        //{ }

        #endregion

        #region public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int Count()
        {
            if (PlayList != null)
                return PlayList.Count();
            else
                return 0;
        }

        #endregion
    }
}