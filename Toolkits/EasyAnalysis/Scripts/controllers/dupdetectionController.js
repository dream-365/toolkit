controllers.controller('dupdetectionController', ['$scope', '$location', '$routeParams', '$http',
        function ($scope, $location, $routeParams, $http) {
            $scope.repository = $routeParams.repository;

            $http.get('http://eas-api.azurewebsites.net/api/dupdetection?repository=uwp')
             .then(function (response) {
                 $scope.items = response.data;
             });
        }]);