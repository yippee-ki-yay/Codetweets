var ChatController = function ($scope, $http, $document)
{

    $scope.chatUserId = '';

    chat.client.addNewMessageToPage = function (name, message) {
        // Add the message to the page.
        $('#discussion').append('<li><strong>' + htmlEncode(name)
            + '</strong>: ' + htmlEncode(message) + '</li>');
    };


    chat.client.sendPrivateMessage = function (msg, name, me) {

        $('.msg_container_base').append($scope.message(msg, name, me));

       /* $($document).on("append", ".msg_container_base", function () {
            $scope.message(msg, name);
        });*/
        }

    $('#message').focus();
        
    $.connection.hub.start();

    function htmlEncode(value) {
        var encodedValue = $('<div />').text(value).html();
        return encodedValue;
    }

    $scope.message = function(msg, name, me)
    {
        var msg_type = "";

        if (me === "CurrentUser")
            msg_type = "base_receive";
        else
            msg_type = "base_sent";

        var str = " <div class='row msg_container " + msg_type +"'> \
                                <div class='col-md-2 col-xs-2 avatar'> \
                                    <img src='http://www.bitrebels.com/wp-content/uploads/2011/02/Original-Facebook-Geek-Profile-Avatar-1.jpg' class=' img-responsive '> \
                                </div> \
                                <div class='col-xs-10 col-md-10'> \
                                    <div class='messages msg_receive'> \
                                        <p>  " + msg + " </p> \
                                        <time datetime='2009-11-13T20:00'></time> \
                                    </div> \
                                </div> \
                            </div>";

        return str;
    }


    $scope.addChatWindow = function(userId, userName)
    {
        
        $scope.chatUserId = userId;

        var chatHtml = "<div class='row chat-window col-md-3' id='chat_window_1' style='margin-left:70px;'> \
                <div class='col-xs-12 col-md-12 '> \
                    <div class='panel panel-default'> \
                       <div id='userId' style='display: none;'>" + userId+ "</div>\
                        <div class='panel-heading top-bar'> \
                            <div class='col-md-8 col-xs-8'> \
                                <h3 class='panel-title'><span class='glyphicon glyphicon-comment'></span> Chat - "+ userName +"</h3> \
                            </div> \
                            <div class='col-md-4 col-xs-4' style='text-align: right;'> \
                                <a href='#'><span id='minim_chat_window' class='glyphicon glyphicon-minus icon_minim'></span></a> \
                                <a href='#'><span class='glyphicon glyphicon-remove icon_close' data-id='chat_window_1'></span></a> \
                            </div> \
                        </div> \
                        <div class='panel-body msg_container_base'> \
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
    }


    $($document).on("click", "#btn-chat", function () {


        
        chat.server.sendPrivateMessage($('#btn-input').val(), $('#userId').text());


    });

}

ChatController.$inject = ['$scope', '$http', '$document'];