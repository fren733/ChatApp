$.ajax({
    url: "/Home/GetNotifications",
    type: "GET",
    success: function (data) {
        $.each(data, function (i, item) {
            var li = document.createElement("li");
            var p = document.createElement("p");
            p.innerText = item.username + " is looking for friend";
            var a = document.createElement("a");
            a.setAttribute("href", item.content);
            var date = document.createElement("p");
            date.innerText = item.date;

            a.append(p);
            a.append(date);
            li.append(a);

            $(".dropdown-friends ul").prepend(li);
        });
    }
});

connection.on("ReceiveNotification", function (content, date, username) {
    console.log("powiadomienie");

    var li = document.createElement("li");
    var p = document.createElement("p");
    p.innerText = + username + " looking for friend";
    var a = document.createElement("a");
    a.setAttribute("href", content);
    var date = document.createElement("p");
    date.innerText = date;

    a.append(p);
    a.append(date);
    li.append(a);

    $(".dropdown-friends ul").prepend(li);

});