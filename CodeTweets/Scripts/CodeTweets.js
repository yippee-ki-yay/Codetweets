var CodeTweets = angular.module('CodeTweets', ['ngRoute']);

CodeTweets.controller('LandingPageController', LandingPageController);
CodeTweets.controller('LoginController', LoginController);
CodeTweets.controller('CodePostController', CodePostController);
CodeTweets.controller('RegisterController', RegisterController);
CodeTweets.controller('FollowController', FollowController);

CodeTweets.factory('LoginFactory', LoginFactory);
CodeTweets.factory('RegisterFactory', RegisterFactory);

var configFunction = function ($routeProvider)
{
    $routeProvider.
         when('/routeOne', {
             templateUrl: 'routesDemo/one'
         })
        .when('/routeTwo', {
            templateUrl: 'routesDemo/two'
        })
        .when('/routeThree', {
            templateUrl: 'routesDemo/three'
        })
         .when('/AddCodePost', {
             templateUrl: 'routesDemo/addCodePost'
         })
         .when('/register', {
             templateUrl: 'Account/Register',
             controller: RegisterController
         })
        .when('/login?returnUrl', {
            templateUrl: 'Account/Login',
            controller: LoginController
        });;
}

configFunction.$inject = ['$routeProvider'];

CodeTweets.config(configFunction);