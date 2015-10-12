var db = db.getSiblingDB('test')

var cursor = db.user_profiles.aggregate([
    {
        $project: {
            _id: 0,
			id: 1,
			display_name: 1,
            total: 1,
            answered: 1,
			tags: 1
        }
    },
	{
		$sort: {total: -1}
	}
	]);


var result = [];

cursor.forEach(function(data) {
    result.push(data);
});

printjson(result);