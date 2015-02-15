$(function () {
    getRQItem(docNo);
});

function renderItemContent() {
    var w = $('#viewer_area').width();
    var h = $('#viewport').height();

    $('#viewer_area').media({
        width: w,
        height: h,
        autoplay: true,
        src: docAdr,
        attrs: { attr1: 'attrValue1', attr2: 'attrValue2' },  // object/embed attrs 
        params: { param1: '', param2: 'paramValue2' }, // object params/embed attrs 
        caption: false // supress caption text 
    }, function (t, p) {
    }, function (t, d, o, p) {
    }
);
}

function renderBibInfo(data) {
    $("#bib_info_area").html(data.Title + ". - " + data.Locality + ": " + data.Publisher + ", " + data.PublTime);
}

function getRQItem(docno) {
    if ($("#bib_info_area").is(":empty")) { // not empty if content has been already written by Index.cshtml 
        var acceptedType = "html" // set json for client transformation to html

        $("#bib_info_area").html("<div id='loading'><img src='" + HostAdress() + "/images/ajax-loader.gif' alt='Please Wait' /></div>");
        $.ajax({
            url: HostAdress() + "/RQItems/" + docno,
            type: "GET",
            data: null,
            dataType: "html",
            success: function (data) {
                debugger;
                if (acceptedType == "json") {
                    renderBibInfo(data);
                    renderItemContent();
                }
                else {
                    $("#bib_info_area").html(data);
                    renderItemContent();
                }
            },
            error: function (xhr) {
                _myHelper.processServerResponse(xhr.responseJSON, null, function () {
                    $("#bib_info_area").html("");
                });
            }
        });
    }
    else
        renderItemContent();
}