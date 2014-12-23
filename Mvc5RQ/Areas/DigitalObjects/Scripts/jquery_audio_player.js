$(function () {
    getRQItem(docNo);
});

function renderItemContent() {
    if (docAdr != "PLAYLIST") {
        $("#jquery_jplayer_1").jPlayer({
            ready: function () {
                $(".jp-title ul li").text(""); //TODO: Track-Titel anzeigen
                $(this).jPlayer("setMedia", {
                    mp3: docAdr
                }).jPlayer("play");
            },
            swfPath: "/js",
            supplied: "mp3",
            errorAlerts: false,
            warningAlerts: false
        });
    }
    else {
        $.getScript(HostAdress() + "/Areas/ItemViewer/Scripts/jplayer.playlist.js", function (data, textStatus, jqxhr) {
            var cssSelector = { jPlayer: "#jquery_jplayer_1",
                cssSelectorAncestor: "#jp_container_1"
            };
            var playlist = playList //[]; // Empty playlist
            var options = { swfPath: "/js",
                supplied: "mp3"
            };
            var myPlaylist = new jPlayerPlaylist(cssSelector, playlist, options);

            $(".jp-title ul li").show();     // TODO: Track-Titel anzeigen
            $(".jp-title ul li").text("");
            myPlaylist.play(0);
        });
    }
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
            error: function (xhr, ajaxOptions, thrownError) {
                alert(decodeURIComponent(xhr.responseText).replace(/\+/g, ' '));
                $("#bib_info_area").html("");
            }
        });
    }
    else
        renderItemContent();
}