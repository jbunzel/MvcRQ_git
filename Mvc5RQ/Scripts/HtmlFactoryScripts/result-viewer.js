function getResultList() {
    callAjax_ResultList(HostAdress() + "/rqds/rqitems/rqi?verb=" + getrqitemverb); //ListViewState");
}

function exportResultList() {
    var json = ""; //JSON.stringify(Form2Data());
    var url = HostAdress() + "/rqds/rqitems/export";
    var fd = new ajaxLoadingIndicator("#html"); 
        
    $.ajax({
        beforeSend: function (req) {
            req.setRequestHeader("Accept", "application/json");
        },
        url: url,
        type: 'POST',
        dataType: 'json',
        data: json,
        contentType: 'application/json; charset=utf-8',
        success: function (data, textStatus, jqXHR) {
            var response = { isSuccess: true,
                message: (verb == "edit") ? "Der Datensatz wurde aktualisiert." : "Ein neuer Datensatz wurde angelegt."
            }
            _myHelper.processServerResponse(response, function () {
            });
        },
        error: function (response) {
            _myHelper.processServerResponse(response, null, function () {
            });
        }
    });
    return false;
}

function getQueryResultList(queryString) {
    callAjax_ResultList(HostAdress() + "/rqds/rqitems/rqi?verb=ListViewState&queryString=" + queryString);
}

function getOldRQItem(docno) {
    var eid = $('div[DocNo =' + docno + ']').attr("id");

    getRQItem(docno, eid, "o" + eid);
}

function renderHtmlList(data) {
    var optionsValues = "";
    
    $(".content-box").html(data);
    $(".category").each(function () {
        optionsValues += '<option value="'+$(this).attr("pos") + '">' + $(this).text() + '</option>';
    });
    $('#segmentSelector').html(optionsValues);
    $(".result_segments").show();
    $('#paging_container').pajinate({
        nav_label_first: '<<',
        nav_label_last: '>>',
        nav_label_prev: '<',
        nav_label_next: '>',
        num_page_links_to_display: 5,
        items_per_page: 15
    });
    resizeToWindowHeight();
}

function renderHtml(data, domElementID) {
    $("#" + domElementID).html(data);
}

function renderXmlList(data, domElementID) {
    var xml = window.ActiveXObject ? data.xml : (new XMLSerializer().serializeToString(data));

    $("#" + domElementID).transform({
        xmlstr: xml, 
        xsl: HostAdress() + "/xslt/ViewTransforms/RQI2SingleItemView.xslt", 
        //success: function(){$(".select-predicates").on("click", selectPredicates)}
    });
 }

