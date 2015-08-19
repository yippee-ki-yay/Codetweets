var FollowController = function($scope, $http)
{
    $scope.follow = function (user) {
        $http.post('/Feed/addFollow', { "userId":user })
        .then(function (response) {
            alert(response);
        }, function (response) {
            alert('server not ok');
        });
    }
}

FollowController.$inject = ['$scope', '$http'];