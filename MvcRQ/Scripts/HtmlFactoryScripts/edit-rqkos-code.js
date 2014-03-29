var saveData = null;
var verb = getUrlVar('verb');
var rqkosId = '';
var compiledTemplate = '';
var editForm = new EditForm();

$(function () {
    var icons = {
        header: 'ui-icon-circle-arrow-e', 
        activeHeader: 'ui-icon-circle-arrow-s' 
    }; 

    $('#accordion').accordion({
        icons: icons,
        collapsible: true,
        beforeActivate: function(event, ui) {
            // The accordion believes a panel is being opened
            if (ui.newHeader[0]) {
                var currHeader  = ui.newHeader;
                var currContent = currHeader.next('.ui-accordion-content');
                // The accordion believes a panel is being closed
            } else {
                var currHeader  = ui.oldHeader;
                var currContent = currHeader.next('.ui-accordion-content');
            }
            // Since we've changed the default behavior, this detects the actual status
            var isPanelSelected = currHeader.attr('aria-selected') == 'true';
            
            // Toggle the panel's header
            currHeader.toggleClass('ui-corner-all',isPanelSelected).toggleClass('accordion-header-active ui-state-active ui-corner-top',!isPanelSelected).attr('aria-selected',((!isPanelSelected).toString()));
            
            // Toggle the panel's icon
            currHeader.children('.ui-icon').toggleClass('ui-icon-triangle-1-e',isPanelSelected).toggleClass('ui-icon-triangle-1-s',!isPanelSelected);
            
            // Toggle the panel's content
            currContent.toggleClass('accordion-content-active',!isPanelSelected)    
            if (isPanelSelected) { currContent.slideUp(); }  else { currContent.slideDown(); }

            return false; // Cancels the default action
        }
    }); 
    //compiledTemplate = $.templates($('#tocTemplate').html());
    editForm.init();
});

