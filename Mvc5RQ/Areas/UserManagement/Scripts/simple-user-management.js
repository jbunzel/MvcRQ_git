
$(function () {

  var addUserArea = new AddUserArea();
  addUserArea.init(".simple-user-table");

  var roleMgmt = new RoleManagement();
  roleMgmt.init();

  var userTableArea = new UserTableArea();
  userTableArea.init(".simple-user-table");

});

function UserTableArea() {
    var userTable;

    /* Initializes the user table and fills it with server data. */
    this.init = function (tableSelector) {
        // get user table
        userTable = $(tableSelector);
        // fill the user table with all 
        $.ajax({
            type: 'POST',
            url: HostAdress() + "/{controllerName}/GetAllUsers",
            success: function (users, textStatus) {
                // Handle success
                for (var i in users) {
                    appendUser(users[i]);
                }
                //initialize table sorter and pager
                userTable.tablesorter({ widthFixed: true, widgets: ['zebra'] }).tablesorterPager({ container: $("#pager"), positionFixed: false, removeRows: false });
                // bind user action link event handlers for all rows
                userTable.on('click', '.delete-user', deleteUser);
                userTable.on("click", ".unlock-user", unlockUser);
                userTable.on("click", ".manage-roles", manageRoles);
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle error
                _myHelper.showMessage(xhr.responseText.replace(/\+/g, ' '), "error");
                }
        });
    }

    /* Shows a manage roles dialog with checkboxes for each role if the user clicks on the manage roles action link. */
    function manageRoles(e) {
        // hide any other manage roles dialog
        $(".manage-roles-dialog").remove();

        var $clickedLink = $(this);
        var user = $(this).closest("tr").data("user");

        // ask server which roles the user is currently in and which else exist
        $.ajax({
            type: 'POST',
            url: HostAdress() + "/{controllerName}/GetUserRoleStatus?Id=" + user.Id,
            success: function (response, textStatus) {
                //build role dialog
                var $dlg = $("<div/>").addClass("manage-roles-dialog").hide();
                var $roleList = $("<ul />").addClass("role-list");
                var roleInfo = response.data;

                for (var i in roleInfo) {
                    var rolename = roleInfo[i].rolename;
                    var cbxId = "cbx-" + rolename;
                    //create a list item, a checkbox, a label foreach role
                    var $li = $("<li />");
                    var $cbx = $("<input type='checkbox' class='cbx-role' />").val(rolename).attr("checked", roleInfo[i].isInRole).attr("id", cbxId);
                    var $label = $("<label />").text(rolename).attr("for", cbxId);

                    // append checkbox and label to list item
                    $li.append($cbx).append($label).appendTo($roleList);
                }
                //append role list to dialog
                $dlg.append($roleList);
                $clickedLink.after($dlg);
                $dlg.fadeIn();
                //bind add or remove role for the created checkboxes
                $dlg.find(".cbx-role").change(function (e) {
                    e.stopPropagation();
                    var data = {
                        rolename: $(this).val(),
                        isInRole: $(this).is(":checked"),
                        userId: user.Id
                    };
                    // start adding or removing the role for the given user
                    addRemoveRoleForUser(data);
                });
                //hide manage roles dialog on document click but not on a click inside the dialog
                $dlg.on("click", function (e) { e.stopPropagation() });
                $(document).one("click", function (e) { $dlg.fadeOut() });
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle error
                _myHelper.showMessage(xhr.responseText.replace(/\+/g, ' '), "error");
            }
        });
        //$.post(HostAdress() + "/{controllerName}/GetUserRoleStatus", { username: user.name }, function (response) {

        //  // on success do...
        //  _myHelper.processServerResponse(response, function () {

        //    //build role dialog
        //    var $dlg = $("<div/>").addClass("manage-roles-dialog").hide();
        //    var $roleList = $("<ul />").addClass("role-list");

        //    var roleInfo = response.data;
        //    for (var i in roleInfo) {

        //      var rolename = roleInfo[i].rolename;
        //      var cbxId = "cbx-" + rolename;

        //      //create a list item, a checkbox, a label foreach role

        //      var $li = $("<li />");
        //      var $cbx = $("<input type='checkbox' class='cbx-role' />").val(rolename).attr("checked", roleInfo[i].isInRole).attr("id", cbxId);
        //      var $label = $("<label />").text(rolename).attr("for", cbxId);

        //      // append checkbox and label to list item
        //      $li.append($cbx).append($label).appendTo($roleList);
        //    }

        //    //append role list to dialog
        //    $dlg.append($roleList);

        //    $clickedLink.after($dlg);
        //    $dlg.fadeIn();

        //    //bind add or remove role for the created checkboxes
        //    $dlg.find(".cbx-role").change(function (e) {

        //      e.stopPropagation();

        //      var data = {
        //        rolename: $(this).val(),
        //        isInRole: $(this).is(":checked"),
        //        username: user.name
        //      };

        //      // start adding or removing the role for the given user
        //      addRemoveRoleForUser(data);

        //    });

        //    //hide manage roles dialog on document click but not on a click inside the dialog
        //    $dlg.on("click", function (e) { e.stopPropagation() });
        //    $(document).one("click", function (e) { $dlg.fadeOut() });


        //  });

        //});
        return false;
    }

    /* Adds or removes a role for a user */
    function addRemoveRoleForUser(data) {
        $.ajax({
            type: 'POST',
            url: HostAdress() + "/{controllerName}/AddRemoveRoleForUser",
            data: data,
            success: function (response, textStatus) {
                _myHelper.processServerResponse(response);
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle error
                _myHelper.showMessage(xhr.responseText.replace(/\+/g, ' '), "error");
            }
        });
    }

    /* Unlocks the user account when the unlock user link is clicked*/
    function unlockUser(e) {

        e.preventDefault();

        var user = $(this).closest("tr").data("user");
        var lockoutLink = $(this);


        // ask server for switching lockout state
        $.post(HostAdress() + "/{controllerName}/UnlockUser", { userName: user.name }, function (response) {

          _myHelper.processServerResponse(response, function () {

            //on success replace link with success info
            lockoutLink.replaceWith("<span>Just Unlocked</span>");

          });
        });

      }

    /* Adds a new user to the user table body */
    function appendUser(user) {
        // build row and append to table
        var row = _myHtml.buildUserTableRow(user);
        userTable.children('tbody').append(row);
      }

    /* Deletes a user through the membership service and the according row in the user table. */
    function deleteUser(e) {
        e.preventDefault();

        var row = $(this).closest("tr");
        var user = row.data("user");
        // cancel user deletion if confirmation is negative
        var result = confirm("Do you really want to delete the user account for " + user.Email + " ?");

        if (!result)
          return;
        //delete user by id and check result
        $.ajax({
            type: 'POST',
            url: HostAdress() + "/{controllerName}/Delete",
            data: {username: user.Email},
            success: function (response, textStatus) {
                _myHelper.processServerResponse(response, function () {

                    //disable table pager
                    userTable.trigger('disable.pager');

                    //remove the specified row from the table
                    row.remove();

                    //enable pager again
                    userTable.trigger('enable.pager');
                })
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle error
                _myHelper.showMessage(xhr.responseText.replace(/\+/g, ' '), "error");
            }
        });
        //$.post(HostAdress() + "/{controllerName}/DeleteUser", { username: user.name }, function (response) {

        //  _myHelper.processServerResponse(response, function () {

        //    //disable table pager
        //    userTable.trigger('disable.pager');

        //    //remove the specified row from the table
        //    row.remove();

        //    //enable pager again
        //    userTable.trigger('enable.pager');
        //  });

        //});
    }
}

