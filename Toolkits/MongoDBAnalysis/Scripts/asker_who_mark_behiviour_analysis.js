var cursor = db.asker_activities.aggregate([{
    $group: {
        _id: "$month",
        marked: {
            $sum: "$marked"
        },
        answered: {
            $sum: "$answered"
        },
        total: {
            $sum: "$total"
        }
    }
}]);

cursor.forEach(function(data) {
    printjson(data);
});


cursor = db.asker_activities.aggregate([{
    $match: {
        marked: {
            $gt: 0
        }
    }
}, {
    $group: {
        _id: "$month",
        marked_user: {
            $sum: 1
        }
    }
}]);

cursor.forEach(function(data) {
    printjson(data);
});


cursor = db.asker_activities.aggregate([{
    $group: {
        _id: "$month",
        total_user: {
            $sum: 1
        }
    }
}]);

cursor.forEach(function(data) {
    printjson(data);
});