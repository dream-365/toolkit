var map = function () {
    var authorId = this.authorId;

    var messages = this.messages;

    for (var index = 1; index < messages.length; index++) {
        var histories = messages[index].histories;

        for (var i = 0; i < histories.length; i++) {
            var record = histories[i];

            if (record.type === 'MarkAnswer' && record.user === authorId) {
                var key = authorId;

                var value = {
                    count: 1
                };

                emit(key, value);

                return;
            }
        }
    }
}

var reduce = function (key, values) {
    reducedVal = { count: 0 };

    for (var idx = 0; idx < values.length; idx++) {
        reducedVal.count += values[idx].count;
    }

    return reducedVal;
}

db.aug_threads.mapReduce(
                     map,
                     reduce,
                     { out: { replace: "op_mark_behiviour", db: 'temp' } }
                   );

var tempdb = db.getSiblingDB('temp');

var cursor = tempdb.op_mark_behiviour.find().sort({ 'value.count': -1 });

var result = [];

cursor.forEach(function (data) {
    result.push(data);
});

printjson(result);