var CodePostController = function($scope, $http, $window)
{
    var placeholder = 'some data';

    $scope.codePost =
        {
            title: 'Title',
            content: 'Lorem ipsum dolor sit amet, velit purus cras laoreet tristique nullam, sollicitudin ad ullamcorper suspendisse, convallis amet fringilla lorem enim dui. Dictum leo aliquam morbi dolor ligula augue, quam sed integer ultricies, quis et. Sodales malesuada. Erat tempor congue ac, justo malesuada sociosqu pede, est vestibulum faucibus id mollis. Consectetuer scelerisque, amet praesentium id imperdiet, arcu aliquam',
            type: 'Lisp'
        };

        $http.post('/Feed/getUserPosts')
       .then(function (response) {
           $scope.posts = response.data;
       }, function (response) {
           alert('server not ok');
       });
    

    //tweet.replace(/(\#\S+)/g, "<a href=\"\"> $1 </a>");

   // $scope.listMyCodePosts();

    $scope.search = function()
    {
        $http.get('/Feed/userPosts', {hashtag: 'ipsum'})
       .then(function (response) {
           alert('src');
       }, function (response) {
           alert('server not ok');
       });
    }

    $scope.listCodePosts = function()
    {
     /*   $http.post('/Feed/getUserPosts')
      .then(function (response) {
          $scope.pos
      }, function (response) {
          alert('server not ok');
      });*/
    }

    $scope.getRow = function (Id) {
        var arr = eval($scope.posts);
        for (var i = 0; i < arr.length; i++) {
            if (arr[i].id === Id)
                return arr[i];
        }

    }

    $scope.retweet = function(post_id)
    {
        $http.post('/Feed/Retweet', { "id": post_id })
      .then(function (response) {
          alert("Retweet");
          $window.location.reload();
          //$window.location.reload();
      }, function (response) {
          alert('server not ok');
      });
    }

    $scope.like = function(post_id)
    {
        $http.post('/Feed/Like', { "id": post_id})
       .then(function (response) {
           if (response.data === "success")
           {
               var curr = $scope.getRow(post_id);
               curr.like++;
           }
       }, function (response) {
           alert('server not ok');
       });
    }

    $scope.hate = function(post_id)
    {
        $http.post('/Feed/Hate', { "id": post_id })
              .then(function (response) {
                  if(response.data === "success")
                  {
                      var curr = $scope.getRow(post_id);
                      curr.hate++;
                  }
              }, function (response) {
                  alert('server not ok');
              });
    }

    $scope.newCodePost = function()
    {
        $http.post('/RoutesDemo/submitCodePost', {"title":$scope.codePost.title, "content": $scope.codePost.content, "type": $scope.codePost.type})
        .then(function (response) {
            $window.location.href = '/Feed/UserPosts';
        }, function (response) {
            alert('server not ok');
        });
    }
}

CodePostController.$inject = ['$scope', '$http', '$window'];