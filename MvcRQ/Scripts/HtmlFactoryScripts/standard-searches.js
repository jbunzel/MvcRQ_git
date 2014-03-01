$(document).ready(function () {
    $(".standard-searches a").on("click", function (e) {
        callAjax_ResultList($(this).attr("href") + "&verb=QueryList");
        $("#queryString").val($(this).html());
        return false;
    });
});