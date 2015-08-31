var ChatHistoryController = function ($scope, $http)
{
    $http.post('/Chat/getUsersITalkedTo')
  .then(function (response) {
      $scope.users = response.data;
  }, function (response) {
      alert('server not ok');
  });
}

ChatHistoryController.$inject = ['$scope', '$http'];