       $(function () {
            // Reference the auto-generated proxy for the hub.
            var chat = $.connection.chatHub;
            // Create a function that the hub can call back to display messages.
            chat.client.addNewMessageToPage = function (name, message) {
                // Add the message to the page.
                $('#discussion').append('<li><strong>' + htmlEncode(name)
                    + '</strong>: ' + htmlEncode(message) + '</li>');
            };


            chat.client.sendPrivateMessage = function(msg, name)
            {
                $('#discussion').append('<li><strong>' + htmlEncode(name)
                   + '</strong>: ' + htmlEncode(msg) + '</li>');
            }

            // Get the user name and store it to prepend to messages.
            // $('#displayname').val(prompt('Enter your name:', ''));
            // Set initial focus to message input box.
            $('#message').focus();
            // Start the connection.
            $.connection.hub.start().done(function () {

                chat.server.connectUser();

                $('#sendmessage').click(function () {
                    // Call the Send method on the hub.
                    // chat.server.send($('#displayname').val(), $('#message').val());

                    chat.server.sendPrivateMessage($('#message').val(), "60966f14-c73e-4a18-b9aa-30d45410ae07");

                    // Clear text box and reset focus for next comment.
                    $('#message').val('').focus();
                });
            });
        });
    // This optional function html-encodes messages for display in the page.
    function htmlEncode(value) {
        var encodedValue = $('<div />').text(value).html();
        return encodedValue;
    }