function selectPredicates(elementId, subjectId) {
    var $clickedLink = $("#" + elementId);

    // hide any other select-predicates-dialog
    $(".select-predicates-dialog").remove();
    // ask server about available linked data predicates
    $.ajax({
        beforeSend: function (req) {
            req.setRequestHeader("Accept", "application/json");
        },
        url: HostAdress() + "/LinkedDataCalls/GetLinkedDataPredicates/" + subjectId,
        type: "GET",
        data: null,
        dataType: "json",
        success: function (response) {
            _myHelper.processServerResponse(response, function () {
                //build select databases dialog
                var $dlg = $("<div/>").addClass("select-database-dialog").hide();
                var $predicateList = $("<ul />").addClass("role-list");
                var predicateInfo = response.data;

                for (var i in predicateInfo) {
                    var predicatename = predicateInfo[i].predicatename;
                    var objectvalue = predicateInfo[i].objectvalue;
                    var cbxId = "cbx-" + predicatename;
                    //create a list item, checkbox and label for each external database
                    var $li = $("<li />");
                    var $cbx = $("<input type='checkbox' class='cbx-role' />").val(predicatename).attr("checked", predicateInfo[i].included).attr("id", cbxId);
                    var $label = $("<label />").text(predicatename).attr("for", cbxId);
                    var $value = $("<value />").text(objectvalue);
                    // append checkbox and label to list item
                    $li.append($cbx).append($label).append($value).appendTo($predicateList);
                }
                //append database list to dialog
                $dlg.append($predicateList);
                $clickedLink.after($dlg);
                $dlg.fadeIn();
                //bind add or remove external database for the created checkboxes
                $dlg.find(".cbx-role").change(function (e) {
                    e.stopPropagation();

                    var data = {
                        predicatename: $(this).val(),
                        included: $(this).is(":checked")
                    };
                });
                //hide select-database-dialog on document click but not on a click inside the dialog
                $dlg.on("click", function (e) { e.stopPropagation() });
                $(document).one("click", function (e) { $dlg.fadeOut() });
            });
        },
        error: function (xhr) {
            _myHelper.showMessage(decodeURIComponent(xhr.responseText).replace(/\+/g, ' '), "error");
        }
    });

    //$.post(HostAdress() + "/LinkedDataCalls/GetLinkedDataPredicates/" + $(e.target).attr("id"), {}, function (response) {
    //$.post(HostAdress() + "/LinkedDataCalls/GetLinkedDataPredicates/" + subjectId, {}, function (response) {
    //    debugger;
    //    // on success do...
    //    _myHelper.processServerResponse(response, function () {
    //        //build select databases dialog
    //        var $dlg = $("<div/>").addClass("select-predicates-dialog").hide();
    //        var $predicateList = $("<ul />").addClass("predicate-list");
    //        var predicateInfo = response.data;

    //        for (var i in predicateInfo) {
    //            var predicatename = predicateInfo[i].predicatename;
    //            var cbxId = "cbx-" + predicatename;
    //            //create a list item, checkbox and label for each external database
    //            var $li = $("<li />");
    //            var $cbx = $("<input type='checkbox' class='cbx-role' />").val(predicatename).attr("checked", predicateInfo[i].included).attr("id", cbxId);
    //            var $label = $("<label />").text(predicatename).attr("for", cbxId);
    //            // append checkbox and label to list item
    //            $li.append($cbx).append($label).appendTo($predicateList);
    //        }
    //        //append database list to dialog
    //        $dlg.append($predicateList);
    //        $clickedLink.after($dlg);
    //        $dlg.fadeIn();
    //        //bind add or remove external database for the created checkboxes
    //        $dlg.find(".cbx-role").change(function (e) {
    //            e.stopPropagation();
                
    //            var data = {
    //                predicatename: $(this).val(),
    //                included: $(this).is(":checked")
    //            };
    //        });
    //        //hide select-database-dialog on document click but not on a click inside the dialog
    //        $dlg.on("click", function (e) { e.stopPropagation() });
    //        $(document).one("click", function (e) { $dlg.fadeOut() });
    //    });
    //});
    //return false;
}

function renderFieldContent(data, domElementID) {
    var xml = window.ActiveXObject ? data.xml : (new XMLSerializer().serializeToString(data));
    
    $("#" + domElementID).transform({ xmlstr: xml, xsl: HostAdress() + "/xslt/ViewTransforms/RQItemFields.xslt" });
}

