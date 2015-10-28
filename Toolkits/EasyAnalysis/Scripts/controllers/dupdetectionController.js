controllers.controller('threadexplorerController', ['$scope', '$location', '$routeParams', '$http',
        function ($scope, $location, $routeParams, $http) {
            $scope.repository = $routeParams.repository;
        }]);