function EditForm() {
    this.init = function () {
        if ($('.edit-form')) {
            var NrOfSubforms = parseInt($('#EditForm .nrofsubclasses').html());
            
            $('#btn-submit').click(this.submit);
            $('#btn-check').click(this.check);
            $('#btn-add').click(this.add);
            $('#btn-up').click(this.up);
            $('#btn-del').click(this.delete);
            if (parseInt($('#EditForm .parentid').html()) < 0)
                $('#btn-up').attr('disabled', 'disabled');
            for (var i = 1; i <= NrOfSubforms; i++) {
                $('#btn-down' + i).click(function (event) {
                    down(event.target.id.substr(8)); //extract subform index appended to 'btn-down';
                });
                $('#EditSubForm' + i + ' .classname').keyup(function (event) {
                    $('#ClassID' + event.target.id.substr(9) + " + span").html($(this).val());
                });
            }
        }
    }

    /* Submits the item to server if all data fields are valid */
    this.submit = function (e) {
        var json = JSON.stringify(EditForm2Data());
        var url = HostAdress() + '/rqkos/' + $('.classid').html() + '?verb=update';
        var fd = new ajaxLoadingIndicator('#html'); 

        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'json',
            data: json,
            contentType: 'application/json; charset=utf-8',
            success: function (data, textStatus, jqXHR) {
                if (data.isSuccess == false) {
                    var msgHtml = HintListHtml(data.hints, data.hints.length);

                    _myHelper.processServerResponse(data, null, function () {
                        $('#EditDialog').html('<p><span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span><span id="edit-dialog-message">Der Editiervorgang kann nicht durchgeführt werden !</span></p>' + '<p>' + msgHtml + '</p>');
                        $(function () {
                            fd.remove();
                            $('#EditDialog').dialog({
                                title: 'Inkonsistente Konkordanz!',
                                width: 600,
                                resizable: false,
                                modal: true,
                                buttons: {
                                    Überarbeiten: function () {
                                        $(this).dialog('close');
                                    },
                                    Rückgängig: function () {
                                        $(this).dialog('close');
                                    }
                                }
                            });
                        });
                    });
                }
                else {
                    Data2EditForm(data);
                    fd.remove();
                    _myHelper.showSuccess('Klassendefinition und Mapping erfolgreich gespeichert');
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                fd.remove();
                _myHelper.processServerResponse(jqXHR, null, null);
            }
        });
        return true;
    }

    this.check = function () {
        var json = JSON.stringify(EditForm2Data());
        var url = HostAdress() + '/rqkos/' + $('#EditForm .classid').html() + '?verb=check';
        var fd = new ajaxLoadingIndicator('#html'); 

        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'json',
            data: json,
            contentType: 'application/json; charset=utf-8',
            success: function (data, textStatus, jqXHR) {
                fd.remove();
                if (data.isSuccess) {
                    _myHelper.processServerResponse(data);
                }
                else {
                    var msgHtml = HintListHtml(data.hints, data.hints.length);
                    _myHelper.processServerResponse(data, null, function () {
                        $('#EditDialog').html('<p><span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span><span id="edit-dialog-message">Der Editiervorgang kann nicht durchgeführt werden !</span></p>' + '<p>' + msgHtml + '</p>');
                        $(function () {
                            fd.remove();
                            $('#EditDialog').dialog({
                                title: 'Inkonsistente Konkordanz!',
                                width: 600,
                                resizable: false,
                                modal: true,
                                buttons: {
                                    Überarbeiten: function () {
                                        $(this).dialog('close');
                                    },
                                    Rückgängig: function () {
                                        $(this).dialog('close');
                                    }
                                }
                            });
                        });
                    });
                }
            },
            error: function(jqXHR, textStatus, errorThrown) {
                fd.remove();
                _myHelper.processServerResponse(jqXHR, null, null);
            }
        });
        return true;
    }

    this.add = function () {
        var json = JSON.stringify(EditForm2Data());
        var url = HostAdress() + '/rqkos/' + $('.classid').html() + '?verb=new';
        var fd = new ajaxLoadingIndicator('#html');
        var addClassHTML = GetClassTemplateHtml()

        addClassHTML = addClassHTML.replace(/§§NEW-ID§§/g, (parseInt($('#EditForm .nrofsubclasses').html()) + 1).toString());
        $('#accordion').append(addClassHTML);
        $('#accordion').accordion('refresh');
        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'json',
            data: json,
            contentType: 'application/json; charset=utf-8',
            success: function (data, textStatus, jqXHR) {
                var i = (parseInt(data[0].NrOfSubclasses) + 1).toString();

                fd.remove();
                data[0].NrOfSubclasses = i 
                Data2EditForm(data);
                $('#ClassID' + i).html("999");
                $('#ClassID' + i + " + span").html("Neue Unterklasse");
                $('#btn-down' + i).click(function (event) {
                    down(event.target.id.substr(8)); //extract subform index appended to 'btn-down';
                });
                $('#btn-down' + i).attr('disabled', 'disabled');
                $('#EditSubForm' + i + ' .classname').keyup(function (event) {
                    $('#ClassID' + event.target.id.substr(9) + " + span").html($(this).val());
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                fd.remove();
            }
        });
        return true;
    }

    this.delete = function () {
        $('#DeleteDialog').dialog({
            modal: true,
            buttons: {
                'JA': function () {
                    $(this).dialog('close');
                    var json = JSON.stringify(EditForm2Data());
                    var url = HostAdress() + '/rqkos/' + $('.classid').html() + '?verb=delete';
                    var fd = new ajaxLoadingIndicator('#html');

                    $.ajax({
                        url: url,
                        type: 'POST',
                        dataType: 'json',
                        data: json,
                        contentType: 'application/json; charset=utf-8',
                        success: function (data, textStatus, jqXHR) {
                            debugger;
                            if (data.isSuccess == false) {
                                var msgHtml = HintListHtml(data.hints, data.hints.length);

                                _myHelper.processServerResponse(data, null, function () {
                                    $('#EditDialog').html('<p><span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span><span id="edit-dialog-message">Die Klassifikationstranche konnte nicht gelöscht werden !</span></p>' + '<p>' + msgHtml + '</p>');
                                    $(function () {
                                        fd.remove();
                                        $('#EditDialog').dialog({
                                            title: 'Inkonsistente Konkordanz!',
                                            width: 600,
                                            resizable: false,
                                            modal: true,
                                            buttons: {
                                                Überarbeiten: function () {
                                                    $(this).dialog('close');
                                                },
                                                Rückgängig: function () {
                                                    $(this).dialog('close');
                                                }
                                            }
                                        });
                                    });
                                });
                            }
                            else {
                                Data2EditForm(data);
                                fd.remove();
                                _myHelper.showSuccess('Klassendefinition und Mapping erfolgreich gespeichert');
                            }
                        },
                        error: function (jqXHR, textStatus, errorThrown) {
                            fd.remove();
                            _myHelper.processServerResponse(jqXHR, null, null);
                        }
                    });
                    return true;
                },
                'NEIN': function () {
                    $(this).dialog("close");
                }
            }
        }).prev().find('.ui-dialog-titlebar-close').hide();
    }

    this.up = function () {
        rqkosId = $('.parentid').html();
        window.open(HostAdress()+ '/RQKos/' + rqkosId + '?verb=' + 'edit', '_self');
    }
}

