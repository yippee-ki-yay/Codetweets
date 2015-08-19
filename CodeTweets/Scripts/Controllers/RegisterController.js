var RegisterController = function ($scope, $window, RegisterFactory) {
    $scope.registerForm = {
        emailAddress: '',
        password: '',
        confirmPassword: '',
        user: '',
        registrationFailure: false
    };

    $scope.register = function () {
        var result = RegisterFactory($scope.registerForm.emailAddress, $scope.registerForm.password, $scope.registerForm.confirmPassword, $scope.registerForm.user);
        result.then(function (result) {
            if (result.success) {
                $window.location.href = '/Feed';
            } else {
                $scope.registerForm.registrationFailure = true;
            }
        });
    }
}

RegisterController.$inject = ['$scope', '$window', 'RegisterFactory'];