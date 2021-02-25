var iterator = 0;

if ($("ul#list li").length == 0) {
    $(".nothing-found").show();
}

function search() {
    var searchString = $('#search-talks').val().toLowerCase();

    $("ul#list li").each(function () {
        var correctTalk = $(this).find('.card').find('h3').text().toLowerCase();

        if (correctTalk.includes(searchString)) {
            $(this).show();
            iterator++;
        }
        else {
            $(this).hide();
        }
    });

    if (iterator == 0) {
        $(".nothing-found").show();
    }
    else {
        $(".nothing-found").hide();
    }
    iterator = 0;
}

function searchModal() {
    iterator = 0;
    var searchString = $('#search-talks-modal').val().toLowerCase();

    $("ul#friends-modal li").each(function () {
        var correctTalk = $(this).find('.card').find('h3').text().toLowerCase();

        if (correctTalk.includes(searchString)) {
            $(this).show();
            iterator++;
        }
        else {
            $(this).hide();
        }
    });

    if (iterator == 0) {
        $(".modal-nothing-found").show();
    }
    else {
        $(".modal-nothing-found").hide();
    }
    iterator = 0;
}

function Start() {
    var input = $('#search-message');

    if (input.css('display') != 'none') {
        input.hide();
    }
    else {
        input.show();
    }
}

function searchMessage() {
    var messageToFind = $('#search-message').val().toLowerCase();

    $('.messages ul li').each(function () {
        var correctMessage = $(this).find("div").find('p').text().toLowerCase();

        if (correctMessage.includes(messageToFind)) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });
}

function searchUser() {
    var usernameToSearch = $('#username').val().toLowerCase();

    $('.modal-user-list li').each(function () {
        var correctUser = $(this).find('.card').find('h3').text().toLowerCase();

        if (correctUser.includes(usernameToSearch)) {
            $(this).show();
        }
        else {
            $(this).hide();
        }
    });

    if (iterator == 0) {
        $(".nothing-found").show();
    }
    else {
        $(".nothing-found").hide();
    }
    iterator = 0;
}


