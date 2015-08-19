var LoginController = function ($scope, $routeParams, $window, LoginFactory)
{
    $scope.loginForm = {
        emailAddress: '',
        password: '',
        rememberMe: false,
        returnUrl: $routeParams.returnUrl,
        loginFailure: false
    };

    $scope.login = function () {
        var result = LoginFactory($scope.loginForm.emailAddress, $scope.loginForm.password, $scope.loginForm.rememberMe);
        result.then(function (result) {
            if (result.success) {
                $window.location.href = '/Feed';
            } else {
                $scope.loginForm.loginFailure = true;
            }
        });
    }
}

LoginController.$inject = ['$scope', '$routeParams', '$window', 'LoginFactory'];