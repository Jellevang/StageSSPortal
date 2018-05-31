function DownTime(id, duur) {
    alert(id +"  "+duur)
    $.ajax("/api/Klant/SSH/PushDowntime/" + id + "/"+ duur + "/", {
        type: "GET",
        dataType: "json"
    })
        .done
        (function (data) {
        })

};

