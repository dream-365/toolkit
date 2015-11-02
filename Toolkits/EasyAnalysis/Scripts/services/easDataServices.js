// [production]: eas-api.azurewebsites.net
// [dev]: localhost:58116

var web_api_config = {
    host: 'eas-api.azurewebsites.net'
    // host: 'localhost:58116'
}

app.factory('userProfileService', ['$http', function ($http) {
    return {
        search: function (resp, name) {
            return $http.get('http://' + web_api_config.host
                               + '/api/UserProfile?repository=' + encodeURIComponent(resp)
                               + '&month=&display_name=' + encodeURIComponent(name));
        },
        list: function (resp, month, length) {
            return $http.get('http://' + web_api_config.host
                                       + '/api/UserProfile?repository=' + encodeURIComponent(resp)
                                       + '&month=' + month
                                       + '&length=' + length);
        }
    }
}]);
