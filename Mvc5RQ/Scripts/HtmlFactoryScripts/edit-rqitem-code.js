var saveData = null;
var verb = null;
var itemId = "";
var compiledTemplate = "";

$(function () {
    var editForm = new EditForm();

    compiledTemplate = $.templates($("#tocTemplate").html());
    editForm.init();
});

function EditForm() {
    this.init = function () {
        if (!$(".edit-form"))
            return;
        else {
            $("#btn-submit-item").click(submitItem);
            $("#btn-reset").click(reset);
            $("#tocAttach").click(attachTOC);
            verb = edittype();
            if (verb == "edit" || verb == "copy") {
                itemId = rqitemid();
                getItem();
            }
        }
    }

    function getItem() {
        var fd = new ajaxLoadingIndicator("#html");
        
        $.ajax({
            beforeSend: function (req) {
                req.setRequestHeader("Accept", "application/json");
            },
            url: HostAdress() + "/rqds/rqitems/" + itemId + "/rqi?verb=edititem",
            type: "GET",
            data: null,
            dataType: "json",
            success: function (data) {
                Data2Form(data);
                fd.remove();
            },
            error: function (response) {
                _myHelper.processServerResponse(response, null, function () {
                    $("#EditDialog").html("<p><span class='ui-icon ui-icon-alert' style='float: left; margin: 0 7px 20px 0;'></span><span id='edit-dialog-message'>Der Editiervorgang kann nicht durchgeführt werden !</span></p>");
                    $(function () {
                        fd.remove();
                        $("#EditDialog").dialog({
                            title: "Schwerwiegender Fehler !",
                            width: 600,
                            resizable: false,
                            modal: true,
                            buttons: {
                                OK: function () {
                                    $(this).dialog("close");
                                    window.location.replace(HostAdress() + "/RQItems?d=" + itemId);
                                }
                            }
                        });
                    });
                });
            }
        });
    }

    /* Submits the item to server if all data fields are valid */
    function submitItem(e) {
        var json = JSON.stringify(Form2Data());
        var url = HostAdress() + "/rqds/rqitems" + ((verb == "edit") ? "/" + itemId : ""); // + "?verb=" + ((verb == "edit") ? "update" : "new");
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
                    $("#EditDialog").html("<p><span class='ui-icon ui-icon-alert' style='float: left; margin: 0 7px 20px 0;'></span><span id='edit-dialog-message'>Wählen Sie den nächsten Bearbeitungsschritt.</span></p>");
                    $(function () {
                        fd.remove();
                        $("#EditDialog").dialog({ title: "Weiter",
                            width: 600,
                            resizable: false,
                            modal: true,
                            buttons: {
                                "Editieren": function () {
                                    verb = "edit";
                                    Data2Form(data);
                                    $(this).dialog("close");
                                },
                                "Kopieren": function () {
                                    verb = "copy";
                                    Data2Form(data);
                                    $(this).dialog("close");
                                },
                                "Neu": function () {
                                    verb = "new"
                                    $('#edit-rqitem-form')[0].reset();
                                    $("#idval").html("");
                                    $("#doknrval").html("");
                                    saveData = Form2Data;
                                    $(this).dialog("close");
                                },
                                Cancel: function () {
                                    $(this).dialog("close");
                                    itemId = data.DocNo;
                                    window.location.replace(HostAdress() + "/RQItems?d=" + itemId + ((verb == "edit") ? "" : "&queryString=$recent$recent additions"));
                                }
                            }
                        });
                    });
                });
            },
            error: function (response) {
                _myHelper.processServerResponse(response, null, function () {
                    $("#EditDialog").html("<p><span class='ui-icon ui-icon-alert' style='float: left; margin: 0 7px 20px 0;'></span><span id='edit-dialog-message'>Beim Speichern des Datensatzes ist der folgende Fehler aufgetreten:</p><p>" + response.responseText.replace(/\+/g, " ") + "</p></span>");
                    $(function () {
                        fd.remove()
                        $("#EditDialog").dialog({ title: "Schwerwiegender Fehler !",
                            width: 600,
                            resizable: false,
                            modal: true,
                            buttons: {
                                OK: function () {
                                    $(this).dialog("close");
                                    window.location.replace(HostAdress() + "/RQItems?d=" + itemId);
                                }
                            }
                        });
                    });
                });
            }
        });
        return false;
    }

    function Data2Form(data) {
        saveData = data;
        reset();
    }

    function Form2Data() {
        return { ID: $("#idval").html(),
                 DocNo: $("#doknrval").html(),
                 Title: $("#Title").val(),
                 Authors: $("#Authors").val(),
                 Source: $("#Source").val(),
                 Edition: $("#Edition").val(), 
                 ISDN: $("#ISDN").val(),
                 Coden: $("#Coden").val(),
                 Institutions: $("#Institutions").val(),
                 Series: $("#Series").val(),
                 Locality: $("#Locality").val(),
                 Publisher: $("#Publisher").val(),
                 PublTime: $("#PublTime").val(),
                 Volume: $("#Volume").val(),
                 Issue: $("#Issue").val(),
                 Pages: $("#Pages").val(),
                 Language: $("#Language").val(),
                 Signature: $("#Signature").val(),
                 DocTypeCode: $("#DocTypeCode").val(),
                 DocTypeName: $("#DocTypeName").val(),
                 IndexTerms: $("#IndexTerms").val(),
                 Subjects: $("#Subjects").val(),
                 WorkType: $("#WorkType").val(),
                 AboutLocation: $("#AboutLocation").val(),
                 AboutTime: $("#AboutTime").val(),
                 AboutPersons: $("#AboutPersons").val(),
                 ClassificationFieldContent: $("#ClassificationFieldContent").val(),
                 Abstract: $("#Abstract").val() + readTOC(),
                 CreateLocation: $("#CreateLocation").val(),
                 CreateTime: $("#CreateTime").val(),
                 Notes: $("#Notes").val()
               };
    }
}

