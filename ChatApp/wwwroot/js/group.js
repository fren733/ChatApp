function Details() {
    var group = $(".group-details");
    if (group.css("display") == "none" && ChosenGroup != "") {
        $(".group-details").show();
        $(".group-details").css("display", "grid");

        var url = "/Home/GroupDetails";

        $(".group-inner").empty();
        $(".group-inner").load(url, { id: ChosenGroup }, function () {
            $(".delete-user").click(function () {
                $(this).parent().remove();
            });

            $(".remove-group-button").click(function () {
                $.ajax({
                    url: "/Home/DeleteGroup",
                    type: "POST",
                    data: { id: ChosenGroup },
                    success: function () {
                        $(".content .talks #list li").each(function () {
                            if ($(this).attr("id") == ChosenGroup) {
                                $(this).remove();
                            }
                        });
                        $(".group-inner").empty();
                        $(".group-details").hide();
                        $(".messages ul").empty();
                    }
                });
            });

            $(".update-group-button").click(function () {
                var name = $("#NewGroupName").val();
                var desc = $("#AboutGroup").val();
                var list = [];
                var images = document.getElementById("GroupImageFile").files;

                $(".group-inner ul li").each(function () {
                    list.push($(this).find("p").text());
                });

                var formData = new FormData();
                formData.append("groupId", ChosenGroup);
                formData.append("list", list);
                formData.append("file", images[0]);
                formData.append("name", name);
                formData.append("desc", desc);

                $.ajax({
                    url: "/Home/EditGroup",
                    type: "POST",
                    processData: false,
                    contentType: false,
                    data: formData,
                    success: function (data) {
                        if (data == null) {
                            return 0;
                        }
                        else {
                            group.hide();
                            Details();
                        }
                    }
                });
            });

            AddUser();
        });
    }
    else {
        $(".group-details").hide();
    }
}

function AddUser() {
    $(".add-user-button").click(function () {
        $.ajax({
            url: "/Home/GetFriends",
            type: "GET",
            data: {},
            success: function (data) {
                var select = document.createElement("select");
                var def = document.createElement("option");

                def.value = "default";
                def.innerText = "Choose user";
                select.append(def);

                $.each(data, function (i, item) {
                    var isOnList = false;
                    $(".group-inner ul li").each(function (i, ii) {
                        if ($(this).children("p")[0].innerText === item.username) {
                            isOnList = true;
                        }
                    });

                    if (isOnList === false) {
                        var option = document.createElement("option");
                        option.value = item.username;
                        option.innerText = item.username;
                        select.append(option);
                    }
                });

                $(".add-user").empty();

                $(".add-user").append(select);

                $(".add-user select").on("change", function () {
                    var username = this.value;

                    $.ajax({
                        url: "/Home/GetUser",
                        type: "GET",
                        data: { username: username },
                        success: function (data) {
                            var li = document.createElement("li");
                            var p = document.createElement("p");
                            var img = document.createElement("img");
                            var removeUser = document.createElement("div");
                            var i = document.createElement("i");
                            var button = document.createElement("button");
                            var bi = document.createElement("i");
                            var bp = document.createElement("p");

                            i.setAttribute("class", "fas fa-user-slash");
                            removeUser.setAttribute("class", "delete-user");
                            img.src = data.source;
                            p.innerText = data.username;

                            li.append(img);
                            li.append(p);
                            removeUser.append(i);
                            li.append(removeUser);

                            $(".group-inner ul").append(li);
                            $(".add-user").empty();

                            button.setAttribute("class", "add-user-button");
                            bi.setAttribute("class", "fas fa-user-plus");
                            bp.innerText = "Add user";

                            button.append(bp);
                            button.append(bi);
                            $(".add-user").append(button);

                            AddUser();

                            $(".delete-user").click(function () {
                                $(this).parent().remove();
                            });
                        }
                    });
                });
            }
        });
    });
}

$(".close-details").click(function () {
    $(".group-details").hide();
});