function getRQItem(docno, newDomElementID, oldDomElementID) {
    if ($("#" + newDomElementID).is(":empty")) {
        acceptedType = "html" // set xml for client transformation to html (does not work in  MSIE Version >= 10 : local $.browser.msie ? "html" : "xml";)
        $("#" + newDomElementID).html("<div id='loading'><img src='" + HostAdress() + "/images/ajax-loader.gif' alt='Please Wait' /></div>");
        $.ajax({
            beforeSend: function (req) {
                req.setRequestHeader("Accept", "application/x-riquest-internal");
            },
            url: HostAdress() + "/rqds/rqitems/" + docno + "/rqi?verb=" + getrqitemverb,
            type: "GET",
            data: null,
            dataType: acceptedType,
            success: function (data) {
                if (acceptedType == "xml")
                    renderXmlList(data, newDomElementID);
                else {
                    renderHtml(data, newDomElementID);
                    //$(".select-predicates").on("click", selectPredicates);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(decodeURIComponent(xhr.responseText).replace(/\+/g, ' '));
                $("#" + newDomElementID).html("");
            }
        });
    }
    $("#" + oldDomElementID).toggle();
    $("#" + newDomElementID).toggle("slow");
}

function call(path) {
    window.open(HostAdress() + "/" + path,"_self");
}

function callAjax(path, domElementID) {
    debugger;
    $.ajax({
        url: HostAdress() + "/" + path,
        type: "GET",
        data: null,
        dataType: "xml",
        success: function (data) {
            renderFieldContent(data, domElementID);
        }
    });
}

function callAjax_ResultList(targetUrl) {
    var fd = new ajaxLoadingIndicator("#html");
    
    $.ajax({
        beforeSend: function (req) {
            req.setRequestHeader("Accept", "application/x-riquest-internal");
        },
        url: targetUrl,
        type: "GET",
        data: null,
        success: function (data) {
            renderHtmlList(data);
            if (docNo != "" && docNo != null) {
                setTimeout(function () {
                    getOldRQItem(docNo);
                }, 1)
            }
            else
                setTimeout(function () {
                    getOldRQItem($("div", data).attr("docno"));
                }, 1)
            fd.remove();
            resizeToWindowHeight();
        },
        error: function (xhr) {
            fd.remove();
            _myHelper.showMessage(decodeURIComponent(xhr.responseText).replace(/\+/g, ' '), "error");
        }
    });
}

function visibilityToggle(id) {
    $("#" + id).slideToggle("slow");
}

function cleanUrl() {
    var d = document.location.href;
    if (d.indexOf("?") != -1) {
        return d.substring(0, d.indexOf("?"))
    }
    else {
        return d;
    }
}

var thumbNail;

function checkAvail(id, isbn) {
    thumbNail = id;
    var jsonScript = document.getElementById("jsonScript");
    if (jsonScript) {
        jsonScript.parentNode.removeChild(jsonScript);
    }
    var scriptElement = document.createElement("script");
    scriptElement.setAttribute("id", "jsonScript");
    scriptElement.setAttribute("src","http://books.google.com/books?bibkeys=" + escape(isbn) + "&jscmd=viewapi&callback=showThumbNail");
    //scriptElement.setAttribute("src", "https://openlibrary.org/api/books?bibkeys=" + escape(isbn) + "&jscmd=viewapi&callback=showThumbNail");
    scriptElement.setAttribute("type", "text/javascript");
    document.documentElement.firstChild.appendChild(scriptElement);
}

function showThumbNail(booksInfo) {
    var divList = document.getElementsByName(thumbNail);
    var div;

    if (divList.length > 1) {
        div = divList[divList.length - 1];
        div.id = thumbNail + Math.floor((Math.random() * 100) + 1);
    }
    else {
        div = document.getElementById(thumbNail);
        if (div.firstChild) div.removeChild(div.firstChild);
    }

    var mainDiv = document.createElement("div");

    for (i in booksInfo) {
        var book = booksInfo[i];
        if ((book.preview != "noview") || (book.thumbnail_url)) {
            var thumbnailDiv = document.createElement("div");

            thumbnailDiv.className = "thumbnail";

            var a = document.createElement("a");

            a.href = book.info_url;
            a.target = "_new";
            if (book.thumbnail_url) {
                var img = document.createElement("img");

                img.src = book.thumbnail_url;
                if (book.preview == "noview")
                    thumbnailDiv.appendChild(img);
                else
                    a.appendChild(img);
            }
            if (book.preview != "noview")
                thumbnailDiv.appendChild(a);
            mainDiv.appendChild(thumbnailDiv);
            break;
        }
    }
    div.appendChild(mainDiv);
}