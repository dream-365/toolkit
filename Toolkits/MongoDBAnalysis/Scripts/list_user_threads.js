var db = db.getSiblingDB('forums')

var cursor = db.uwp_sep_threads
    .aggregate([
    { $match : { authorId : 'babbd8ec-f63d-4020-93c2-aeccb4b6318b' } },
    {
        $project: {
            _id: 0,
            title: 1,
            url: 1,
            answered: 1
        }
    }]);

var result = [];

cursor.forEach(function(data) {
    result.push(data);
});

printjson(result);