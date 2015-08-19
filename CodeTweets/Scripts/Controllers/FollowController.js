var FollowController = function($scope, $http)
{
    $scope.isDisabled = true;

    $scope.row = {
        show  : false
    };

    $scope.follow = function (user) {
        $http.post('/Feed/addFollow', { "userId":user })
        .then(function (response) {
            $scope.row.show = true;
            alert("Follow success");
        }, function (response) {
            alert('server not ok');
        });
    }
}

FollowController.$inject = ['$scope', '$http'];