// Javascript utility functions
// Get query parameters from URL
function getUrlVar(key) {
    var result = new RegExp(key + "=([^&]*)", "i").exec(window.location.search);
    return result && unescape(result[1]) || "";
}

function cancel() {
    window.location.replace(HostAdress() + "/RQItems?d=" + itemId);
}

function reset() {
    if (verb == "edit") {
        $("#idval").html(saveData.ID);
        itemId = saveData.DocNo;
        $("#doknrval").html(saveData.DocNo);
    } else {
        $("#idval").html("");
        $("#doknrval").html("");
    };
    $("#Title").val(saveData.Title);
    $("#Authors").val(saveData.Authors);
    $("#Source").val(saveData.Source);
    $("#Edition").val(saveData.Edition);
    $("#ISDN").val(saveData.ISDN);
    $("#Coden").val(saveData.Coden);
    $("#Institutions").val(saveData.Institutions);
    $("#Series").val(saveData.Series);
    $("#Locality").val(saveData.Locality);
    $("#Publisher").val(saveData.Publisher);
    $("#PublTime").val(saveData.PublTime);
    $("#Volume").val(saveData.Volume);
    $("#Issue").val(saveData.Issue);
    $("#Pages").val(saveData.Pages);
    $("#Language").val(saveData.Language);
    $("#Signature").val(saveData.Signature);
    $("#DocTypeCode").val(saveData.DocTypeCode);
    $("#DocTypeName").val(saveData.DocTypeName);
    $("#IndexTerms").val(saveData.IndexTerms);
    $("#Subjects").val(saveData.Subjects);
    $("#WorkType").val(saveData.WorkType);
    $("#AboutLocation").val(saveData.AboutLocation);
    $("#AboutTime").val(saveData.AboutTime);
    $("#AboutPersons").val(saveData.AboutPersons);
    $("#ClassificationFieldContent").val(saveData.ClassificationFieldContent);
    $("#Abstract").val(saveData.Abstract);
    $("#Abstract").val(function () {
        if (saveData.Abstract.indexOf("$$TOC$$") != -1)
            return displayTOC(saveData.Abstract);
        else
            hideTOC();
        return saveData.Abstract;
    });
    $("#CreateLocation").val(saveData.CreateLocation);
    $("#CreateTime").val(saveData.CreateTime);
    $("#Notes").val(saveData.Notes);
}

function displayTOC(strWithToc) {
    var toc = strWithToc.substr(strWithToc.indexOf("$$TOC$$=") + "$$TOC$$=".length) + " ";
    var tocItemList = [];
    var index = 1;

    while (toc != "")
    {
        var p1 = toc.indexOf("MyMusic=");

        if (p1 == -1) p1 = toc.indexOf("MyVideo=");
        if (p1 == -1) p1 = toc.indexOf("MyDoc="); 
        if (p1 != -1) {
            var p2 = toc.indexOf("; ") != -1 ? toc.indexOf("; ") : -1;
            var l = ("; ").length;

            tocItemList.push({
                position: index++,
                name: toc.substring(0, p1),
                adress: toc.substring(p1, p2)
            });
            toc = toc.substr(p2 + l);
        }
        else {
            toc = "";
        }
    }
    $(".toc").css("display", "block");
    $("#tocAttach").prop("value", "detach object").attr("title", "Not yet implemented.").attr("disabled", true);
    $("#tocList").html(compiledTemplate.render(tocItemList));
    return strWithToc.substring(0,strWithToc.indexOf("$$TOC$$="));
}

function hideTOC() {
    $(".toc").css("display", "none");
    $("#tocAttach").prop("value", "attach object").attr("title", "Attach a digital object to this item.").attr("disabled", false);
    $("#tocList").html();
}

function readTOC() {
    if ($(".toc").css("display") == "block") {
        var tocStr = "";
        var tocItemList = [];

        $("#tocList li").each(function (index) {
            tocItemList.push({
                position: $(this).find(".tocitemposition")[0].value,
                name: $(this).find(".tocitemname")[0].value,
                adress: $(this).find(".tocitemadress")[0].value
            });
        });
        tocItemList.sort(function (a, b) {
            return a.position-b.position;
        });
        for (var i = 0; i < tocItemList.length; i++) {
            var str = "";
            
            str = tocItemList[i].name + tocItemList[i].adress;
            if (str != "")
                tocStr += str + "; "
        }  
        return (tocStr != "") ? "$$TOC$$=" + tocStr : tocStr;
    }
    else   
        return "";
}

function attachTOC(e) {
    $("#AttachTocDialog").dialog({ title: "Attach Digital Object",
        width: 600,
        resizable: false,
        modal: true,
        buttons: {
            OK: function () {
                $(this).dialog("close");
                var adr = $("#ObjAdress").prop("value");
                var type = $("form input[name=DigitalObjectType]:checked").val()
                var url = HostAdress() + "/rqdos/" + type + "/" + adr;
                var fd = new ajaxLoadingIndicator("#html");

                $.ajax({
                    beforeSend: function (req) {
                        req.setRequestHeader("Accept", "application/json");
                    },
                    url: url,
                    type: "GET",
                    dataType: "json",
                    success: function (data) {
                        fd.remove();
                        displayTOC(data.toc);
                    },
                    error: function (xhr) {
                        fd.remove();
                        _myHelper.showMessage(decodeURIComponent(xhr.responseText).replace(/\+/g, ' '), "error");
                    }
                });
            },
            Cancel: function () {
                $(this).dialog("close");
            }
        }
    });
}