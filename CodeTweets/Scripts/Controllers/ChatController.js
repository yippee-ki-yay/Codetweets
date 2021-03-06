﻿var ChatController = function ($scope, $http, $document, $localStorage) {
    $scope.openChatWindows = 0;
    $scope.chatUserId = '';
    $scope.idleTime = 0;
    $scope.typingTime = 0;

    $scope.$storage = $localStorage;

    $localStorage.windowList = [];

    $.connection.hub.start().done(function () { chat.server.connectUser(); });

    $http.post('/Chat/getUsersITalkedTo')
    .then(function (response) {
        $scope.users = response.data;

     }, function (response) {
        alert('server not ok');
 });

    $http.post('/Chat/getUnseenMessageNumber')
       .then(function (response) {
           $scope.unseenMsgNumber = response.data;
       }, function (response) {
           alert('server not ok');
       });

    $scope.setMessagesSeen = function (toUser) {
        $http.post('/Chat/setMessagesSeen', { 'toUser': toUser })
              .then(function (response) {
                  var num = response.data;
                  $scope.unseenMsgNumber = $scope.unseenMsgNumber - num;
              }, function (response) {
                  alert('server not ok');
              });
    }

    $scope.reloadChat = function ()
    {
        $.each($scope.$storage.windowList, function (i, item) {

            $scope.addChatWindow(item.userId, item.userName);
        });
    }

    $scope.reloadChat();

    //checks users status is Online/Offline/Away
    $scope.updateStatus = function ()
    {
        $(".chat-window").each(function (index) {

            var text = $(this).find('.panel-title').text();
            var title = $(this).find('.panel-title');
            chat.server.isConnected($(this).find('#userId').text()).done(function (result) {

                text = text.replace(/(\()\S+(\))/, "$1" + result + "$2");
                title.text(text);
            });
        });
    }

    setInterval($scope.updateStatus, 5000);
 
        $scope.idleInterval = setInterval(timerIncrement, 1000); 

        $($document).mousemove(function (e) {
            $scope.idleTime = 0;
            chat.server.userBackOnline();
        });
        $($document).keypress(function (e) {
            $scope.idleTime = 0;
            $scope.typingTime = 0;
            chat.server.userBackOnline();
        });
   

        function timerIncrement() {
            $scope.idleTime = $scope.idleTime + 1;
            $scope.typingTime++;
            if($scope.typingTime > 5)
            {
                $('#userId').each(function (index) {
                    chat.server.notTyping($(this).text());
                });
            }

            if ($scope.idleTime > 19) { 
                chat.server.userAway();
            }
        }

    //when we send a message myname is the user you are sending to and name is your name
    //when you are receiving message myname is your name and name is the user sending the message
    $scope.loadChatHistory = function(userId)
    {
        $http.post('/Chat/getChatConversation', {'otherUserId':userId})
        .then(function (response) {
            var msgs = response.data;

           
                $scope.setMessagesSeen(userId);

            $.each(msgs, function(i, item) {
                chat.client.sendPrivateMessage(item.toId, item.toUser, item.content, item.fromUser, item.msgState, item.imgPath);
            })

        }, function (response) {
            alert('server not ok');
 });
    }


    chat.client.removeTyping = function (name) {
        var sendWindow = $('#' + name);

        sendWindow.find('#typing').remove();

    }

    chat.client.userBlocked = function(name)
    {
        var sendWindow = $('#' + name);

        //we remove it so we don't stack the messages
        sendWindow.find('#blocked').remove();

        sendWindow.append("<div class='row msg_container base_receive' id='blocked'>You are blocked by this user</div>");
    }

    //show if the other user is typing
    chat.client.showTyping = function(name)
    {
        var sendWindow = $('#' + name);

        //we remove it so we don't stack the messages
        sendWindow.find('#typing').remove();

        sendWindow.append("<div class='row msg_container base_receive' id='typing'>Is Typing...</div>");

    }

    chat.client.sendPrivateMessage = function (id, myname, msg, name, me, imgPath) {

        var newWindow = null;
        var currWindow = $('#' + myname); //find an open window with this name
        var sendWindow = $('#' + name);

        if (currWindow.length == 0 && sendWindow.length == 0) //if doesn't exists add new window
        {
            $scope.addChatWindow(id, name);
            $scope.reloadChat();
            newWindow = $("#" + name);
        }
        else
            newWindow = $("#" + myname);

        if (sendWindow.length != 0)
            newWindow = $("#" + name);

        newWindow.find('#typing').remove();

        newWindow.append($scope.message(msg, name, me, imgPath));

        //if you load only specific number of messages this number wotn be a problem
        newWindow.scrollTop(newWindow.height()+10000000);
    }

    //generates msg html based on is it receive msg or sent
    $scope.message = function (msg, name, me, imgPath) {
        var msg_type = "";

        if (me === "CurrentUser")
            msg_type = "_sent";
        else
            msg_type = "_receive";

        var avatar = "<div class='col-md-2 col-xs-2 avatar'> \
                                   <img src='/img/" + imgPath + "' class=' img-responsive '> \
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

        //chat.server.isConnected(userId).done(function (result) {
        var result = "Online";

        if ($("#" + userName).length != 0)
            return;

        $scope.loadChatHistory(userId);

        var chatHtml = "<div class='row chat-window col-md-2' id='chat_window_" + $scope.openChatWindows + "' style='margin-left:10px;'> \
               <div class='col-xs-12 col-md-12 '> \
                   <div class='panel panel-default'> \
                      <div id='userId' style='display: none;'>" + userId + "</div>\
                       <div class='panel-heading top-bar'> \
                           <div class='col-md-8 col-xs-8'> \
                               <h3 class='panel-title'><span class='glyphicon glyphicon-comment'></span> Chat - " + userName + " (" + result + ")</h3> \
                           </div> \
                           <div class='col-md-4 col-xs-4' style='text-align: right;'> \
                               <a href='#'><span id='minim_chat_window' class='glyphicon glyphicon-minus icon_minim' data-id='" + $scope.openChatWindows + "'></span></a> \
                               <a href='#'><span class='glyphicon glyphicon-remove icon_close' data-id='" + $scope.openChatWindows  + "'></span></a> \
                           </div> \
                       </div> \
                       <div class='panel-body msg_container_base' id='" + userName +"'> \
                       </div> \
                                   <div class='panel-footer'> \
                               <div class='input-group'> \
                                   <input id='btn-input' type='text' class='form-control input-sm chat_input' data-id='" + $scope.openChatWindows + "' placeholder='Write your message here...' /> \
                                   <span class='input-group-btn'> \
                                       <button class='btn btn-primary btn-sm' id='btn-chat' data-id='" + $scope.openChatWindows + "'>Send</button> \
                                   </span> \
                               </div> \
                           </div> \
                       </div> \
                   </div> \
               </div> \
           </div> \
       </div>";

        $("#chatStrip").append(chatHtml);

        $localStorage.windowList.push({'userId': userId, 'userName':userName});

        $scope.updateStatus();

        var windowWidth = $("#chat_window_" + $scope.openChatWindows).width();
        $("#chat_window_" + $scope.openChatWindows).css("margin-left",  (windowWidth + 10) * $scope.openChatWindows);

        $scope.openChatWindows++;

       // });
    }

    $(document).on('keyup', '#btn-input', function (e) {
        var currWindow = $('#chat_window_' + $(this).data('id'));

        if (e.keyCode === 13) {
            chat.server.sendPrivateMessage(currWindow.find('#btn-input').val(), currWindow.find('#userId').text());
            currWindow.find('#btn-input').val('').focus();
        }
        else
        {
            chat.server.isTyping(currWindow.find('#userId').text());
            //setTimeout(chat.server.notTyping(currWindow.find('#userId').text()), 3000);
        }
    });

    $($document).on("click", "#btn-chat", function () {

        var currWindow = $('#chat_window_' + $(this).data('id'));

        chat.server.sendPrivateMessage(currWindow.find('#btn-input').val(), currWindow.find('#userId').text());
        currWindow.find('#btn-input').val('').focus();

    });

    $(document).on('click', '.icon_close', function (e) {
        var currWindow = $(this).data('id');
        $('#chat_window_' + currWindow).remove();

        $scope.openChatWindows--;
    });

}

ChatController.$inject = ['$scope', '$http', '$document', '$localStorage'];