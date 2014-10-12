$(document).ready(function () {
    $(".search-box").on("submit", function (e) {
        //callAjax_ResultList(cleanUrl() + "?verb=QueryList&queryString=" + encodeURIComponent($("#queryString").val()));
        callAjax_ResultList(HostAdress() + "/rqds/rqitems/rqi?verb=ListViewState&queryString=" + encodeURIComponent($("#queryString").val()));
        return false;
    });
});