// Javascript code to view RQKOS shelves with DynaTree
function getRQItemList(node) {
    $.ajax({
        beforeSend: function (req) {
            req.setRequestHeader("Accept", "application/x-riquest-internal");
        },
        url: HostAdress() + encodeURI("/rqds/rqitems/rqi?verb=BrowseViewState&queryString=$class$" + node.data.key.substring(node.data.key.indexOf('$') + 1)),
        type: "GET",
        data: null,
        dataType: "html",
        success: function (data) {
            renderHtmlList(data);
            if (docNo != "") {
                setTimeout(function () {
                    getOldRQItem(docNo);
                }, 1)
            }
        },
        error: function (xhr) {
            alert(xhr.responseText);
        }
    });
    $("#echoActive").text("" + node + " (" + node.getKeyPath() + ")");
}

function getOldRQItem(docno) {
    var eid = $('div[DocNo =' + docno + ']').attr("id");

    getRQItem(docno, eid, "o" + eid);
}

//function getRQKos(path, docno) {
//    debugger;
//    locPath = path;
//    docNo = docno;
//    ExpandPath(path);
//}

$(function () {
    // Attach the dynatree widget to an existing <div id="tree"> element 
    // and pass the tree options as an argument to the dynatree() function:
    // var urlbase = document.location.href.substring(0, document.location.href.toLowerCase().toLowerCase().indexOf("rqkos") + 5);
    var urlbase = document.location.href.substring(0, document.location.href.toLowerCase().indexOf("rqkos")) + "rqds/rqkos";

    $("#tree").dynatree({
        initAjax: {
            url: urlbase + '/0?verb=dt',
            type: "GET",
            data: {
                mode: "all"
            },
            dataType: "json",
        },
        autoFocus: false,
        onLazyRead: function (node) {
            node.appendAjax({
                url: urlbase + "/" + node.data.key.substring(0, node.data.key.indexOf('$')) + "?verb=dt",
                data: {
                    "key": node.data.key.substring(0, node.data.key.indexOf('$')), // Optional url arguments 
                    mode: "all"
                },
                success: function (node) {
                    SortChildren(node);
                }
            });
        },
        onActivate: function (node) {
            // A DynaTreeNode object is passed to the activation handler 
            // Note: we also get this event, if persistence is on, and the page is reloaded. 
            getRQItemList(node);
        },
        onPostInit: function (isReloading, isError) {
               this.reactivate();
               ExpandPath(locPath);
        },
        persist: true
    });
});

function SortChildren(node) {
    node.sortChildren(function (a, b) {
        a = a.data.key.substr(a.data.key.indexOf('$') + 1).toLowerCase();
        b = b.data.key.substr(b.data.key.indexOf('$') + 1).toLowerCase();
        return a > b ? 1 : a < b ? -1 : 0;
    }, false);
}

function EditActiveNode() {
    var node = $("#tree").dynatree("getActiveNode");

    if (node) {
        var path = 'rqkos/edit/rqc_' + node.data.key.substr(node.data.key.indexOf('$') + 1).toLowerCase(); // + '?verb=edit';

        window.open(HostAdress() + "/" + path, "_self");
    }
    else
        alert("Sie haben im Klassifikationsbaum noch keine Klasse ausgewählt!");
}

function ExpandPath(keyPath) {
    var tree = $("#tree").dynatree("getTree");
    
    tree.loadKeyPath(keyPath, function (node, status) {
        if (status == "loaded") {
            // 'node' is a parent that was just traversed. 
            // If we call expand() here, then all nodes will be expanded 
            // as we go 
            node.expand();
        } else if (status == "ok") {
            // 'node' is the end node of our path. 
            // If we call activate() or makeVisible() here, then the 
            // whole branch will be expanded now 
            if ($("#tree").dynatree("getActiveNode") == node)
                getRQItemList(node);
            else
                node.activate();
        }
    });
}