function AddUserArea() {
    var userTable;

    /* Initializes all controlls of the add-user-form */
    this.init = function (userTableSelector) {

    userTable = $(userTableSelector);

    //stop initialization if add user form is unused
    if (!$("#add-user-form"))
      return;

    // binds the add user buttons click event to the create user function
    $("#btn-create-user").click(createUser);


  }

    /* Creates a new user if all field data is valid */
    function createUser(e) {
        var username = $("#tbx-add-username").val();
        var pwd = $("#tbx-add-password").val();
        var pwd2 = $("#tbx-add-repeat-password").val();
        var email = $("#tbx-add-email").val();
        var roles = [];

        $("#add-user-roles :selected").each(function (i, selected) {
            roles[i] = $(selected).text();
        });
        //verify that both passwords are equal
        if (pwd !== pwd2 || pwd === "" || username === "" || email === "") {
            _myHelper.showError("You have missed something or the passwords do not match.");
            return false;
        }

        var postData = { username: username,
          password: pwd,
          email: email,
          roles: roles
        };

        //send user creation request to server
        $.ajax({
            type: "POST",
            url: HostAdress() + "/{controllerName}/Create",
            data: postData,
            dataType: "json",
            traditional: true,
            success: function (response) {
                _myHelper.processServerResponse(response, function () {
                    // build a user row and add it to the table by maintaining paging and sorting state
                    var user = response.data;
                    var $row = _myHtml.buildUserTableRow(user);
                    userTable.find('tbody').append($row); //.trigger('addRows', [$row]);
                })
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle error
                _myHelper.showMessage(xhr.responseText.replace(/\+/g, ' '), "error");
            }
        });
        return false;
  }
}