function Data2EditForm(data) {
    saveData = data;
    reset();
}

function HintListHtml(hints, count) {
    var retStr = '<ul class="message-box">'

    for (var i = 0; i < count; i++) {
        retStr += '<li>' + hints[i].HintTitle + ': ' + hints[i].HintMessage + '</li>';
    }
    retStr += '</ul>';
    return retStr;
}

function EditForm2Data() {
    var s = parseInt($('#EditForm .nrofsubclasses').html());
    var ret = new Object();
    var scl = new Array(s);
    var cl = new Object;

    cl.ClassID = $('#EditForm .classid').html();
    cl.ClassCode = $('#EditForm .classcode').html();
    cl.ParentID = $('#EditForm .parentid').html();
    cl.ClassName = $('#EditForm .classname').html();
    cl.ClassTitle = $('#EditForm .classtitle').html();
    cl.RVKClassCodes = $('#EditForm .rvkclasscodes').html();
    cl.NrOfSubclasses = $('#EditForm .nrofsubclasses').html();
    cl.NrOfDocuments = $('#EditForm .nrofdocuments').html();
    scl[0] = cl;
    for (var i = 1; i <= s; i++) {
        cl = new Object();
        cl.ClassID = $('#ClassID' + i).html();
        cl.ClassCode = $('#EditSubForm' + i + ' .classcode').val();
        cl.ParentID = $('#EditSubForm' + i + ' .parentid').html();
        cl.ClassName = $('#EditSubForm' + i + ' .classname').val();
        cl.ClassTitle = $('#EditSubForm' + i + ' .classtitle').html();
        cl.RVKClassCodes = $('#EditSubForm' + i + ' .rvkclasscodes').val();
        cl.NrOfSubclasses = $('#EditSubForm' + i + ' .nrofsubclasses').html();
        cl.NrOfDocuments = $('#EditSubForm' + i + ' .nrofdocuments').html();
        scl[i] = cl;
    }
    return scl;
}

function down(index) {
    rqkosId = $('#ClassID' + index).html();
    window.open(HostAdress() + '/RQKos/' + rqkosId + '?verb=' + 'edit', '_self');
}

function GetClassTemplateHtml() {
    //var len = $('.extraPerson').length;
    var $html = $('.extra-class-template').clone();
    //$html.find('[name=firstname]')[0].name = "firstname" + len;
    //$html.find('[name=lastname]')[0].name = "lastname" + len;
    //$html.find('[name=gender]')[0].name = "gender" + len;
    return $html.html();
}

// Javascript utility functions
// Get query parameters from URL
function getUrlVar(key) {
    var result = new RegExp(key + '=([^&]*)', 'i').exec(window.location.search);
    return result && unescape(result[1]) || '';
}

function cancel() {
    window.location.replace(HostAdress() + '/RQItems?d=' + itemId);
}

function reset() {
    var s = parseInt(saveData[0].NrOfSubclasses);

    $('#EditForm .classid').html(saveData[0].ClassID);
    $('#EditForm .classcode').html(saveData[0].ClassCode);
    $('#EditForm .parentid').html(saveData[0].ParentID);
    $('#EditForm .classname').html(saveData[0].ClassName);
    $('#EditForm .classtitle').html(saveData[0].ClassTitle);
    $('#EditForm .rvkclasscodes').html(saveData[0].RVKClassCodes);
    $('#EditForm .nrofsubclasses').html(saveData[0].NrOfSubclasses);
    $('#EditForm .nrofdocuments').html(saveData[0].NrOfDocuments);
    for (var i = 1; i <= s; i++) {
        $('#ClassID' + i).html(saveData[i].ClassID);
        $('#EditSubForm' + i + ' .classcode').val(saveData[i].ClassCode);
        $('#EditSubForm' + i + ' .parentid').html(saveData[i].ParentID);
        $('#EditSubForm' + i + ' .classname').val(saveData[i].ClassName);
        $('#EditSubForm' + i + ' .classtitle').html(saveData[i].ClassTitle);
        $('#EditSubForm' + i + ' .rvkclasscodes').val(saveData[i].RVKClassCodes);
        $('#EditSubForm' + i + ' .nrofsubclasses').html(saveData[i].NrOfSubclasses);
        $('#EditSubForm' + i + ' .nrofdocuments').html(saveData[i].NrOfDocuments);
    }
}

