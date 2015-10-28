controllers.controller('dupdetectionController', ['$scope', '$location', '$routeParams', '$http',
        function ($scope, $location, $routeParams, $http) {
            $scope.repository = $routeParams.repository;

            var PAGE_LENGTH = 10;

            var local_data = null;

            var current_page = 1;

            function getItems(page) {
                var length = local_data.length;

                var start = (page - 1) * PAGE_LENGTH;

                if(start > (length - 1))
                {
                    return null;
                }

                var result = [];

                var take = length - start;

                take = take > 10 ? 10 : take;

                for(var i = 0; i < take; i++)
                {
                    var index = start + i;

                    result.push(local_data[index]);
                }

                return result;
            }

            function removeItem(id) {
                var length = local_data.length;

                for (var i = 0; i < length; i++) {

                    if (id === local_data[i]._id) {

                        local_data.splice(i, 1);

                        return;
                    }
                }
            }

            $http.get('http://eas-api.azurewebsites.net/api/dupdetection?repository=uwp')
             .then(function (response) {
                 local_data = response.data;
                 $scope.items = getItems(current_page);
             });

            $scope.remove_item = function(id)
            {
                removeItem(id);

                $scope.items = getItems(current_page);
            }
        }]);