using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RQDigitalObjects.AudioObjects;

namespace Mvc5RQ.Areas.DigitalObjects.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class VideoModel : ViewerModel
    {
        #region public properties

        //public JPlayerPlaylist PlayList { get; set; }

        #endregion

        #region public constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rqitemId"></param>
        /// <param name="URI"></param>
        public VideoModel(string rqitemId, string URI)
            :base(rqitemId, URI)
        {
            //PlayList = new RQDigitalObjects.AudioObjects.JPlayerPlaylist(new RQDigitalObjects.AudioObjects.MP3.M3uPlayList(M3uURI)); 
        }

        /// <summary>
        /// 
        /// </summary>
        //public VideoModel()
        //    :base()
        //{ }

        #endregion
    }
}