//function displayTOC(strWithToc) {
//    var toc = strWithToc.substr(strWithToc.indexOf('$$TOC$$=') + '$$TOC$$='.length) + ' ';
//    var tocItemList = [];
//    var index = 1;

//    while (toc != '')
//    {
//        var p1 = toc.indexOf('MyMusic=');

//        if (p1 == -1) p1 = toc.indexOf('MyVideo=');
//        if (p1 == -1) p1 = toc.indexOf('MyDoc='); 
//        if (p1 != -1) {
//            var p2 = toc.indexOf('; ') != -1 ? toc.indexOf('; ') : -1;
//            var l = ('; ').length;

//            tocItemList.push({
//                position: index++,
//                name: toc.substring(0, p1),
//                adress: toc.substring(p1, p2)
//            });
//            toc = toc.substr(p2 + l);
//        }
//        else {
//            toc = '';
//        }
//    }
//    $('.toc').css('display', 'block');
//    $('#tocAttach').prop('value', 'detach object').attr('title', 'Not yet implemented.').attr('disabled', true);
//    //$('#tocList').html($('#tocTemplate').render(tocItemList));
//    $('#tocList').html(compiledTemplate.render(tocItemList));
//    return strWithToc.substring(0,strWithToc.indexOf('$$TOC$$='));
//}

//function hideTOC() {
//    $('.toc').css('display', 'none');
//    $('#tocAttach').prop('value', 'attach object').attr('title', 'Attach a digital object to this item.').attr('disabled', false);
//    $('#tocList').html();
//}

//function readTOC() {
//    if ($('.toc').css('display') == 'block') {
//        var tocStr = '';
//        var tocItemList = [];

//        $('#tocList li').each(function (index) {
//            tocItemList.push({
//                position: $(this).find('.tocitemposition')[0].value,
//                name: $(this).find('.tocitemname')[0].value,
//                adress: $(this).find('.tocitemadress')[0].value
//            });
//        });
//        tocItemList.sort(function (a, b) {
//            return a.position-b.position;
//        });
//        for (var i = 0; i < tocItemList.length; i++) {
//            var str = '';
            
//            str = tocItemList[i].name + tocItemList[i].adress;
//            if (str != '')
//                tocStr += str + '; '
//        }  
//        return (tocStr != '') ? '$$TOC$$=' + tocStr : tocStr;
//    }
//    else   
//        return '';
//}

//function attachTOC(e) {
//    $('#AttachTocDialog').dialog({ title: 'Attach Digital Object',
//        width: 600,
//        resizable: false,
//        modal: true,
//        buttons: {
//            OK: function () {
//                $(this).dialog('close');

//                var adr = $('#ObjAdress').attr('value');
//                var type = $('form input[name=DigitalObjectType]:checked').val()
//                var url = HostAdress() + '/DigitalObjects/Viewer/TableOfContent/' + adr + '?verb=' + type;

//                //debugger;
//                $.ajax({
//                    url: url,
//                    type: 'GET',
//                    dataType: 'json',
//                    success: function (data) {
//                        //debugger;
//                        displayTOC(data.toc);
//                    },
//                    error: function (response) {
//                        //debugger;
//                        _myHelper.processServerResponse(response, null, function () {
//                            $('#edit-dialog').html('<p><span class="ui-icon ui-icon-alert" style="float: left; margin: 0 7px 20px 0;"></span><span id="edit-dialog-message">Das digitale Objekt konnte nicht gefunden oder nicht ausgewertet werden.</span></p>');
//                            $(function () {
//                                $('#edit-dialog').dialog({ title: 'Schwerwiegender Fehler !',
//                                    width: 600,
//                                    resizable: false,
//                                    modal: true,
//                                    buttons: {
//                                        OK: function () {
//                                            $(this).dialog('close');
//                                        }
//                                    }
//                                });
//                            });
//                        });
//                    }
//                });
//            },
//            Cancel: function () {
//                $(this).dialog('close');
//            }
//        }
//    });
//}