$(function () {
    getRQItem(docNo);
});

function renderItemContent() {
    $("#jquery_jplayer_1").jPlayer({
        ready: function () {
            $(this).jPlayer("setMedia", {
                m4v: docAdr,
                ogv: "http://www.jplayer.org/video/ogv/Big_Buck_Bunny_Trailer_480x270.ogv",
                poster: "http://www.jplayer.org/video/poster/Big_Buck_Bunny_Trailer_480x270.png"
            }).jPlayer("play");
        },
        error: function (event) {
            alert(event.jPlayer.error + " // " + event.jPlayer.error.type);
        },
        swfPath: "/areas/digitalobjects/scripts",
        supplied: "m4v, ogv" 
    });
}

function renderBibInfo(data) {
    $("#bib_info_area").html(data.Title + ". - " + data.Locality + ": " + data.Publisher + ", " + data.PublTime);
}

function getRQItem(docno) {
    if ($("#bib_info_area").is(":empty")) {
        $("#bib_info_area").html("<div id='loading'><img src='" + HostAdress() + "/images/ajax-loader.gif' alt='Please Wait' /></div>");
        $.ajax({
            url: HostAdress() + "/RQItems/" + docno,
            type: "GET",
            data: null,
            dataType: "json",
            success: function (data) {
                renderBibInfo(data);
                renderItemContent();
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