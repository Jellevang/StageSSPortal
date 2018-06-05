$(document).ready(function () {
    $("table#LijstServerVMs").delegate("tr:not(:first)", "click", function () {
        $(".VmRow").css("color", "rgb(0, 0, 0)");
        $(this).css("color", "rgb(244, 78, 66)");
        showInfo();
    });
    showVmNames();
    loadKlantenAccounts();
});

function showVmNames() {
    // $("#btns").hide();
    if ($("#ShowVms").css("display") != "none") {
        $("table#LijstServerVMs").find("tr:not(:first)").remove();
        $("#VmInfo").css("display", "none");

    }
    $.ajax("/api/Klant/SSH/AccountOVMs", {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            if (data == false) {
                $("#NoVms").css("display", "block");
            }
            else {
                ShowVms(data);
            }
        })
};
function ShowVms(LijstServerVMs) {
    $.each(LijstServerVMs, function (index, value) {
        addVM(value);
    })
    $("#ShowVms").css("display", "block");
}
function changeColor() {
    $(this).css('background-color', 'rgb(58, 187, 252)');
}
function addVM(vm) {
    $("table#LijstServerVMs").append("<tr  class=VmRow id=" + vm.id + ">" + "<td>" + vm.Name + "</td>" /*+ "<td>" + vm.Status + "</td>"*/ + "</tr>");
}
function showInfo(id) {
    $("#btns").hide();
    $("#ListVmInfo").find("tr").remove();
    $("#VmInfo").css("display", "none");
    if (id == null || id == "") {
        var t;
        $("tr.VmRow").each(function () {
            if ($(this).css('color') == "rgb(244, 78, 66)") {
                //t = $(this).children("td:first").attr("id");
                t = $(this).attr('id');
                //t.replace(".", "-");
                //alert(t);
            }
        })
    } else {
        t = id;
    }

    $.ajax("/api/SSH/info/" + t, { //+ $(this).val(), {
        type: "GET",
        dataType: "json"
    })
        .done(function (data) {
            var text = data;
            //alert(text);
            //alert("test");
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
            $("#VmInfo").css("display", "block");
            $("table#ListVmInfo").find("tr:first").css("font-weight", "bold");
            $("#btns").show();
            checkDowntime(t, false);
            $("tr#" + t).css("color", "rgb(244, 78, 66)");
        })
};
function StopVM() {
    var t;
    var naam;
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
    duur = prompt("Hoelang wilt u de server offline halen? ( in minuten)");
    //document.location.reload();
    naam = "Stop";
    CreateLog(t, naam);
    DownTime(t, duur);
    showVmNames();
    showInfo(t);
};
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
    showVmNames($("#servers").val());
    for (var i = 0; i < 100; i++) {
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
        $.ajax("/api/ssh/StartVM/" + t, { //+ $(this).val(), {
            type: "GET",
            dataType: "json"
        })
            .done
            (function (data) {
            })
    });
    naam = "Start"
    CreateLog(t, naam);
    showVmNames();
    for (var i = 0; i < 100; i++) {
        if (i == 50) {
            showInfo(t);
        }
    }

};
function CreateLog(id, naam) {
    $.ajax("/api/ssh/CreateNaam/" + id + "/" + naam, { //+ $(this).val(), {
        type: "GET",
        dataType: "json"
    })
        .done
        (function (data) {
        })
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
    alert(id + "  " + duur)
    $.ajax("/api/Klant/SSH/PushDowntime/" + id + "/" + duur + "/", {
        type: "GET",
        dataType: "json"
    })
        .done
        (function (data) {
        })

};
//////////=====================================================================