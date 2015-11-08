var db = db.getSiblingDB('uwp')

//Get the amount of asked and marked questions for user
var cursor = db.asker_activities.aggregate([
    {
        $group: {
            _id: '$id',
            asked: {
                $sum: '$total'
            },
            answered: {
                $sum: '$answered'
            },
            marked: {
                $sum: '$marked'
            },
            months: {
                $push: '$month'
            }
        }
    },
    {
        $project: {
            _id: 0,
            id: '$_id',
            asked: 1,
            answered: 1,
            marked: 1,
            months: 1
        }
    },
    {
        $match: {
            asked: {
                $gte: 7
            }
        }
    }

])

var result = [];

cursor.forEach(function (data) {
    var _tags = [];
    var _threads = [];
    var _marked_threads = [];

    var op = db.asker_activities.find({ id: data.id });

    op.forEach(function (item) {
        //Unpacking Tags
        item.tags.forEach(function (tag) {
            _tags.push(tag);
        });

        //Unpacking Threads
        item.threads.forEach(function (thread) {
            _threads.push(thread);
        });

        //Unpacking Marked Threads
        if (item.marked_threads) {
            item.marked_threads.forEach(function (thread) {
                _marked_threads.push(thread);
            });
        }

        data['tags'] = _tags;
        data['threads'] = _threads;
        data['marked_threads'] = _marked_threads;

        result.push(data);
    });
});

printjson(result);