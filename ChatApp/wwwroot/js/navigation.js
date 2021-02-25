function scrollToBottom() {
    var element = document.getElementById("main");
    element.scrollTop = element.scrollHeight;
}

scrollToBottom();

function nav() {
    var navbar = document.getElementById("mySidenav").style.width;
    console.log(navbar.valueOf());


    if (document.getElementById("mySidenav").style.width == 0) {
        document.getElementById("mySidenav").style.width = "210px";
    }
    else {
        document.getElementById("mySidenav").style.width = "";
    }
}