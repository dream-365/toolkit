var controllers = angular.module('controllers', []);

controllers.controller('discoverController', ['$scope', '$location', 'threadService',
        function ($scope, $location, threadService) {
            $scope.state = 'init';

            $scope.URIText_keypress = function (e) {
                if (e.which === 13) {
                    $scope.state = 'search';

                    threadService.query($scope.URIText)
                                 .success(function (identifier) {
                                     $location.url('/detail/' + identifier);
                                 });
                }
            }
        }]);


controllers.controller('detailController', ['$scope', 'threadService', '$location', '$routeParams',
    function ($scope, threadService, $location, $routeParams) {
        $scope.identifier = $routeParams.identifier;

        $scope.state = 'load';

        threadService.detail($scope.identifier)
                     .success(function (data) {
                         $scope.item = data;
                         $scope.state = 'done';
                     });

        $scope.TagText_keypress = function (e) {
            if (e.which === 13) {
                var tags = $scope.item.Tags = $scope.item.Tags || [];

                tags.push($scope.TagText);

                $scope.TagText = '';
            }
        }
    }]);

var app = angular.module('_app_', ['ngRoute', 'controllers']);

app.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/', {
            templateUrl: 'partials/discover.html',
            controller: 'discoverController'
        }).when('/detail/:identifier', {
            templateUrl: 'partials/detail.html',
            controller: 'detailController'
        });
  }]);

app.factory('threadService', ['$http', function ($http) {
    return {
        query: function (uri) {
            var req = {
                method: 'POST',
                url: '/api/thread',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                data: '=' + uri
            }

            return $http(req);
        },
        get: function (id) {
            return $http.get('api/thread/' + id);
        },
        detail: function (id) {
            return $http.get('api/thread/' + id + '/detail');
        },
        addTag: function (id, tag) {
            return $http.post('api/thread/' + id + '/tag/' + tag);
        },
        getTags: function (id) {
            return $http.get('api/thread/' + id + '/tags');
        }
    }
}]);


