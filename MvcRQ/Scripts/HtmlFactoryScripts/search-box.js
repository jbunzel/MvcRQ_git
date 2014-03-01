$(document).ready(function () {
    $(".search-box").on("submit", function (e) {
        //callAjax_ResultList(cleanUrl() + "?verb=QueryList&queryString=" + encodeURIComponent($("#queryString").val()));
        callAjax_ResultList(HostAdress() + "/rqitems?verb=QueryList&queryString=" + encodeURIComponent($("#queryString").val()));
        return false;
    });
});