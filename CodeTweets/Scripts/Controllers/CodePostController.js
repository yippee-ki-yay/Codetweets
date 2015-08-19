var CodePostController = function($scope, $http)
{
    var placeholder = 'some data';

    $scope.codePost =
        {
            title: 'tit',
            content: 'vontre',
            type: 'fgf'
        };

    $scope.listCodePosts = function()
    {

    }

    $scope.newCodePost = function()
    {
        $http.post('/RoutesDemo/submitCodePost', {"title":$scope.codePost.title, "content": $scope.codePost.content, "type": $scope.codePost.type})
        .then(function (response) {
            alert(response);
        }, function (response) {
            alert('server not ok');
        });
    }
}

CodePostController.$inject = ['$scope', '$http'];