var ChosenGroup = '';
var page = 0;


// Prepare side list as links to open groups
$(document).ready(function () {
    $("ul#list li").each(function () {
        $(this).click(function () {

            var name = $(this).find('.card').find('h3').text();
            if (ChosenGroup != this.id) {
                ChosenGroup = this.id;

                var src = $(this).find('.profile-picture').find('.circle').find('img').attr('src');
                $('#group-main-image').attr('src', src);

                $('.messages ul').empty();
                $('.header h3').text(name);



                GetGroup(this.id, page);
            }
        });
    });
});


// Listen when user press enter and calls send button click
document.getElementById("message-text").addEventListener("keyup", function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        document.getElementById("send").click();
    }
});


connection.on("ReceiveMessage", function (messageId, imageId, groupId) {

    if (groupId == ChosenGroup) {

        $.ajax({
            url: "/Home/GetMessage",
            dataType: "json",
            type: "GET",
            data: {
                messageId: messageId,
                imageId: imageId
            },
            success: function (data) {
                if (data != null) {
                    ShowMessage(data.username, data.profileImage, data.image, data.text, data.date);
                }
            }
        });
    }
    else {
        $(".content .talks #list li").each(function () {
            if ($(this).attr("id") == groupId) {
                $(this).prependTo($(".content .talks #list"));
            }
        });

    }
});


// Calls when new group is created and append one to side list
connection.on("ReceiveNewGroup", function (name, src, id) {
    $('#nothing-found-side-list').hide();

    var li = document.createElement("li");
    li.setAttribute("id", id);

    var profilePicture = document.createElement("div");
    profilePicture.setAttribute("class", "profile-picture");

    var circle = document.createElement("div");
    circle.setAttribute("class", "circle");
    var img = document.createElement("img");
    img.src = src;
    circle.append(img);
    profilePicture.append(circle);

    var card = document.createElement("div");
    card.setAttribute("class", "card");

    var h = document.createElement("h3");
    h.innerHTML = name;
    card.append(h);

    var online = document.createElement("div");
    online.setAttribute("class", "offline");
    var i = document.createElement("i");
    i.setAttribute("class", "fas fa-dot-circle");
    online.append(i);
    var lastDate = document.createElement("p");
    lastDate.innerHTML = "20.20.20";
    online.append(lastDate);
    card.append(online);

    var lastMessage = document.createElement("p");
    lastMessage.innerHTML = "Send something";
    card.append(lastMessage);

    li.append(profilePicture);
    li.append(card);

    //$("#list").prepend(li);

    $("#list li").first().click(function () {

        ChosenGroup = id;

        $('.messages ul').empty();
        $('.header h3').text(name);

        GetGroup(id, page);
    });
});


// Send message by ajax calling
$("#send").click(function () {
    var talk = $('.main .header h3').text().trim();
    var text = $('#message-text').val();
    var files = document.getElementById("ImageFile").files;  
    var file = files[0];
    var formData = new FormData();                           

    if (text != "" && talk != "") {
        formData.append("file", files[0]);
        formData.append("text", text);
        formData.append("talk", talk);

        $.ajax({                                           
            url: "/Home/SendMessage",                     
            data: formData,                               
            processData: false,                             
            contentType: false,                            
            type: "POST",                                   
            success: function (data) {                    
                var date = GetCurrentDate();
                var profileImage = "test";
                $.ajax({
                    url: "/Home/GetUser",
                    data: { username: identity },
                    type: "GET",
                    contentType: "json",
                    success: function (data) {
                        if (data.source != null)
                            profileImage = data.source;
                    }
                });

                if (file != null) {
                    var reader = new FileReader();
                    reader.onloadend = function () {
                        var imageSrc = reader.result;

                        ShowMessage(identity, data.source, imageSrc, text, date);
                    }
                    reader.readAsDataURL(file);
                }
                else {
                    ShowMessage(identity, profileImage, null, text, date);
                }

                $(".content .talks #list li").each(function () {
                    if ($(this).attr("id") == ChosenGroup) {
                        $(this).prependTo(".content .talks #list");
                    }
                });
            },
            error: function () {  }
        });
        $('.footer input').val('');
    }
});


// Close full-size image
$('.close-image').click(function () {
    $('.image-modal').hide();
});


// Calls when user adding new group
$(".modal-inner button").click(function () {
    var selected = [];
    var talkName = $("#talk-name").val();
    var files = document.getElementById("ModalImageFile").files;
    var description = $("#description").val();
    var formData = new FormData();

    $("#friends-modal li").each(function () {
        if ($(this).find(".modal-options").find(".container").find("input").is(":checked")) {
            selected.push($(this).find(".card").find("h3").text());
        }
    });

    formData.append("usersList", selected);
    formData.append("file", files[0]);
    formData.append("name", talkName);
    formData.append("description", description);

    $.ajax({
        url: "/Home/AddGroup",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: (data) => {
            $('.modal').hide();
            if (data != null) {
                AppendGroupToList(data.id, data.src, data.name);
            }   
        }
    });
});


