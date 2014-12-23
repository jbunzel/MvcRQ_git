using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Mvc5RQ.Areas;
using System.Text.RegularExpressions;

namespace Mvc5RQ.Areas.DigitalObjects
{
    /// <summary>
    /// Static Html Generator for building the user interface html, javascript and css code.
    /// </summary>
    public static class BundleConfig
    {
        const string _styleSheetBlock = "<style type='text/css'>{0}</style>";
        const string _javascriptBlock = "<script type='text/javascript'>{0}</script>";

        /// <summary>
        /// Generates html-code for bibliographic information area.
        /// </summary>
        /// <returns>Html-code for bibliographic information area.</returns>
        public static IHtmlString ItemBibInfo()
        {
            var addItemBibInfo = ScriptPack.item_viewer_bib_info;

            return new HtmlString(addItemBibInfo);
        }

        /// <summary>
        /// Generates html-code for JPlayer video player.
        /// </summary>
        /// <returns>Html-code for JPlayer video player.</returns>
        public static IHtmlString VideoPlayerHtml()
        {
            var addItemBibInfo = ScriptPack.video_player;

            return new HtmlString(addItemBibInfo);
        }

        /// <summary>
        /// Generates html-code for JPlayer audio player.
        /// </summary>
        /// <returns>Html-code for JPlayer audio player.</returns>
        public static IHtmlString AudioPlayerHtml()
        {
            var addItemBibInfo = ScriptPack.audio_player;

            return new HtmlString(addItemBibInfo);
        }

        /// <summary>
        /// Generates html-code for JPlayer audio player & playlist.
        /// </summary>
        /// <returns>Html-code for JPlayer audio player & playlist.</returns>
        public static IHtmlString AudioPlaylistHtml()
        {
            var addItemBibInfo = ScriptPack.audio_playlist;

            return new HtmlString(addItemBibInfo);
        }

        /// <summary>
        /// Required: Builds three javascript blocks containing the tablesorter, pager plugin and the main ui/ajax javascript code.
        /// </summary>
        /// <returns>(required) Javascript code for generating the user interface.</returns>
        public static IHtmlString ItemViewerJavascript()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(_javascriptBlock, ScriptPack.jquery_item_viewer);
            builder.AppendFormat(_javascriptBlock, ScriptPack.jquery_media);

            //builder.AppendFormat(_javascriptBlock, ScriptPack.simple_user_management.Replace("{controllerName}", "UserManagement"));

            return new HtmlString(builder.ToString());
        }

        /// <summary>
        /// Required: Builds three javascript blocks containing the tablesorter, pager plugin and the main ui/ajax javascript code.
        /// </summary>
        /// <returns>(required) Javascript code for generating the user interface.</returns>
        public static IHtmlString VideoPlayerJavascript()
        {
            var builder = new StringBuilder();
            //builder.AppendFormat(_javascriptBlock, ScriptPack.jquery_jplayer);
            builder.AppendFormat(_javascriptBlock, ScriptPack.jquery_video_player);

            //builder.AppendFormat(_javascriptBlock, ScriptPack.simple_user_management.Replace("{controllerName}", "UserManagement"));

            return new HtmlString(builder.ToString());
        }

        /// <summary>
        /// Required: Builds three javascript blocks containing the tablesorter, pager plugin and the main ui/ajax javascript code.
        /// </summary>
        /// <returns>(required) Javascript code for generating the user interface.</returns>
        public static IHtmlString AudioPlayerJavascript()
        {
            var builder = new StringBuilder();
            //builder.AppendFormat(_javascriptBlock, ScriptPack.jquery_jplayer);
            builder.AppendFormat(_javascriptBlock, ScriptPack.jquery_audio_player);

            //builder.AppendFormat(_javascriptBlock, ScriptPack.simple_user_management.Replace("{controllerName}", "UserManagement"));

            return new HtmlString(builder.ToString());
        }

        /// <summary>
        /// (optional) Builds a special styling css block. Could be used as starting point for custom styles. Optional, put in head section.
        /// </summary>
        /// <returns>(optional) A basic css style block for special ui elements.</returns>
        public static IHtmlString VideoPlayerSkin()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(_styleSheetBlock, ScriptPack.jplayer_pink_flag);

            return new HtmlString(builder.ToString());
        }

        /// <summary>
        /// (optional) Builds a special styling css block. Could be used as starting point for custom styles. Optional, put in head section.
        /// </summary>
        /// <returns>(optional) A basic css style block for special ui elements.</returns>
        public static IHtmlString AudioPlayerSkin()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(_styleSheetBlock, ScriptPack.jplayer_blue_monday);

            return new HtmlString(builder.ToString());
        }

        /// <summary>
        /// (optional) Builds a special styling css block. Could be used as starting point for custom styles. Optional, put in head section.
        /// </summary>
        /// <returns>(optional) A basic css style block for special ui elements.</returns>
        public static IHtmlString SpecialCss()
        {
            var builder = new StringBuilder();
            builder.AppendFormat(_styleSheetBlock, ScriptPack.specialstyle);

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
