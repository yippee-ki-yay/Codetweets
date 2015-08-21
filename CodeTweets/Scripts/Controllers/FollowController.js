var FollowController = function($scope, $http)
{
    $scope.isDisabled = true;

    $scope.row = {
        show  : false
    };

    $http.post('/Explore/UsersJson')
        .then(function (response) {
            $scope.AllUsers = response.data;
        }, function (response) {
        alert('server not ok');
        });

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

    $scope.follow = function (user) {
        $http.post('/Feed/addFollow', { "userId":user })
        .then(function (response) {
            $scope.row.show = true;
            $scope.removeRow(user);
        }, function (response) {
            alert('server not ok');
        });
    }
}

FollowController.$inject = ['$scope', '$http'];