// Enables to all images open in full size
function prepareImages() {
    $("img").each(function () {

        $(this).click(function () {
            $(".image-modal-content") = this.src;
            $(".image-modal").show();
        });
    });
}


// Prepare and append DOM for one message
function ShowMessage(username, profileImage, image, content, datetime) {

    var li = document.createElement("li");

    var messageHeader = document.createElement("div");
    messageHeader.setAttribute("class", "message-header");
    var header = document.createElement("h5");
    header.innerText = username;
    messageHeader.append(header);

    var circle = document.createElement("div");
    circle.setAttribute("class", "circle");
    var profile = document.createElement("img");

    if (profileImage != null) {
        profile.src = profileImage;
    }
    else {
        profile.src = "~\Images\anonymous.png";
    }
    circle.append(profile);

    var messageInner = document.createElement("div");
    var text = document.createElement("p");
    text.innerText = content;
    messageInner.append(text);

    if (image != null) {
        var img = document.createElement("img");
        img.src = image;
        messageInner.append(img);
    }

    var date = document.createElement("div");
    date.setAttribute("class", "message-date");
    var dateString = document.createElement("h5");
    dateString.innerText = datetime;
    date.append(dateString);
    messageInner.append(date);

    li.append(messageHeader);
    li.append(circle);
    li.append(messageInner);

    if (username == identity) {
        li.setAttribute("class", "my-message-outer");
        messageInner.setAttribute("class", "my-message-inner");
    }
    else {
        li.setAttribute("class", "message-outer");
        messageInner.setAttribute("class", "message-inner");
    }

    $(".messages ul").append(li);

    scrollToBottom();

}


// Get current date and time
function GetCurrentDate() {
    var currentdate = new Date();
    var datetime = currentdate.getDate() + "."
        + (currentdate.getMonth() + 1) + "."
        + currentdate.getFullYear() + ", "
        + currentdate.getHours() + ":"
        + currentdate.getMinutes() + ":"
        + currentdate.getSeconds();

    return datetime;
}


// Get groups data async from server
function GetGroup(id, page) {
    $.ajax({
        dataType: 'json',
        url: '/Home/GetGroup',
        type: 'GET',
        data: {
            id: id,
            page: page
        },
        success: function (data) {
            $(".main ul").empty();

            if (data != null) {
                $.each(data, function (i, item) {
                    ShowMessage(item.username, item.profileImage, item.image, item.text, item.date);
                    $("img").each(function () {

                        $(this).click(function () {
                            $(".image-modal-content").attr("src", $(this).attr("src"));
                            $(".image-modal").show();
                        });
                    });
                    page++;
                });
            }
        }
    });
    Details();
    $(".group-details").hide();

    if ($(document).width() < 1080) {
        HideTalks();
    }
}


// Prepare and append DOM for one group to side list when its created
function AppendGroupToList(id, src, name) {
    $('#nothing-found-side-list').hide();

    var li = document.createElement("li");
    li.setAttribute("id", id);

    var profilePicture = document.createElement("div");
    profilePicture.setAttribute("class", "profile-picture");

    var circle = document.createElement("div");
    circle.setAttribute("class", "circle");
    var img = document.createElement("img");
    img.src = src;
    circle.append(img);
    profilePicture.append(circle);

    var card = document.createElement("div");
    card.setAttribute("class", "card");

    var h = document.createElement("h3");
    h.innerHTML = name;
    card.append(h);

    var online = document.createElement("div");
    online.setAttribute("class", "offline");
    var i = document.createElement("i");
    i.setAttribute("class", "fas fa-dot-circle");
    online.append(i);
    var lastDate = document.createElement("p");
    lastDate.innerHTML = GetCurrentDate();
    online.append(lastDate);
    card.append(online);

    var lastMessage = document.createElement("p");
    lastMessage.innerHTML = "Send something";
    card.append(lastMessage);

    li.append(profilePicture);
    li.append(card);

    $("#list").prepend(li);

    $("#list li").first().click(function () {

        ChosenGroup = id;

        $('.messages ul').empty();
        $('.header h3').text(name);

        GetGroup(id, page);
    });
}


// Auto scroll to bottom in window with messages
function scrollToBottom() {
    $('.messages').scrollTop($('.messages')[0].scrollHeight - $('.messages')[0].offsetHeight);
}


