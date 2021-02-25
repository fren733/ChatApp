$(document).ready(function () {
    $("#addRemoveFriend").click(function () {
        var username = $(".main-header h1").text();

        $.ajax({
            dataType: "json",
            url: "/Home/AddRemoveFriend",
            type: "POST",
            data: { username: username },
            error: function () {
                console.log("zjebało się :((((");
            }
        });

        $.ajax({
            dataType: "json",
            url: "/Home/AddNotification",
            type: "POST",
            data: {username: username}
        });
    });
});


