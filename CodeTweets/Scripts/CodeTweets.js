var CodeTweets = angular.module('CodeTweets', ['ngRoute']);

CodeTweets.controller('LandingPageController', LandingPageController);
CodeTweets.controller('LoginController', LoginController);
CodeTweets.controller('CodePostController', CodePostController);
CodeTweets.factory('LoginFactory', LoginFactory);

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
        .when('/login?returnUrl', {
            templateUrl: 'Account/Login',
            controller: LoginController
        });;
}

configFunction.$inject = ['$routeProvider'];

CodeTweets.config(configFunction);