$(document).ready(function () {
    $("table#LijstServerVMs").delegate("tr:not(:first)", "click", function () {
        $(".VmRow").css("color", "rgb(0, 0, 0)");
        $(".AdminRow").css("color", "rgb(0, 0, 0)");
        $(this).css("color", "rgb(244, 78, 66)");
        ShowLogsVM();
    });
    $("table#LijstKlanten").delegate("tr:not(:first)", "click", function () {
        $(".VmRow").css("color", "rgb(0, 0, 0)");
        $(".AdminRow").css("color", "rgb(0, 0, 0)");
        $(this).css("color", "rgb(244, 78, 66)");
        ShowLogsKlant();
    });
    $(".AdminRow").delegate("tr", "click", function () {
        $(".AdminRow").css("color", "rgb(0, 0, 0)");
        $(this).css("color", "rgb(244, 78, 66)");
       ShowLogsAdmin();
    });
    //$("select").hide();
    getServersDB();
    $("select").hide();
    $(".Vm").change(function () {
        if ($("input.Vm").prop('checked', true)) {
            $("input.Klant").prop('checked', false);
            //showToegekend();
            $("#ShowLogs").css("display", "none");
            $("#ShowKlantLogs").css("display", "none");
            $("#servers").css("display", "block");
            showVmNames($("#servers").val(), true);
        }
    })
    $(".Klant").change(function () {
        if ($("input.Klant").prop('checked', true)) {
            $("input.Vm").prop('checked', false);
            //showNietToegekend();
            $("#ShowLogs").css("display", "none");
            $("#ShowKlantLogs").css("display", "none");
            $(".VmRow").css("color", "inherit");
            loadKlanten();
        }
    })
    //loadKlanten();
});
//===========SERVERS====================
function getServersDB() {
    $.ajax("/api/ssh/getServersDB", {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            $.each(data, function (index, value) {
                $("#servers").append("<option class='" + value.ServersId + "'value='" + value.Id + "' > " + value.ServerNaam + "</option > ");
            })
            //                $("#servers").css("display", "block");
            $("#servers").change(function () {
                showVmNames($("#servers").val());
            })
        })
}

//=============KLANTEN===================
function ShowLogsAdmin() {
    //alert("heheh")
    $(".VmRow").css("color", "rgb(0, 0, 0)");
    $.ajax("/api/Klant/SSH/LogLijstUser", {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            $.each(data, function (index, value) {
                $("table#LijstKlantLogs").append("<tr  class=LogRow><td>" + value.Gebruiker + "</td><td>" + value.Ovm + "</td> <td> " + value.Naam + "</td><td>" + value.ActionDate + "</td>" + "</tr>");
            })
            $("#ShowKlantLogs").show();
        })
}

function ShowLogsKlant() {
    $("table#LijstKlantLogs").find("tr:not(:first)").remove();

    var id;
    $("tr.VmRow").each(function () {
        if ($(this).css('color') == "rgb(244, 78, 66)") {
            id = $(this).attr('id');
        }
    })
    // alert(id);
    $.ajax("/api/Klant/SSH/LogLijstKlant/" + id, {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            if (data == false) {
                $("table#LijstKlantLogs").append("<tr  class=LogRowEmpty><td colspan=4>" + "Nog geen Audits" + "</td></tr>");
            }
            else {
                $.each(data, function (index, value) {
                    $("table#LijstKlantLogs").append("<tr  class=LogRow><td>" + value.Gebruiker + "</td><td>" + value.Ovm + "</td> <td> " + value.Naam + "</td><td>" + value.ActionDate + "</td>" + "</tr>");
                })
            }
            $("#ShowKlantLogs").show();
        })
}
function loadKlanten() {
    $("table#LijstKlanten").find("tr:not(:first)").remove();
    //$("#VmInfo").css("display", "none");

    $("select").hide();
    //alert('test')
    $("#ShowVms").hide();
    $.ajax("/api/SSH/getAdmin", {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            $("table#LijstKlanten").append("<tr  class=AdminRow id=" + data.AdminId + " onClick=ShowLogsAdmin()>" + "<td>" + data.Email + "</td></tr>");
            $.ajax("/api/SSH/getKlanten", {
                type: "GET",
                dataType: "json"
            })
                .done(function (data) {
                    ShowKlanten(data)

                })
            // alert(data);
        });

}
function ShowKlanten(LijstKlanten) {
    // $("table#LijstKlanten").append("<tr  class=VmRow id=" + klant.KlantId + ">" + "<td>" + klant.Email + "</td></tr>");
    $.each(LijstKlanten, function (index, value) {
        //alert(value)
        addKlant(value);
    })
    $("#ShowKlanten").css("display", "block");

}
function addKlant(klant) {
    $("table#LijstKlanten").append("<tr  class=VmRow id=" + klant.KlantId + ">" + "<td>" + klant.Email + "</td></tr>");
    if (klant.IsKlantAccount == false) {
        $("#" + klant.KlantId).css("font-weight", "bold");
    }

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
function showVmNames(id, bool) {
    //alert(id+bool);
    $("#ShowKlanten").hide();

    $("select").show();

    $("div#btns").hide();
    $("#btnSave").hide();
    $(".VmRow").css("background-color", "inherit");
    // alert("vmnames");
    if ($("#ShowVms").css("display") != "none") {
        $("table#LijstServerVMs").find("tr:not(:first)").remove();
        $("#VmInfo").css("display", "none");
    }
    if (id == 0) {
        $.ajax("/api/SSH/GetAllVmsDB", {
            type: "GET",
            dataType: "json"
        })
            .done(function (data) {
                ShowVms(data, bool);
                //alert(bool);
            })
    } else {
        $.ajax("/api/SSH/vmsDB/" + id + "/", {
            type: "GET",
            dataType: "json"
        })
            .done(function (data) {
                ShowVms(data, bool);
            })
    };
}
function ShowVms(LijstServerVMs, bool) {
    $.each(LijstServerVMs, function (index, value) {
        addVM(value, bool);
    })
    $("#ShowVms").css("display", "block");

}
function addVM(vm, bool) {
    if (vm.KlantId != 0) {
        $("table#LijstServerVMs").append("<tr  class=VmRow id=" + vm.id + ">" + "<td>" + vm.Name + "</td>" /*+ "<td>" + vm.Status + "</td>"*/ + "</tr>");
    }
}
