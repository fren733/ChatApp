connection.on("ReceiveConnectionInfo", function (username, isOnline, lastOnline) {

    $("#list li").each(function () {
        var name = $(this).find(".card").find("h3").text();

        if (name == username && name != "") {
            var online = $(this).find(".card").find("div");

            online.removeClass("online");
            online.removeClass("offline");

            if (isOnline == "true") {
                online.addClass("online");
                $(this).find(".card").find("div").find("p").text("Online now");
            }
            else {
                online.addClass("offline");
                $(this).find(".card").find("div").find("p").text("last " + lastOnline);
            }
        }
    });
});
