$(function () {
    var onlyShelvesCheckBox = new AddOnlyShelvesCheckBox();
    onlyShelvesCheckBox.init("#classtree-options");
});

function AddOnlyShelvesCheckBox() {
    var checkBox;

    /* Initialize the only shelves check box and fill it with server data */
    this.init = function (selector) {
        // get check box
        checkBox = $(selector);
        // init the check box with server data  
        $.post(HostAdress() + '/{controllerName}/GetOnlyShelves',
              {},
              function (data) {
                  checkBox.prop('checked', data);
              }
        );
        checkBox.click(function () {
            $.post(HostAdress() + '/{controllerName}/ChangeOnlyShelves',
              { status: 'changed' },
              function (data) {
                  //checkBox.attr('checked', data);
              });
        });
    }
}