var controllers = angular.module('controllers', []);

controllers.controller('discoverController', ['$scope', '$location', 'threadService',
        function ($scope, $location, threadService) {
            $scope.state = 'init';

            $scope.URIText_keypress = function (e) {
                if (e.which === 13) {
                    $scope.state = 'search';

                    threadService.query($scope.URIText)
                                 .success(function (result) {
                                     $location.url('/detail/' + result.repository + '/' + result.identifier);
                                 });
                }
            }
        }]);

controllers.controller('detailController', ['$scope', 'threadService', '$location', '$routeParams',
    function ($scope, threadService, $location, $routeParams) {
        // model state init
        $scope.identifier = $routeParams.identifier;
        $scope.repository = $routeParams.identifier;

        $scope.state = 'load';

        $scope.model = {
            categorySelect: '-1',
            typeSelect: '-1'
        };

        threadService.types($scope.identifier)
            .success(function (data) {
                $scope.data = data;

                // load detail data
                threadService.detail($scope.identifier)
                             .success(function (data) {
                                 $scope.item = data;

                                 var typeId = $scope.item.TypeId;

                                 var vm = calculateSelection(typeId);

                                 $scope.model.categorySelect = vm.categorySelect;

                                 // temp workaround to update the UI, refactor the code
                                 // in the future
                                 setTimeout(function () {
                                     $scope.$apply(function () {
                                         $scope.model.typeSelect = vm.typeSelect;
                                     });
                                 }, 0);

                                 $scope.state = 'done';
                             });
            });

        $scope.remoteUrlRequestFn = function (str) {
            return { q: str };
        };

        $scope.$watch('selectedTag', function (newValue, oldValue) {
            if (newValue === undefined)
            {
                return;
            }

            var newTag = typeof newValue === 'string'
                      ? newValue
                      : newValue.originalObject.Name;

            threadService
                .addTag($scope.identifier, newTag)
                .success(function (tag) {
                    if (tag === '') {
                        return;
                    }

                    var tags = $scope.item.Tags = $scope.item.Tags || [];

                    tags.push(tag);
                });
        });


        $scope.Tag_click = function () {
            console.log("tag click");
        }

        $scope.typeSelectChange = function () {
            threadService.classify($scope.identifier, $scope.model.typeSelect);
        }

        $scope.back = function () {
            $location.url('/');
        }

        function calculateSelection(id)
        {
            var vm = {
                categorySelect: '-1',
                typeSelect: '-1'
            };

            for (var i = 0; i < $scope.data.categories.length; i++)
            {
                var group = $scope.data.typeGroups[i];

                for (var j = 0; j < group.length; j++)
                {
                    if(group[j].id == id)
                    {
                        vm.categorySelect = i.toString();
                        vm.typeSelect = id.toString();
                    }
                }
            }

            return vm;
        }
    }]);

var app = angular.module('_app_', ['ngRoute', 'controllers', 'angucomplete-alt']);

app.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
        when('/', {
            templateUrl: 'partials/discover.html',
            controller: 'discoverController'
        }).when('/detail/:repository/:identifier', {
            templateUrl: 'partials/detail.html',
            controller: 'detailController'
        });
  }]);

app.factory('threadService', ['$http', function ($http) {
    return {
        query: function (uri) {
            // TODO: CODE_REFACTOR
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
        classify: function(threadId, typeId) {
            return $http.post('api/thread/' + threadId + '/classify/' + typeId);
        },
        types: function(id) {
            return $http.get('api/thread/' + id + '/types');
        },
        detail: function (id) {
            return $http.get('api/thread/' + id + '/detail');
        },
        addTag: function (id, tag) {
            // TODO: CODE_REFACTOR
            var req = {
                method: 'POST',
                url: 'api/thread/' + id + '/tag/',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                data: '=' + encodeURIComponent(tag)
            }

            return $http(req);
        },
        getTags: function (id) {
            return $http.get('api/thread/' + id + '/tags');
        }
    }
}]);