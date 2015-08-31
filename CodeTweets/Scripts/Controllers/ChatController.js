var ChatController = function ($scope, $http, $document) {
    $scope.openChatWindows = 0;
    $scope.chatUserId = '';

    $http.post('/Chat/getUnseenMessageNumber')
       .then(function (response) {
           $scope.unseenMsgNumber = response.data;
       }, function (response) {
           alert('server not ok');
       });


    //when we send a message myname is the user you are sending to and name is your name
    //when you are receiving message myname is your name and name is the user sending the message
    $scope.loadChatHistory = function(userId)
    {
        $http.post('/Chat/getChatConversation', {'otherUserId':userId})
        .then(function (response) {
            var msgs = response.data;

            $.each(msgs, function(i, item) {
                chat.client.sendPrivateMessage(item.toId, item.toUser, item.content, item.fromUser, item.msgState);
            })

        }, function (response) {
            alert('server not ok');
 });
    }

    chat.client.showTyping = function(name)
    {
        var sendWindow = $('#' + name);

        sendWindow.append('<div>Is Typing...</div>');

    }

    chat.client.sendPrivateMessage = function (id, myname, msg, name, me) {

        var newWindow = null;
        var currWindow = $('#' + myname); //find an open window with this name
        var sendWindow = $('#' + name);

        if (currWindow.length == 0 && sendWindow.length == 0) //if doesn't exists add new window
        {
            $scope.addChatWindow(id, name);

            newWindow = $("#" + name);
        }
        else
            newWindow = $("#" + myname);

        if (sendWindow.length != 0)
            newWindow = $("#" + name);

        newWindow.append($scope.message(msg, name, me));
    }

    $.connection.hub.start().done(function () { chat.server.connectUser(); });

    //generates msg html based on is it receive msg or sent
    $scope.message = function (msg, name, me) {
        var msg_type = "";

        if (me === "CurrentUser")
            msg_type = "_sent";
        else
            msg_type = "_receive";

        var avatar = "<div class='col-md-2 col-xs-2 avatar'> \
                                   <img src='http://www.bitrebels.com/wp-content/uploads/2011/02/Original-Facebook-Geek-Profile-Avatar-1.jpg' class=' img-responsive '> \
                               </div>";

        var stringMsg = "<div class='col-xs-10 col-md-10'> \
                                   <div class='messages msg" + msg_type + "'> \
                                       <p>  " + msg + " </p> \
                                       <time datetime='2009-11-13T20:00'>10 minutes ago</time> \
                                   </div> \
                               </div>";

        var str = "";

        if (msg_type === "_receive")
            str = " <div class='row msg_container base_receive'> \
                               " + avatar + "\
                               " + stringMsg + "\
                           </div>";
        else
            str = " <div class='row msg_container base_sent'> \
                               " + stringMsg + "\
                               " + avatar + "\
                           </div>";

        return str;
    }


    $scope.addChatWindow = function (userId, userName) {

        $scope.chatUserId = userId;

        var chatHtml = "<div class='row chat-window col-md-3' id='chat_window_" + $scope.openChatWindows + "' style='margin-left:10px;'> \
               <div class='col-xs-12 col-md-12 '> \
                   <div class='panel panel-default' data-id='lel'> \
                      <div id='userId' style='display: none;'>" + userId + "</div>\
                       <div class='panel-heading top-bar'> \
                           <div class='col-md-8 col-xs-8'> \
                               <h3 class='panel-title'><span class='glyphicon glyphicon-comment'></span> Chat - "+ userName + "</h3> \
                           </div> \
                           <div class='col-md-4 col-xs-4' style='text-align: right;'> \
                               <a href='#'><span id='minim_chat_window' class='glyphicon glyphicon-minus icon_minim'></span></a> \
                               <a href='#'><span class='glyphicon glyphicon-remove icon_close' data-id='chat_window_1'></span></a> \
                           </div> \
                       </div> \
                       <div class='panel-body msg_container_base' id='" + userName +"'> \
                       </div> \
                                   <div class='panel-footer'> \
                               <div class='input-group'> \
                                   <input id='btn-input' type='text' class='form-control input-sm chat_input' placeholder='Write your message here...' /> \
                                   <span class='input-group-btn'> \
                                       <button class='btn btn-primary btn-sm' id='btn-chat'>Send</button> \
                                   </span> \
                               </div> \
                           </div> \
                       </div> \
                   </div> \
               </div> \
           </div> \
       </div>";

        $("#chatStrip").append(chatHtml);
        $("#chat_window_" + $scope.openChatWindows).css("margin-left", 300 * $scope.openChatWindows);

        $scope.openChatWindows++;
    }

    $(document).on('keyup', '#btn-input', function (e) {
        if (e.keyCode === 13) {
            chat.server.sendPrivateMessage($('#btn-input').val(), $('#userId').text());
            $('#btn-input').val('').focus();
        }
        else
        {
            chat.server.isTyping($('#userId').text());
        }
    });

    $($document).on("click", "#btn-chat", function () {

        chat.server.sendPrivateMessage($('#btn-input').val(), $('#userId').text());
        $('#btn-input').val('').focus();

    });

    $(document).on('click', '.icon_close', function (e) {
        $(this).parent().parent().parent().parent().remove();

        var chatWin = $(this).parent().parent().parent().parent();

       // $("#chat_window_1").remove();
    });

}

ChatController.$inject = ['$scope', '$http', '$document'];