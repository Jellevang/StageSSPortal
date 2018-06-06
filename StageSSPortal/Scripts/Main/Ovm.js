    $(document).ready(function () {
        //$("button#test").click(function () {
        // alert("test");
        $("select").hide();
    $("#btnSave").hide();
        $("table#LijstServerVMs").delegate("tr:not(:first)", "click", function () {
        $(".VmRow").css("color", "rgb(0, 0, 0)");
    $(this).css("color", "rgb(244, 78, 66)");
            showInfo();
        });
        showVmNames();

    });

    function checkForVms() {
        $("select").hide();
    $("#btnSave").hide();
        $.ajax("/api/SSH/CheckVms", {
        type: "GET",
            dataType: "json"
        })
            .done(function (data) {
        ShowVms(data);
    })
    }
    function loadKlanten() {
        $("select").show();
    $("#btnSave").show();
        $.ajax("/api/SSH/Klanten", {
        type: "GET",
            dataType: "json"
        })
            .done(function (data) {
        $("#klantenlijst").find("option").remove();
    $.each(data, function (index, value) {
        $("#klantenlijst").append("<option class=klant> " + value.Naam + "</option>");
    })
            });
    }
    function showVmNames() {
        //alert("vmnames");
        if ($("#ShowVms").css("display") != "none") {
        $("table#LijstServerVMs").find("tr:not(:first)").remove();
    $("#VmInfo").css("display", "none");
        }
        $.ajax("/api/SSH/vmsDB", {
        type: "GET",
            dataType: "json"
        })
            .done(function (data) {
        //   alert("vmnamesdone")
        checkForVms();
    //ShowVms(data);
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
        // alert(vm.KlantId)

        $("table#LijstServerVMs").append("<tr  class=VmRow id=" + vm.id + ">" + "<td>" + vm.Name + "</td>" /*+ "<td>" + vm.Status + "</td>"*/ + "</tr>");
    if (vm.KlantId == 0) {
        $(this).css("font-weigth", "bold");
    }
    }
    function showInfo() {
        $("select").hide();

    $("#ListVmInfo").find("tr").remove();
        $("#VmInfo").css("display", "none");
        var t;
        $("tr.VmRow").each(function () {
            if ($(this).css('color') == "rgb(244, 78, 66)") {
        //t = $(this).children("td:first").attr("id");
        t = $(this).attr('id');
    //t.replace(".", "-");
    //  alert(t);
    }
        })
        $.ajax("/api/SSH/info/" + t, { //+ $(this).val(), {
        type: "GET",
            dataType: "json"
        })
            .done(function (data) {
        loadKlanten();
    var text = data;
                //alert(text);
                //alert("test");
                $.each(data, function (index, value) {
        $("table#ListVmInfo").append("<tr class=VmInfoRow> <td>" + value + "</td> </tr>");
    })
                $("#VmInfo").css("display", "block");
                $("table#ListVmInfo").find("tr:first").css("font-weight", "bold");

            })
    };


    function StopVM() {
        var t;
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
        }); showVmNames();

        //document.location.reload();

    };

    function SaveKlantVM() {
        var t;
        //alert("t=" + t);

        var k = $("#klantenlijst").val();
        $("tr.VmRow").each(function () {
            if ($(this).css('color') == "rgb(244, 78, 66)") {
        t = $(this).attr('id');
    //alert("t=" + t);
    $.ajax("/api/SSH/KlantOVM/" + t + "/" + k, {
        type: "GET",
                    dataType: "json"
                })
                    .done
                    (function (data) {
        $("select").hide();
    $("#btnSave").hide();
                    })
            }
        });
        showVmNames();
    }
    function StartVM() {
        var t;
        $("tr.VmRow").each(function () {
            if ($(this).css('color') == "rgb(244, 78, 66)") {
        //t = $(this).children("td:first").attr("id");
        t = $(this).attr('id');
    //t.replace(".", "-");
    //alert(t);
            }
            $.ajax("/api/SSH/StartVM/" + t, { //+ $(this).val(), {
        type: "GET",
                dataType: "json"
            })
                .done
                (function (data) {

    })
        });
        //document.location.reload();
        showVmNames();


    };
