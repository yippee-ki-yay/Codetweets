var UsersController = function ($scope, $http) {

    $http.post('/Explore/UsersJson')
     .then(function (response) {
         $scope.AllUsers = response.data;
     }, function (response) {
         alert('server not ok');
     });
}

UsersController.$inject = ['$scope', '$http'];