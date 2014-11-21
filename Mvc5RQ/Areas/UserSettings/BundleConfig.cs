using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;

namespace Mvc5RQ.Areas.UserSettings
{
  /// <summary>
  /// Static Html Generator for building the user interface html, javascript and css code.
  /// </summary>
  public static class BundleConfig
  {
    const string _styleSheetBlock = "<style type='text/css'>{0}</style>";
    const string _javascriptBlock = "<script type='text/javascript'>{0}</script>";

    /// <summary>
    /// Required: Builds javascript blocks for setting of query option.
    /// </summary>
    /// <returns>(required) Javascript code for generating the user interface.</returns>
    public static IHtmlString QueryOptionsJavascript()
    {
        var builder = new StringBuilder();
        
        builder.AppendFormat(_javascriptBlock, ScriptPack.simple_query_options_management.Replace("{controllerName}","UserSettings"));
        return new HtmlString(builder.ToString());
    }

    /// <summary>
    /// Required: Builds javascript blocks for setting of classtree options.
    /// </summary>
    /// <returns>(required) Javascript code for generating the user interface.</returns>
    public static IHtmlString ClasstreeOptionsJavascript()
    {
        var builder = new StringBuilder();

        builder.AppendFormat(_javascriptBlock, ScriptPack.classtree_options_management.Replace("{controllerName}", "UserSettings"));
        return new HtmlString(builder.ToString());
    }

    /// <summary>
    /// Builds the options selection for classtree.
    /// </summary>
    /// <returns>Classtree option settings html code</returns>
    public static IHtmlString ClasstreeOptionsForm()
    {
        string Html = ScriptPack.classtree_options_form;

        Html = Html.Replace("{classtree-options}", RQResources.Views.Shared.SharedStrings.classtree_options);
        return new HtmlString(Html);
    }

    /// <summary>
    /// Builds the checkbox selection for external database inclusion.
    /// </summary>
    /// <returns>User search settings html code</returns>
    public static IHtmlString IncludeExternalDbForm()
    {
        string Html = ScriptPack.imclude_external_db_form;

        Html = Html.Replace("{include-external}", RQResources.Views.Shared.SharedStrings.include_external);
        Html = Html.Replace("{select-databases}", RQResources.Views.Shared.SharedStrings.select_databases);
        return new HtmlString(Html);
    }

    /// <summary>
    /// (optional) Builds a table styling css block. Could be used as starting point for custom styles. Optional, put in head section.
    /// </summary>
    /// <returns>(optional) A basic css style block for the user table.</returns>
    public static IHtmlString TableCss()
    {
        var builder = new StringBuilder();

        builder.AppendFormat(_styleSheetBlock, ScriptPack.tablesorter_style);
        return new HtmlString(builder.ToString());
    }

    /// <summary>
    /// (recommended) Builds a basic layout style css block. It mainly contains css for the info-message overlay and some form styling.
    /// </summary>
    /// <returns>(recommended) Basic style css block</returns>
    public static IHtmlString StyleCss()
    {
        var styleBlock = string.Format(_styleSheetBlock, ScriptPack.style);

        return new HtmlString(styleBlock);
    }
  }
}
