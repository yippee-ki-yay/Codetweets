var CodePostController = function($scope, $http, $window, $location, $interval)
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

    function getJsonFromUrl() {
        var query = $window.location.search.substr(1);
        var result = {};
        query.split("&").forEach(function (part) {
            var item = part.split("=");
            result[item[0]] = decodeURIComponent(item[1]);
        });
        return result;
    }

    $scope.urlJson = getJsonFromUrl();


    $scope.loadData = function () {
        $http.post('/Feed/getUserPosts', { hashtag: $scope.urlJson['hashtag'], type: $scope.urlJson['type'], count: $scope.scroll.count })
           .then(function (response) {
               $scope.posts = response.data;
               $scope.scroll.count = $scope.posts.length;

               for (var i = 0; i < $scope.posts.length; ++i) {
                   $scope.posts[i].content = $scope.posts[i].content.replace(/\#(\S+)/g, "<a href=\"/Feed/UserPosts?hashtag=$1\"> $1 </a>");
                   $scope.posts[i].showComments = false;
                   $scope.posts[i].newComment = false;
               }


           }, function (response) {
               alert('server not ok');
           });
    }

   // $scope.loadData();

    /*$http.post('/Feed/getUserPosts', {type:"feed"})
       .then(function (response) {
           $scope.posts = response.data;
       }, function (response) {
           alert('server not ok');
       });*/
    

    $scope.loadMore = function ()
    {
        if ($scope.scroll.busy == true)
            return;

        $scope.scroll.busy = true;

        $http.post('/Feed/getUserPosts', { hashtag: $scope.urlJson['hashtag'], type: $scope.urlJson['type'], count: $scope.scroll.count })
      .then(function (response) {
          for (var i = 0; i < response.data.length; ++i)
          {
              if ($scope.posts == undefined) {
                  $scope.posts = [];
              }
              response.data[i].content =  response.data[i].content.replace(/\#(\S+)/g, "<a href=\"/Feed/UserPosts?hashtag=$1\"> $1 </a>");
            //  response.data[i].showComments = false;
            //  response.data[i].newComment = false;
              $scope.posts.push(response.data[i]);
              $scope.scroll.busy = false;
              
   
          }
          $scope.scroll.count += 5;
         
      }, function (response) {
          alert('server not ok');
      });

    }

    $scope.refreshPosts = function()
    {
        $scope.scroll.busy = false;
        $scope.scroll.count = 0;
        $scope.posts = undefined;
        $scope.loadMore();
    }

    //refresh the feed every 20 seconds
    setInterval($scope.refreshPosts, 60000);

        $scope.order = function(t)
        {
            $http.post('/Feed/getUserPosts', { orderParam: t, hashtag: $scope.urlJson['hashtag'], type: $scope.urlJson['type'], count: $scope.scroll.count })
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
        var curr = $scope.getRow(post_id);

        if (curr.liked === "You liked this")
        {
           $http.post('/Feed/Unlike', { "id": post_id })
           .then(function (response) {
               if (response.data === "success") {
                   curr.like--;
                   curr.liked = "Like";
               }
           }, function (response) {
               alert('server not ok');
           });
        }
        else
        {
           $http.post('/Feed/Like', { "id": post_id })
          .then(function (response) {
              if (response.data === "success") {
                  curr.like++;
                  curr.liked = "You liked this";
              }
          }, function (response) {
              alert('server not ok');
          });
        }
   
    }

    $scope.listComments = function(postId)
    {
        $http.post('/Feed/commentsForPost', { "postId": postId})
           .then(function (response) {
               if (response.data === "success") {

               }
           }, function (response) {
               alert('server not ok');
           });
    }

    $scope.reply = function(postId, txt)
    {
        $http.post('/Feed/Reply', { "postId": postId, "commentContent": txt })
            .then(function (response) {
                var post = $scope.getRow(postId);
                post.commentList.push(response.data);
                post.newComment = false;
                post.showComments = true;
                $scope.commentText = "";
            }, function (response) {
                alert('server not ok');
            });
    }

    $scope.hate = function(post_id)
    {
        var curr = $scope.getRow(post_id);

        if (curr.hated === "Hate")
        {
            $http.post('/Feed/Hate', { "id": post_id, 'state': 'hate' })
            .then(function (response) {
                if (response.data === "success") {
                    curr.hate++;
                    curr.hated = "You hate this";
                }
            }, function (response) {
                alert('server not ok');
            });
        }
        else
        {
            $http.post('/Feed/Hate', { "id": post_id, 'state': 'unhate' })
            .then(function (response) {
                if (response.data === "success") {
                    curr.hate--;
                    curr.hated = "Hate";
                }
            }, function (response) {
                alert('server not ok');
            });
        }
      
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

CodePostController.$inject = ['$scope', '$http', '$window', '$location', '$interval'];