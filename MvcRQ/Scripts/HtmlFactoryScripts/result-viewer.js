function getResultList() {
    var c = cleanUrl() + "?verb=QueryList";
    var fd = new ajaxLoadingIndicator("#html");
    
    $.ajax({
        url: c,
        type: "GET",
        data: null,
        dataType: "html",
        success: function (data) {
            renderHtmlList(data);
            if (docNo != "") {
                setTimeout(function () {
                    getOldRQItem(docNo);
                }, 1)
            };
            fd.remove();
            resizeToWindowHeight();
        },
        error: function (xhr) {
            _myHelper.showMessage(decodeURIComponent(xhr.responseText).replace(/\+/g, ' '), "error");
        }
    });
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
        success: function(){$(".select-predicates").on("click", selectPredicates)}
    });
 }

function selectPredicates(e) {
    alert("TEST");
    // hide any other select-database-dialog
    $(".select-databases-dialog").remove();
    var $clickedLink = $(this);
    // ask server which external databases the user links to
    $.post(HostAdress() + "/LinkedDataCalls/GetLinkedDataPredicates/" + $(e.target).attr("id"), {}, function (response) {
        // on success do...
        _myHelper.processServerResponse(response, function () {
            //build select databases dialog
            var $dlg = $("<div/>").addClass("select-predicates-dialog").hide();
            var $predicateList = $("<ul />").addClass("predicate-list");
            var predicateInfo = response.data;

            for (var i in predicateInfo) {
                var predicatename = predicateInfo[i].predicatename;
                var cbxId = "cbx-" + predicatename;
                //create a list item, checkbox and label for each external database
                var $li = $("<li />");
                var $cbx = $("<input type='checkbox' class='cbx-role' />").val(predicatename).attr("checked", predicateInfo[i].included).attr("id", cbxId);
                var $label = $("<label />").text(predicatename).attr("for", cbxId);
                // append checkbox and label to list item
                $li.append($cbx).append($label).appendTo($predicateList);
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
    });
    return false;
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
            url: HostAdress() + "/RQItems/" + docno + "?verb=" + getrqitemverb,
            type: "GET",
            data: null,
            dataType: acceptedType,
            success: function (data) {
                if (acceptedType == "xml")
                    renderXmlList(data, newDomElementID);
                else {
                    renderHtml(data, newDomElementID);
                    $(".select-predicates").on("click", selectPredicates);
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