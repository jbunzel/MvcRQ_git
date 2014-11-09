var myInterval = false; // this variable will hold the interval and work as a flag
var $win = $(window); //jquery win object
var dimensions = [$win.width(), $win.height()]; //initial dimensions

$(function () {
    $(window).on("debouncedresize", function (event) {
        dimensions[0] = $win.width(); //else keep the new dimensions
        dimensions[1] = $win.height();
        resizeToWindowHeight();
    });
});

function resizeToWindowHeight() {
    var height = dimensions[1] - 60;
    var nav = $("#nav").outerHeight(true);
    var footer = $("#footer").outerHeight(true);
    var page_navigation = $(".page_navigation").outerHeight(true);
    
    $("#content").css("min-height", height + "px");
    $(".content-box").css("max-height", (height - nav) + "px");
    $(".content-box").css("min-height", (height - nav) + "px");
    $(".rq-result-box .container").css("max-height", (height - nav + page_navigation) + "px");
    $(".rq-result-box .container").css("min-height", (height - nav + page_navigation) + "px");
    $(".rq-sidebar, .rq-left-sidebar, .rq-right-sidebar, .rq-classtree-sidebar").css("max-height", (height - nav - footer) + "px");
    $(".rq-sidebar, .rq-left-sidebar, .rq-right-sidebar, .rq-classtree-sidebar").css("min-height", (height - nav - footer) + "px");
};

/*
 * debouncedresize: special jQuery event that happens once after a window resize
 *
 * latest version and complete README available on Github:
 * https://github.com/louisremi/jquery-smartresize
 *
 * Copyright 2012 @louis_remi
 * Licensed under the MIT license.
 *
 * This saved you an hour of work? 
 * Send me music http://www.amazon.co.uk/wishlist/HNTU0468LQON
 */
(function ($) {


    var $event = $.event,
        $special,
        resizeTimeout;


    $special = $event.special.debouncedresize = {
        setup: function () {
            $(this).on("resize", $special.handler);
        },
        teardown: function () {
            $(this).off("resize", $special.handler);
        },
        handler: function (event, execAsap) {
            // Save the context
            var context = this,
                args = arguments,
                dispatch = function () {
                    // set correct event type
                    event.type = "debouncedresize";
                    $event.dispatch.apply(context, args);
                };


            if (resizeTimeout) {
                clearTimeout(resizeTimeout);
            }


            execAsap ?
                dispatch() :
                resizeTimeout = setTimeout(dispatch, $special.threshold);
        },
        threshold: 150
    };


})(jQuery);

function ajaxLoadingIndicator(el) {
    this.init = function () {
        $('.rq-ajax-wait').css("width", $(window).width());
        $('.rq-ajax-wait').css("height", $(window).height());
        $('.rq-ajax-wait').fadeIn(800);
    }

    this.remove = function () {
        $('.rq-ajax-wait').fadeOut(800);
    }

    this.init();
}

//Ajax overlay 1.0
// Author: Simon Ilett(at)aplusdesign.com.au
//* Descrip: Creates and inserts an ajax loader for ajax calls / timed events 
//* Date: 03/08/2011 
//function ajaxLoader(el, options) {
    //// Becomes this.options
    //var defaults = {
    //    bgColor: 'green',
    //    duration: 800,
    //    opacity: 0.7,
    //    classOveride: false
    //}
    
    //this.options = jQuery.extend(defaults, options);
    //this.container = $(el);
    //this.init = function () {
    //    var container = this.container;
    //    // Delete any other loaders
    //    this.remove();
    //    // Create the overlay 
    //    var overlay = $('<div></div>').css({
    //        'background-color': this.options.bgColor,
    //        'opacity': this.options.opacity,
    //        'width': container.width(),
    //        'height': container.height(),
    //        'position': 'absolute',
    //        'top': '0px',
    //        'left': '0px',
    //        'z-index': 99999
    //    }).addClass('ajax_overlay');
    //    // add an overiding class name to set new loader style 
    //    if (this.options.classOveride) {
    //        overlay.addClass(this.options.classOveride);
    //    }
    //    // insert overlay and loader into DOM 
    //    container.append(
    //        overlay.append(
    //            $('<div></div>').addClass('ajax_loader')
    //        ).fadeIn(this.options.duration)
    //    );
    //};
    
    //this.remove = function () {
    //    var overlay = this.container.children(".ajax_overlay");
        
    //    if (overlay.length) {
    //        overlay.fadeOut(this.options.classOveride, function () {
    //            overlay.remove();
    //        });
    //    }
    //}

    //this.init();
//}

var _myHelper = {
    /* helper function for processing the server response. Triggers either an error or success message window 
    /* and calls provided functions if neccessary. */
    processServerResponse: function (response, onSuccess, onError) {
        if (response.isSuccess) {
            if (response.message) // show message only if available
                _myHelper.showSuccess(response.message);
            // call success callback
            if ($.isFunction(onSuccess))
                onSuccess.apply();
        } else {
            //show error message
            var errMsg = "";

            if (response.message) // show message only if available
                errMsg = response.message;
            if (response.responseText)
                errMsg += ((errMsg != "") ? "<br>" : "") + response.responseText.replace(/\+/g, " ");
            if (errMsg != "")
                _myHelper.showError(errMsg, "error");
            if ($.isFunction(onError))
                onError.apply();
        }
    },

    showSuccess: function (msg) { _myHelper.showMessage(msg, "success"); },

    showError: function (msg) { _myHelper.showMessage(msg, "error"); },

    /* returns the info window element and creates one if it is not yet existing */
    infoWindow: function () {
        //remove old info window
        $("#simple-user-info").remove();
        //add new one
        return $("<div/>").attr("id", "simple-user-info")
               .appendTo("body");
    },

    /* Shows an error or success notification */
    showMessage: function (message, cssClass) {
        var $info = _myHelper.infoWindow().addClass(cssClass).text(message);
        //show info message
        $info.fadeIn(1000).on("mouseover", fadeOutInfoWindow);
        //hide after 10 seconds and on one document click
        setTimeout(function () { fadeOutInfoWindow(); }, 10000);
        $(document).one("click", fadeOutInfoWindow)
        //fadeOut info window
        function fadeOutInfoWindow() {
            $info.fadeOut(1000);
        }
    },

    /* Helper function for html encoding unsecure user input */
    encodeHtml: function (input) {
        return $("<div/>").text(input).html();
    }
}