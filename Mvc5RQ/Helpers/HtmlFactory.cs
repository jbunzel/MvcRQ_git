using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MvcRQ.Areas.Scripts;
using System.Text.RegularExpressions;

namespace MvcRQ.Helpers
{
  /// <summary>
  /// Static Html Generator for building the user interface html, javascript and css code.
  /// </summary>
  public static class HtmlFactory
  {
    const string _styleSheetBlock = "<style type='text/css'>{0}</style>";
    const string _javascriptBlock = "<script type='text/javascript'>{0}</script>";

    /// <summary>
    /// Builds the edit RQItem form for client editing & creating of RQItems.
    /// </summary>
    /// <returns>Html code for client editing & creating ofRQItems.</returns>
    //public static IHtmlString EditRQItemForm()
    //{
    //  var editRQItemForm = MvcRQ.Scripts.HtmlFactoryScripts.ScriptPack.edit_rqitem_form;

    //  return new HtmlString(editRQItemForm);
    //}

    /// <summary>
    /// Builds the user interface for client editing & creating of RQItems.
    /// </summary>
    /// <returns>Javascript code for client editing & creating ofRQItems.</returns>
    public static IHtmlString EditRQItemCode()
    {
      var builder = new StringBuilder();

      builder.AppendFormat(_javascriptBlock, MvcRQ.Scripts.HtmlFactoryScripts.ScriptPack.rqui_helpers);
      builder.AppendFormat(_javascriptBlock, MvcRQ.Scripts.HtmlFactoryScripts.ScriptPack.edit_rqitem_code.Replace("{controllerName}", "RQItems"));
      return new HtmlString(builder.ToString());
    }

    /// <summary>
    /// Builds styles for client editing & creating of RQItems.
    /// </summary>
    /// <returns>Css code for client editing & creating ofRQItems.</returns>
    public static IHtmlString EditRQItemStyle()
    {
      var styleBlock = string.Format(_styleSheetBlock, MvcRQ.Scripts.HtmlFactoryScripts.ScriptPack.edit_rqitem_style);
     
      return new HtmlString(styleBlock);
    }
  }
}
