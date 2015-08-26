var CodePostController = function($scope, $http, $window, $location)
{
    var placeholder = 'some data';

    $scope.codePost =
        {
            title: 'Title',
            content: 'Lorem ipsum dolor sit amet, velit purus cras laoreet tristique nullam, sollicitudin ad ullamcorper suspendisse, convallis amet fringilla lorem enim dui. Dictum leo aliquam morbi dolor ligula augue, quam sed integer ultricies, quis et. Sodales malesuada. Erat tempor congue ac, justo malesuada sociosqu pede, est vestibulum faucibus id mollis. Consectetuer scelerisque, amet praesentium id imperdiet, arcu aliquam',
            type: 'Lisp'
        };

    $scope.scroll =
        {
            busy: "false",
            count: 0
        };

    var searchObject = $location.search();

    $http.post('/Feed/getUserPosts', {hashtag:$scope.hashtag})
       .then(function (response) {
           $scope.posts = response.data;
           $scope.scroll.count = $scope.posts.length;

           for (var i = 0; i < $scope.posts.length; ++i)
           {
               $scope.posts[i].content = $scope.posts[i].content.replace(/\#(\S+)/g, "<a href=\"/Feed/UserPosts?hashtag=$1\"> $1 </a>");
           }


       }, function (response) {
           alert('server not ok');
       });

    /*$http.post('/Feed/getUserPosts', {type:"feed"})
       .then(function (response) {
           $scope.posts = response.data;
       }, function (response) {
           alert('server not ok');
       });*/
    

    $scope.loadMore = function ()
    {
        console.log("skrolaj");

        if ($scope.scroll.busy == true)
            return;

        $scope.scroll.busy = true;

        $http.post('/Feed/getUserPosts', { count: $scope.scroll.count })
      .then(function (response) {
          for (var i = 0; i < response.data.length; ++i)
          {
              if ($scope.posts != undefined)
              {
                  $scope.posts.push(response.data[i]);
                  $scope.scroll.busy = false;
              }
   
          }
          $scope.scroll.count += 5;
         
      }, function (response) {
          alert('server not ok');
      });

        console.log('get more data');
    }

    //tweet.replace(/(\#\S+)/g, "<a href=\"\"> $1 </a>");

   // $scope.listMyCodePosts();

        $scope.order = function(type)
        {
            $http.post('/Feed/getUserPosts', { orderParam :type})
                .then(function (response) {
                 $scope.posts = response.data;
            }, function (response) {
             alert('server not ok');
             });
        }


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

CodePostController.$inject = ['$scope', '$http', '$window', '$location'];