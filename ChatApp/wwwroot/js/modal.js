var userPage = 0;
var modalUsername = "";

$('#new-message-button').click(function () {
    $('.modal').show();
});

$('.close').click(function () {
    $('.modal').hide();
});

$("#open-modal").click(function () {
    $(".modal").show();
});

$(".modal-profile-button").click(function () {
    if (modalUsername != $(".modal-profile-inner input").val()) {
        userPage = 0;
    }

    var username = $(".modal-profile-inner input").val();

    $.ajax({
        url: "/Home/GetUsers",
        data: {
            partOfName: username,
            page: userPage
        },
        type: "json",
        success: function (data) {
            if (data) {
                if (userPage == 0) {
                    $(".modal-profile-list ul").empty();
                }

                $.each(data, function (i, item) {

                    var li = document.createElement("li");
                    var img = document.createElement("img");
                    var p = document.createElement("p");
                    var a = document.createElement("a");
                    img.src = item.source;
                    p.innerText = item.username;
                    a.href = item.about;
                    a.append(img);
                    a.append(p);
                    li.append(a);

                    $(".modal-profile-list ul").append(li);
                    $(".modal-profile-list").show();

                    $("#modal-arrow").show();
                });
                userPage++;
            }
            else {
                $("#modal-arrow").hide();
            }
        }
    });

    modalUsername = $(".modal-profile-inner input").val();

});

$("#modal-arrow").click(function () {
    $(".modal-profile-button").click();
});

