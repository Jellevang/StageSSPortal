function DownTime(id) {
    $.ajax("/api/Klant/SSH/PushDowntime/" + id + "/", {
        type: "GET",
        dataType: "json"
    })
        .done
        (function (data) {
        })

};