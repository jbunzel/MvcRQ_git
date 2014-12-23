using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mvc5RQ.Models;
using Mvc5RQ.Controllers;

namespace Mvc5RQ.Areas.DigitalObjects.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ViewerModel
    {
        #region public properties

        /// <summary>
        /// 
        /// </summary>
        public RQItem viewItem { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string digitalObjectAdress { get; set; }

        #endregion

        #region public constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="objectAdr"></param>
        public ViewerModel(string itemId, string objectAdr)
        {
            if (objectAdr.EndsWith(".mht")) objectAdr = objectAdr.Replace(".mht", ".pdf");
            this.viewItem = new RQItem(itemId);
            this.digitalObjectAdress = objectAdr;
        }

        #endregion

        #region public methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        virtual public int Count()
        {
            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string BibInfo()
        {
            string ret = "";

            this.viewItem._formatPreprocessor = new FormatParameter(FormatParameter.FormatEnum.rqi_short_title);
            ret = this.viewItem.ToString();
            return ret;
        }

        #endregion
    }
}