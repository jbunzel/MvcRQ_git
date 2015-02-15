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
        $.getScript(HostAdress() + "/areas/digitalobjects/scripts/jplayer.playlist.js", function (data, textStatus, jqxhr) {
            var cssSelector = {
                jPlayer: "#jquery_jplayer_1",
                cssSelectorAncestor: "#jp_container_1"
            };
            var playlist = playList //[]; // Empty playlist
            var options = {
                autoPlay: true,
                swfPath: "/areas/digitalobjects/scripts",
                supplied: "mp3"
            };
            var myPlaylist = new jPlayerPlaylist(cssSelector, playlist, options);

            $(".jp-title ul li").show();     // TODO: Track-Titel anzeigen
            $(".jp-title ul li").text("");
            $("#jquery_jplayer_1").bind($.jPlayer.event.play, function (event) {
                var current = myPlaylist.current; //This is an integer which represents the index of the array object currently being played.
                var playlist = myPlaylist.playlist //This is an array, which holds each of the set of the items youve defined (e.q. title, mp3, artist etc...)

                $.each(playlist, function(index, object) { //$.each is a jQuery iteration method which lets us iterate over an array(playlist), so we actually look at playlist[index] = object
                    if(index == current) {   
                        $(".jp-song-title").html(object.title);
                    }
                });
            });
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