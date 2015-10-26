controllers.controller('askersController', ['$scope', '$location', '$routeParams',
        function ($scope, $location, $routeParams) {
            $scope.repository = $routeParams.repository;

            $scope.user_profiles = "";
        }]);