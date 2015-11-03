controllers.controller('askersController', ['$scope', '$location', '$routeParams', 'userProfileService',
        function ($scope, $location, $routeParams, userProfileService) {
            $scope.state = 'init';

            $scope.repository = $routeParams.repository;

            $scope.UserNameText_keypress = function (e) {
                if (e.which === 13) {
                    $scope.state = 'search';

                    if ($scope.UserNameText) {
                        userProfileService.newsearch('uwp', $scope.UserNameText, ['aug', 'sep'])
                                          .then(function (response) {
                                              $scope.user_profiles = response.data;
                                              $scope.state = 'details';
                                          });
                        //userProfileService.search('uwp', $scope.UserNameText)
                        //                  .then(function (response) {
                        //                      $scope.user_profiles = response.data;
                        //                      $scope.state = 'details';
                        //                  });
                    }
                    else {
                        userProfileService.newlist('uwp', 20, ['aug', 'sep'])
                                          .then(function (response) {
                                              $scope.user_profiles = response.data;
                                              $scope.state = 'init';
                                          });
                        //userProfileService.list('uwp', 'sep', 20)
                        //      .then(function (response) {
                        //          $scope.user_profiles = response.data;
                        //          $scope.state = 'init';
                        //      });
                    }

                }
            }

            

            userProfileService.newlist('uwp', 20, ['aug', 'sep'])
                                          .then(function (response) {
                                              $scope.user_profiles = response.data;
                                          });

            //userProfileService.list('uwp', 'sep', 20)
            //                  .then(function (response) {
            //                      $scope.user_profiles = response.data;
            //                  });
        }]);
