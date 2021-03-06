﻿var FollowController = function($scope, $http)
{

    $scope.followed = "Follow";
    $scope.blocked = "Blocked";

    $scope.userType = "All";

    $scope.row = {
        show  : false
    };

    $scope.getUsers = function () {
        $http.post('/Explore/UsersJson', { "searchText": $scope.searchText, "userType": $scope.userType })
            .then(function (response) {
                $scope.AllUsers = response.data;

                for (var i = 0; i < $scope.AllUsers.length; ++i)
                {
                    if($scope.AllUsers[i].isBlocked == "Unblock")
                    {
                        $scope.AllUsers[i].isChat = true;
                        $scope.AllUsers[i].isDisabled = true;
                    }
                }

            }, function (response) {
                alert('server not ok');
            });
    }

    $scope.getUsers();

    $scope.search = function()
    {
        $http.post('/Explore/UsersSearchJson', { "searchText": $scope.searchText })
       .then(function (response) {
           $scope.AllUsers = response.data;
       }, function (response) {
           alert('server not ok');
       });
    }

    $scope.removeRow = function(Id)
    {
        var index = -1;
        var comArr = eval($scope.AllUsers);
        for (var i = 0; i < comArr.length; i++) {
            if (comArr[i].Id === Id) {
                index = i;
                break;
            }
        }

        $scope.AllUsers.splice(index, 1);
    }

    $scope.getRow = function(Id)
    {
        var arr = eval($scope.AllUsers);
        for (var i = 0; i < arr.length; i++)
        {
            if (arr[i].Id === Id)
                return arr[i];
        }

    }

    $scope.follow = function (user, state) {
        $http.post('/Feed/addFollow', { "userId":user, type:state})
        .then(function (response) {
            if (response.data === "success")
            {
                var curr = $scope.getRow(user);
                if (curr.isFollowed == "Follow")
                    curr.isFollowed = "Unfollow";
                else
                    curr.isFollowed = "Follow";
            }
        }, function (response) {
            alert('server not ok');
        });
    }

    $scope.block = function (user, state) {
        $http.post('/Feed/addBlock', { "userId": user, type:state })
        .then(function (response) {
            if(response.data === "success")
            {
                var curr = $scope.getRow(user);
                if (curr.isBlocked == "Block") {
                    curr.isBlocked = "Unblock";
                    curr.isDisabled = true;
                    curr.isChat = true;
                }
                else {
                    curr.isBlocked = "Block";
                    curr.isDisabled = false;
                    curr.isChat = false;
                }
            }
        }, function (response) {
            alert('server not ok');
        });
    }
}

FollowController.$inject = ['$scope', '$http'];