function RoleManagement() {
    var roleSelectBox = $(".role-select-box");

    this.init = function () {
        fillRoleSelectBox();
        bindAddRoleButtonClick();
        bindDeleteRoleButtonClick();
    }

    function fillRoleSelectBox() {
        $.ajax({
            type: 'POST',
            url: HostAdress() + "/RolesAdmin/GetAllRoles",
            success: function (roles, textStatus) {
                // Handle success
                for (var i in roles) {
                    addRoleToRoleSelectBox(roles[i].Text);
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle error
                _myHelper.showMessage(xhr.responseText.replace(/\+/g, ' '), "error");
            }
        });
    }

    function addRoleToRoleSelectBox(role) {
       roleSelectBox.append($("<option value=" + role + ">" + role + "</option>"));
    }

    /* Starts the role creation process when the add role button is clicked */
    function bindAddRoleButtonClick() {
        $("#btn-add-role").click(function (e) {
            var rolename = $("#role-name").val();

            //check that the role input is not empty
            if (rolename.length === 0) {
                _myHelper.showError("You have not entered a role name");
            }
            else {
            addRole(rolename);
            }
            return false;
        });
    }

    /* Sends a role creation request to the server */
    function addRole(roleName) {
        $.ajax({
            type: 'POST',
            url: HostAdress() + "/RolesAdmin/Create",
            data: {
                Id: "",
                Name: roleName
            },
            success: function (response, textStatus) {
                addRoleToRoleSelectBox(roleName);
            },
            error: function (xhr, textStatus, errorThrown) {
                _myHelper.showMessage(xhr.responseText.replace(/\+/g, ' '), "error");
            }
        });
    }

    /* Handles the delete role button click event */
    function bindDeleteRoleButtonClick() {
        $("#btn-delete-role").click(function (e) {
            var selectedOption = roleSelectBox.children(":selected");

            if (selectedOption.length === 1) {
                var rolename = selectedOption.text();
                var allowPopulatedRoleDeletion = $("#allow-populated-role").is(":checked");
                var confirmationResult = confirm("Do you really want to delete the role named " + rolename + " ?");

                if (!confirmationResult) return false;
                deleteRole(rolename, allowPopulatedRoleDeletion);
            } else {
                _myHelper.showError("You have not selected a role to delete.");
            }
            return false;
        });
    }

    function deleteRole(roleName, allowPopulatedRoleDeletion) {
        $.ajax({
            type: 'POST',
            url: HostAdress() + "/RolesAdmin/Delete",
            data: {
                Id: "",
                Name: roleName
            },
            success: function (response, textStatus) {
                roleSelectBox.children("[value='" + roleName + "']").remove();
            },
            error: function (xhr, textStatus, errorThrown) {
                _myHelper.showMessage(xhr.responseText.replace(/\+/g, ' '), "error");
            }
        });
    }
}

var _myHtml = {
  /* generates a new user table row as jquery element */
    buildUserTableRow: function (user) {
        var userRow = new UserRowBuilder(user);

        userRow.addCell(user.Id, "user-id");
        userRow.addCell(user.Email,"user-name");
        userRow.addCell(FormatDateTime(new Date(parseInt(user.RegisterDate.substr(6)))), "register-date");
        userRow.addCell(user.EmailConfirmed, "email-confirmed");
        userRow.addCell(FormatDateTime(new Date(parseInt(user.LastActivityDate.substr(6)))), "last-act-date");
        //build lockout text or link
        var $lockoutElem;

        if (user.LockoutEndDateUtc == null) {
          $lockoutElem = $("<span/>").text("Is Unlocked");
        }
        else {
          $lockoutElem = $("<a />").addClass("unlock-user").text("Unlock Account");
        }
        userRow.addCellForElem($lockoutElem, "lock-state");

        //build manage roles link
        $manageRolesLink = $("<a />").addClass("manage-roles").text("User Roles");
        userRow.addCellForElem($manageRolesLink,"edit-roles");

        //build deletion link and append
        var $deletionLink = $("<a />").data("user-name", user.name).addClass("delete-user").text("Delete");
        userRow.addCellForElem($deletionLink, "action");

        return userRow.getElem();
    }
}

/* User Row Object for generating new rows containing user information */
function UserRowBuilder(user) {
    var $row = $("<tr />").data("user", user);

    /*Adds a new cell with the specified text */
    this.addCell = function (text, cssClass) {

    var $cell = $("<td />");
    $cell.text(text);

    //add cell css class
    $cell.addClass(cssClass);

    $row.append($cell);
  }

    /* Adds a new cell for the specified element */
    this.addCellForElem = function (elem, cssClass) {

    var $cell = $("<td />");
    $cell.append(elem);

    //add cell css class
    $cell.addClass(cssClass);

    $row.append($cell);
  }

    /* returns the row element itself */
    this.getElem = function () {
    return $row;
  }
}

function FormatDateTime(datetime)
{
    var month = (datetime.getMonth() + 1) > 9 ? datetime.getMonth() + 1 : "0" + datetime.getMonth() + 1;
    var date = datetime.getDate() > 9 ? datetime.getDate() : "0" + datetime.getDate();
    var minutes = datetime.getMinutes() > 9 ? datetime.getMinutes() : "0" + datetime.getMinutes();
    var seconds = datetime.getSeconds() > 9 ? datetime.getSeconds() : "0" + datetime.getSeconds();
    
    return (date + "." + month + "." + datetime.getFullYear() + " / " + datetime.getHours() + "h:" + minutes + "m:" + seconds + "s");
}