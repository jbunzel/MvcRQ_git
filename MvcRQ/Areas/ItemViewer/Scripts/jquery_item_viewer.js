$(function () {
    getRQItem(docNo);
});

function renderItemContent() {
    var test = Modernizr.audio.ogg;

    test = Modernizr.audio.mp3;
    test = Modernizr.video.h264;
    $('#viewer_area').media({
        width: 900,
        height: 650,
        autoplay: true,
        src: docAdr,
        attrs: { attr1: 'attrValue1', attr2: 'attrValue2' },  // object/embed attrs 
        params: { param1: 'paramValue1', param2: 'paramValue2' }, // object params/embed attrs 
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