
var connection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();
connection.start();
$(document).ready(function () {

    var href = $("#sendChatMessage").attr("href");
    var connectionId = "";
    var message = "";
    var url = href;

    $(document).on("keyup", "#message", function () {

        if (url.includes("?connectionId")) {
            message = "&message=" + $(this).val();
            url = href + connectionId + message;
        } else if (url.includes("&connectionId")) {
            message = "?message=" + $(this).val();
            url = href + message + connectionId;
        } else {
            message = "?message=" + $(this).val();
            url = href + message;
        }
        console.log(url);
    })

    $(document).on("click", "#usersList li a", function (e) {
        e.preventDefault();
        $("#usersList li a").removeClass("text-success");
        $("#chatUser h3").text($(this).text());
        $(this).addClass("text-success");

        if (url.includes("?message")) {
            connectionId = "&connectionId=" + $(this).attr("data-connectionId");
            url = href + message + connectionId;
        } else if (url.includes("&message")) {
            connectionId = "?connectionId=" + $(this).attr("data-connectionId");
            url = href + connectionId + message;
        } else {
            connectionId = "?connectionId=" + $(this).attr("data-connectionId");
            url = href + connectionId;
        }
    })

    connection.on("ReceiveMessage", function (fullname, message) {
        var li = document.createElement("li");
        $(li).addClass("list-group-item mb-0");
        $(li).text(fullname + " - " + message);
        $("#messageList").append(li);
        console.log("li:" + li);
    });

    connection.on("LoadingPage", function (userId, userConnectionId) {
        $('[data-userId="' + userId + '"]').attr("data-connectionId", userConnectionId);
        console.log("1.1_&cId: " + url.includes("&connectionId"));
        console.log("2.1_?cId: " + url.includes("?connectionId"));
        if (url.includes("&connectionId")) {
            url = url.replace(url.substring(url.indexOf("connectionId")), "connectionId=" + userConnectionId);
            connectionId = connectionId.replace(connectionId.substring(connectionId.indexOf("connectionId")), "connectionId=" + userConnectionId)
            console.log("1.2_&cId-true_url: " + url);
        } else if (url.includes("?connectionId")) {
            url = url.replace(url.substring(url.indexOf("connectionId"), url.indexOf("&message")), "connectionId=" + userConnectionId);
            connectionId = connectionId.replace(connectionId.substring(connectionId.indexOf("connectionId")), "connectionId=" + userConnectionId);
            console.log("2.2_?cId-true_url: " + url);
        }
    })


    $(document).on("click", "#sendChatMessage", function (e) {
        e.preventDefault();
        if (url.includes("connectionId=&") || url.includes("connectionId=") && url.substring(url.indexOf("connectionId=")).length <= 13) {
            if (url.includes("?connectionId=&")) {
                url = url.replace("?connectionId=&", "?");
            } else if (url.includes("&connectionId=")) {
                url = url.replace("&connectionId=", "");
            }
        }
        ResponseHtml();
    })
    async function ResponseHtml() {
        const response = await fetch(url)
            .then(resp => resp.json())
            .then(data => {
                if (data.status == 200) {
                    console.log("200")
                }
            }
            );
    }
});