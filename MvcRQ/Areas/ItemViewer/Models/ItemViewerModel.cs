using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcRQ.Models;
using MvcRQ.Controllers;

namespace MvcRQ.Areas.ItemViewer.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class ItemViewerModel
    {
        #region public properties

        /// <summary>
        /// 
        /// </summary>
        public RQItem viewItem { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string itemAdress { get; set; }

        #endregion

        #region public constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="itemAdr"></param>
        public ItemViewerModel(string itemId, string itemAdr)
        {
            this.viewItem = new RQItem(itemId);
            this.itemAdress = itemAdr;
        }

        /// <summary>
        /// 
        /// </summary>
        public ItemViewerModel()
        { }

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
            return this.viewItem.ConvertToHTML(RQItem.DisplFormat.short_title);
        }

        #endregion
    }
}