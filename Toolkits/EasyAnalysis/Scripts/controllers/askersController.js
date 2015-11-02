controllers.controller('askersController', ['$scope', '$location', '$routeParams', 'userProfileService',
        function ($scope, $location, $routeParams, userProfileService) {
            $scope.state = 'init';

            $scope.repository = $routeParams.repository;

            $scope.UserNameText_keypress = function (e) {
                if (e.which === 13) {
                    $scope.state = 'search';

                    userProfileService.search('uwp', $scope.UserNameText)
                                      .then(function (response) {
                                          $scope.user_profiles = response.data;
                                          $scope.state = 'details';
                                      });

                }
            }

            userProfileService.list('uwp', 'sep', 20)
                              .then(function (response) {
                                  $scope.user_profiles = response.data;
                              });
        }]);
