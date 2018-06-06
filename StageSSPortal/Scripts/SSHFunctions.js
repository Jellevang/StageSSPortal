$(document).ready(function () {
    // alert("test");
    //$("select").hide();
    $("#btnSave").hide();
    $(".checkToeg").change(function () {
        if ($("input.checkToeg").prop('checked', true)) {
            $("input.checkNietToeg").prop('checked', false);
            //showToegekend();
            showVmNames($("#servers").val(), true);
        }
    })
    $(".checkNietToeg").change(function () {
        if ($("input.checkNietToeg").prop('checked', true)) {
            $("input.checkToeg").prop('checked', false);
            //showNietToegekend();
            showVmNames($("#servers").val(), false);
        }
    })
    $("table#LijstServerVMs").delegate("tr:not(:first)", "click", function () {
        $(".VmRow").css("color", "rgb(0, 0, 0)");
        $(this).css("color", "rgb(244, 78, 66)");
        showInfo();
    });
    //alert("heheh");
    getServersDB();
});

function checkForVms() {
    $("select").hide();
    $("#btnSave").hide();
    $.ajax("/api/SSH/VmsDb", {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            ShowVms(data);
        })
}
function getServersDB() {
    $.ajax("/api/ssh/getServersDB", {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            $.each(data, function (index, value) {
                $("#servers").append("<option class='" + value.ServersId
                    + "'value='" + value.Id + "' > "
                    + value.ServerNaam + "</option > ");
            })
            $("#servers").css("display", "block");
            $("#servers").change(function () {
                if ($("div#CheckBox").css("display", "block")) {
                    if ($("input.checkNietToeg").prop('checked') == true) {
                        showVmNames($("#servers").val(), false);
                    }
                    if ($("input.checkToeg").prop('checked') == true) {
                        showVmNames($("#servers").val(), true);
                    }

                } else {
                    showVmNames($("#servers").val());
                    $("div#CheckBox").css("display", "block");
                }
            })
        })
}
function loadKlanten(klant) {
    //alert(klant)
    $("select").show();
    $("#btnSave").show();
    $.ajax("/api/SSH/Klanten", {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            $("#klantenlijst").find("option").remove();
            $("#btns").css("display", "block");
            $.each(data, function (index, value) {
                $("#klantenlijst").append("<option class=klant> " + value.Naam + "</option>");
            })
            $("#klantenlijst").each(function () {
                // alert(klant);
                $('option:contains(' + klant + ')').prop('selected', true);
            })

        });
}
function showVmNames(id, bool) {
    $(".VmRow").css("background-color", "inherit");
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
function changeColor(id) {
    $("#" + id).css('background-color', 'rgb(190, 192, 196)');
}
function addVM(vm, bool) {
    $("table#LijstServerVMs").append("<tr  class=VmRow id=" + vm.id + ">" + "<td>" + vm.Name + "</td>" /*+ "<td>" + vm.Status + "</td>"*/ + "</tr>");
    if (vm.KlantId == 0) {
        changeColor(vm.id);
        if (bool == true) {
            $("#" + vm.id).hide();
        } else if (bool == false) {
            $("#" + vm.id).show();
        }

    } else {
        if (bool == true) {
            $("#" + vm.id).show();
        } else if (bool == false) {
            $("#" + vm.id).hide();
        }
    }
}
function showInfo(id) {
    var klant;
    $('#Dialog-Box').hide();
    $("select#LijstServerVms").hide();
    $("#btns").css("display", "none");
    $("#ListVmInfo").find("tr").remove();
    $("#VmInfo").css("display", "none");
    var t;
    if (id == null || id == "") {

        $("tr.VmRow").each(function () {
            if ($(this).css('color') == "rgb(244, 78, 66)") {
                t = $(this).attr('id');
            }
        })
    } else {
        t = id;
    }
    $.ajax("/api/SSH/info/" + t, {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {

            var text = data;
            $.each(data, function (index, value) {
                //alert(value)
                if (index == 0) {
                    $("table#ListVmInfo").append("<tr class=VmInfoRow> <td> id: " + value + "</td> </tr>");

                } else {
                    if (index == 7) {
                        $("table#ListVmInfo").append("<tr class=VmInfoRow> <td> Klant = " + value + "</td> </tr>");
                        klant = value;
                        // alert(klant)
                    } else {
                        $("table#ListVmInfo").append("<tr class=VmInfoRow> <td>" + value + "</td> </tr>");

                    }

                }
            })
            $(".VmInfoRow").each(function () {
                if ($(this).is(':contains("Running")')) {
                    $(this).css("color", "green");
                } else {
                    if ($(this).is(':contains("Stopped")')) {
                        $(this).css("color", "red");
                    }
                    else {
                        if ($(this).is(':contains("Status")')) {
                            $(this).css("color", "orange");
                        }
                    }
                }
            })
            $('#Dialog-Box').show();
            loadKlanten(klant);
            $("#VmInfo").css("display", "block");
            $("table#ListVmInfo").find("tr:first").css("font-weight", "bold");
            checkDowntime(t, false)
            $("tr#" + t).css("color", "rgb(244, 78, 66)")
        })
}
function StopVM() {
    var t;
    var naam;
    var duur;


    $("tr.VmRow").each(function () {
        if ($(this).css('color') == "rgb(244, 78, 66)") {
            t = $(this).attr('id');
            //alert(t);
        }
        $.ajax("/api/SSH/StopVM/" + t, {
            type: "GET",
            dataType: "json"
        })
            .done
            (function (data) {
            })
    });
    duur = prompt("Hoelang wilt u de server in downtime zetten? (in minuten)");
    naam = "Stop";
    DownTime(t, duur);
    CreateLog(t, naam);
    if ($(".checkToeg").is(":checked") && $(".checkNietToeg").is(":checked") == false) {
        showVmNames($("#servers").val(), true);
    }
    if ($(".checkToeg").is(":checked") == false && $(".checkNietToeg").is(":checked")) {
        showVmNames($("#servers").val(), false);
    }
    showInfo(t);


}
function RestartVM() {
    alert("Dit kan 30 seconden duren");
    var t;
    var naam;
    $("tr.VmRow").each(function () {
        if ($(this).css('color') == "rgb(244, 78, 66)") {
            t = $(this).attr('id');
        }
        $.ajax("/api/SSH/RestartVM/" + t + "/", {
            type: "GET",
            dataType: "json"
        })
            .done
            (function (data) {
            })
    });
    naam = "Restart";
    CreateLog(t, naam);
    if ($(".checkToeg").is(":checked") && $(".checkNietToeg").is(":checked") == false) {
        showVmNames($("#servers").val(), true);
    }
    if ($(".checkToeg").is(":checked") == false && $(".checkNietToeg").is(":checked")) {
        showVmNames($("#servers").val(), false);
    } for (var i = 0; i < 100; i++) {
        if (i == 50) {
            showInfo(t);
        }
    }

}
function StartVM() {
    var t;
    var naam;
    $("tr.VmRow").each(function () {
        if ($(this).css('color') == "rgb(244, 78, 66)") {
            //t = $(this).children("td:first").attr("id");
            t = $(this).attr('id');
            //t.replace(".", "-");
            // alert(t);
        }
        $.ajax("/api/ssh/StartVM/" + t + "/", { //+ $(this).val(), {
            type: "GET",
            dataType: "json"
        })
            .done
            (function (data) {

            })

    });
    naam = "Start"
    CreateLog(t, naam);
    if ($(".checkToeg").is(":checked") && $(".checkNietToeg").is(":checked") == false) {
        showVmNames($("#servers").val(), true);
    }
    if ($(".checkToeg").is(":checked") == false && $(".checkNietToeg").is(":checked")) {
        showVmNames($("#servers").val(), false);
    } for (var i = 0; i < 100; i++) {
        if (i == 50) {
            showInfo(t);
        }
    }

}
function CreateLog(id, naam) {
    $.ajax("/api/ssh/CreateNaam/" + id + "/" + naam, { //+ $(this).val(), {
        type: "GET",
        dataType: "json"
    })
        .done
        (function (data) {
        })
}
function SaveKlantVM() {
    var t;
    var k = $("#klantenlijst").val();
    $("tr.VmRow").each(function () {
        if ($(this).css('color') == "rgb(244, 78, 66)") {
            t = $(this).attr('id');
            //alert(k);
            $.ajax("/api/SSH/KlantOVM/" + t + "/" + k, {
                type: "GET",
                dataType: "json"
            })

                .done
                (function (data) {
                    //$("div#btns").hide();
                    //$("#btnSave").hide();
                    $("#" + data).css('background-color', 'inherit');
                    $("#" + data).css('color', "rgb(244, 78, 66)");
                    showInfo();
                })
        }
    });
    //     alert($("#servers").val());
    if ($(".checkToeg").is(":checked") && $(".checkNietToeg").is(":checked") == false) {
        showVmNames($("#servers").val(), true);
    }
    if ($(".checkToeg").is(":checked") == false && $(".checkNietToeg").is(":checked")) {
        showVmNames($("#servers").val(), false);
    }
}
$('#Dialog-Box').dialog({
    autoOpen: false,
    height: 500,
    width: 700,
    modal: false
});
function OpenDialog() {
    loadKlanten();
    $('#Dialog-Box').dialog('open');
}
function checkDowntime(id, bool) {
    if (bool == true) {
        alert("Downtime succesvol toegevoegd!")
    }
    if ($("#DivDownTime").css("display") != "none") {
        $("table#Downtime").find("tr:not(:first)").remove();
    } $("#DivDownTime").css("display", "none");


    $.ajax("/api/Klant/SSH/GetDowntime/" + id + "/", {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            $.each(data, function (index, value) {
                $("table#Downtime").append("<tr><td>" + value.Gebruikersnaam + "</td><td>" + value.Start + "</td><td>" + value.Eind + "</td></tr>");
            })
        })
    $("#DivDownTime").css("display", "block");
}
//////////=====================================================================

function ScheduleDownTime() {
    $("tr.VmRow").each(function () {
        if ($(this).css('color') == "rgb(244, 78, 66)") {
            id = $(this).attr('id');
        }
    })
    var date1;
    date1 = $("tr#sched").children("td").children("input#date").val();
    var date2;
    date2 = $("tr#sched2").children("td").children("input#date").val();
    var time1;
    time1 = $("tr#sched").children("td").children("input#time").val();
    var time2 = $("tr#sched2").children("td").children("input#time").val();
    var startdate;
    startdate = (date1.replace("-", "", )).replace("-", "");
    var starttime;
    starttime = time1.replace(":", "");
    var einddate = (date2.replace("-", "", )).replace("-", "");
    var eindtime = time2.replace(":", "");
    var begin = startdate + starttime;

    var eind = einddate + eindtime;
    $.ajax("/api/Klant/SSH/ScheduleDowntime/" + id + "/" + begin + "/" + eind + "/", {
        type: "GET",
        dataType: "json"
    })
        .done
    checkDowntime(id, true);
}

///////////DOWNTIME=============================================================
function DownTime(id, duur) {
    $.ajax("/api/Klant/SSH/PushDowntime/" + id + "/"+ duur + "/", {
        type: "GET",
        dataType: "json"
    })
        .done
        (function (data) {
        })

};
//////////=====================================================================
