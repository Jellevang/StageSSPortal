$(document).ready(function () {
    checkPass();
});


function checkPass() {
    $.ajax('/api/Gebruiker/GetGebruiker', {
        type: 'GET',
        dataType: 'json'
    })
        .done(function (data) {
            var k = data.MustChangePassword;
            
            if (k === true) {
                alert("verander eerst je passwoord");
                window.location.href = '/Manage/ChangePassword';
            }

        })
};