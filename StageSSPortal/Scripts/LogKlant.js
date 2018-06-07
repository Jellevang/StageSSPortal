$(document).ready(function () {
    $("table#LijstServerVMs").delegate("tr:not(:first)", "click", function () {
        $(".VmRow").css("color", "rgb(0, 0, 0)");
        $(this).css("color", "rgb(244, 78, 66)");
        ShowLogsVM();
    });
    $("table#LijstKlanten").delegate("tr:not(:first)", "click", function () {
        $(".VmRow").css("color", "rgb(0, 0, 0)");
        $(this).css("color", "rgb(244, 78, 66)");
        ShowLogsKlant();
    });

    $(".Vm").change(function () {
        if ($("input.Vm").prop('checked', true)) {
            $("input.Klant").prop('checked', false);
            //showToegekend();
            $("#ShowLogs").css("display", "none");
            $("#ShowKlantLogs").css("display", "none");
            //$("table#LijstServerVms").find("tr:not(:first)").remove();
            showVmNames(true);
        }
    })
    $(".Klant").change(function () {
        if ($("input.Klant").prop('checked', true)) {
            $("input.Vm").prop('checked', false);
            //showNietToegekend();
            $("#ShowLogs").css("display", "none");
            $("#ShowKlantLogs").css("display", "none");
            $(".VmRow").css("color", "inherit");
            $("table#LijstServerVMs").find("tr:not(:first)").remove();
            loadKlanten();
        }
    })
    //loadKlanten();
});


//=============KLANTEN===================
function ShowLogsKlant() {
    $("table#LijstKlantLogs").find("tr:not(:first)").remove();

    var id;
    $("tr.VmRow").each(function () {
        if ($(this).css('color') == "rgb(244, 78, 66)") {
            id = $(this).attr('id');
        }
    })
     //alert(id);
    $.ajax("/api/Klant/SSH/LogLijstUser/" + id, {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            if (data == false) {
                $("table#LijstKlantLogs").append("<tr  class=LogRowEmpty><td colspan=3>" + "Nog geen audits" + "</td></tr>");
            }
            else {
                $.each(data, function (index, value) {
                    $("table#LijstKlantLogs").append("<tr  class=LogRow><td>" + value.Ovm + "</td> <td> " + value.Naam + "</td><td>" + value.ActionDate + "</td>" + "</tr>");
                })
            }
            $("#ShowKlantLogs").show();

        })
}
function loadKlanten() {
    $("table#LijstKlanten").find("tr:not(:first)").remove();
    $("table#LijstServerVms").find("tr:not(:first)").remove();
    $("#ShowVms").hide();
    $.ajax("/api/SSH/GetKlantenUser", {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            ShowKlanten(data);
        });
}

function ShowKlanten(LijstKlanten) {
    $.each(LijstKlanten, function (index, value) {
        //alert(value)
        addKlant(value);
    })
    $("#ShowKlanten").css("display", "block");

}
function addKlant(klant) {
    //alert('add');
    $("table#LijstKlanten").append("<tr  class=VmRow id=" + klant.KlantId + ">" + "<td>" + klant.Email + "</td></tr>");
    if (klant.IsKlantAccount == false) {
        // changeColor(vm.id);
        $("#" + klant.KlantId).css("font-weight", "bold");

        // $("#" + klant.KlantId).hide();
    }
    //} else if (bool == false) {
    //    $("#" + vm.id).show();
    //}

    //} else {
    //    if (bool == true) {
    //        $("#" + vm.id).show();
    //    } else if (bool == false) {
    //        $("#" + vm.id).hide();
    //    }
    //}
}
//=============VMs=========================
function ShowLogsVM() {
    $("table#LijstLogs").find("tr:not(:first)").remove();

    var id;
    $("tr.VmRow").each(function () {
        if ($(this).css('color') == "rgb(244, 78, 66)") {
            id = $(this).attr('id');
        }
    })
    // alert(id);
    $.ajax("/api/Klant/SSH/LogLijstOvm/" + id, {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            if (data == false) {
                $("table#LijstLogs").append("<tr  class=LogRowEmpty><td colspan=3>" + "Nog geen Audits" + "</td></tr>");
            }
            else {
                $.each(data, function (index, value) {
                    $("table#LijstLogs").append("<tr  class=LogRow><td>" + value.Gebruiker + "</td> <td> " + value.Naam + "</td><td>" + value.ActionDate + "</td>" + "</tr>");
                })
            }
            $("#ShowLogs").show();
        })
}
function showVmNames() {
    $("#ShowKlanten").hide();
    $(".VmRow").css("background-color", "inherit");
    if ($("#ShowVms").css("display") != "none") {
        $("table#LijstServerVMs").find("tr:not(:first)").remove();
    }
    $.ajax("/api/Klant/SSH/KlantOVMs", {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            ShowVms(data);
        });
}

function ShowVms(LijstServerVMs) {
    $.each(LijstServerVMs, function (index, value) {
        addVM(value);
    })
    $("#ShowVms").css("display", "block");

}
function addVM(vm) {
    $("table#LijstServerVMs").append("<tr  class=VmRow id=" + vm.id + ">" + "<td>" + vm.Name + "</td></tr>");
}
