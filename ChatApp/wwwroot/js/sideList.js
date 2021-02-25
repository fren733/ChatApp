$(window).resize(function () {
    if ($(window).width() > 1055) {
        ShowTalks();
    }
});

$(".show-talks").click(function () {
    if ($(".content .talks").css("display") === "none") {
        ShowTalks();
    }
    else {
        HideTalks();
    }
});

function ShowTalks() {
    $(".content .talks").css("display", "grid");
    $(".show-talks").css("width", "80%");
    $(".show-talks").empty();

    var i = document.createElement("i");

    i.setAttribute("class", "fas fa-align-left");
    $(".show-talks").append(i);
}

function HideTalks() {
    $(".content .talks").css("display", "none");

    if ($(window).width() < 770) {
        $(".show-talks").css("width", "3em");
    }
    else {
        $(".show-talks").css("width", "4em");
    }

    $(".show-talks").empty();

    var i = document.createElement("i");
    i.setAttribute("class", "fas fa-align-right");

    $(".show-talks").append(i);
}