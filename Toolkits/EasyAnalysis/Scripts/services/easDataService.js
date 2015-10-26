app.factory('easDataService', ['$http', function ($http) {
    return {
        getUserProfiles: function (repository, page, lenth) {
            return $http.get('http://app-svr.cloudapp.net/api/userprofile?page=1&length=20');
        }
    };
}]);