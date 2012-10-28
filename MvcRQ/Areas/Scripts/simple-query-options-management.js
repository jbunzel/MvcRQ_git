$(function () {
    var includeExternalCheckBox = new AddIncludeExternalCheckBox();
    includeExternalCheckBox.init("#include-external");
    
    $(".select-databases").on("click", selectDatabases);
});

function AddIncludeExternalCheckBox() {
    var checkBox;

    /* Initialize the include external check box and fill it with server data */
    this.init = function (selector) {
        // get check box
        checkBox = $(selector);
        // init the check box with server data  
        $.post(HostAdress() + '/{controllerName}/GetIncludeExternal',
              {},
              function (data) {
                  checkBox.prop('checked', data);
              }
        );
        checkBox.click(function () {
            $.post(HostAdress() + '/{controllerName}/ChangeIncludeExternal',
              { status: 'changed' },
              function (data) {
                  //checkBox.attr('checked', data);
              });
        });
    }
}

/* Shows a select database dialog with checkboxes for each database if the user clicks on the select databases action link. */
function selectDatabases(e) {
    // hide any other manage roles dialog
    $(".select-databases-dialog").remove();
    var $clickedLink = $(this);

    // ask server which roles the user is currently in and which else exist
    $.post(HostAdress() + "/{controllerName}/GetExternalDatabaseStatus", {}, function (response) {
        // on success do...
        _myHelper.processServerResponse(response, function () {
            //build select databases dialog
            var $dlg = $("<div/>").addClass("select-databases-dialog").hide();
            var $databaseList = $("<ul />").addClass("database-list");
            var databaseInfo = response.data;

            for (var i in databaseInfo) {
                var databasename = databaseInfo[i].databasename;
                var cbxId = "cbx-" + databasename;
                //create a list item, a checkbox, a label foreach role
                var $li = $("<li />");
                var $cbx = $("<input type='checkbox' class='cbx-role' />").val(databasename).attr("checked", databaseInfo[i].included).attr("id", cbxId);
                var $label = $("<label />").text(databasename).attr("for", cbxId);
                // append checkbox and label to list item
                $li.append($cbx).append($label).appendTo($databaseList);
            }

            //append database list to dialog
            $dlg.append($databaseList);
            $clickedLink.after($dlg);
            $dlg.fadeIn();

            //bind add or remove role for the created checkboxes
            $dlg.find(".cbx-role").change(function (e) {
                e.stopPropagation();
                
                var data = {
                    databasename: $(this).val(),
                    included: $(this).is(":checked"),
                };

                // start adding or removing the role for the given user
                addRemoveDatabaseForUser(data);

            });

            //hide manage roles dialog on document click but not on a click inside the dialog
            $dlg.on("click", function (e) { e.stopPropagation() });
            $(document).one("click", function (e) { $dlg.fadeOut() });
        });
    });
    return false;
}

/* Adds or removes a database for a user */
function addRemoveDatabaseForUser(data) {
    $.post(HostAdress() + "/{controllerName}/AddRemoveDatabaseForUser", data, function (response) {
        _myHelper.processServerResponse(response);
    })
}

var _myHelper = {

    /* helper function for processing the server response. Triggers either an error or success message window 
    /* and calls provided functions if neccessary. */
    processServerResponse: function (response, onSuccess, onError) {

        if (response.isSuccess) {
            if (response.message) // show message only if available
                _myHelper.showSuccess(response.message);

            // call success callback
            if ($.isFunction(onSuccess))
                onSuccess.apply();

        } else {

            //show error message
            if (response.message) // show message only if available
                _myHelper.showError(response.message, "error");

            if ($.isFunction(onError))
                onError.apply();
        }
    },

    showSuccess: function (msg) { _myHelper.showMessage(msg, "success"); },

    showError: function (msg) { _myHelper.showMessage(msg, "error"); },

    /* returns the info window element and creates one if it is not yet existing */
    infoWindow: function () {

        //remove old info window
        $("#simple-user-info").remove();

        //add new one
        return $("<div/>").attr("id", "simple-user-info")
               .appendTo("body");
    },

    /* Shows an error or success notification */
    showMessage: function (message, cssClass) {


        var $info = _myHelper.infoWindow().addClass(cssClass).text(message);

        //show info message
        $info.fadeIn(1000).on("mouseover", fadeOutInfoWindow);

        //hide after 4 seconds and on one document click
        setTimeout(function () { fadeOutInfoWindow(); }, 4000);
        $(document).one("click", fadeOutInfoWindow)

        //fadeOut info window
        function fadeOutInfoWindow() {
            $info.fadeOut(1000);
        }
    },

    /* Helper function for html encoding unsecure user input */
    encodeHtml: function (input) {

        return $("<div/>").text(input).html();

    }
}
