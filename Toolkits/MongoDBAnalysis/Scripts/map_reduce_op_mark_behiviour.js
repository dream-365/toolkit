var MONTH = 'sep';

var map = function () {
	var authorId = this.authorId;

	var threadId = this.id;

	var messages = this.messages;

	for(var index = 1; index < messages.length; index++)
	{
		var histories = messages[index].histories;

		for(var i = 0; i < histories.length; i++)
		{
			var record = histories[i];

			if(record.type === 'MarkAnswer' && record.user === authorId)
			{
				var key = authorId;

				var value = {
					count: 1,
					threads: [ {id: threadId, mark_on: new Date(record.date)}]
				};

				emit(key, value);

				return;
			}
		}
	}
}

var reduce = function (key, values) {
	reducedVal = { count: 0, threads: [] };

    for (var idx = 0; idx < values.length; idx++) {
    	reducedVal.count += values[idx].count;

    	var threads = values[idx].threads;

    	for(var j =0; j < threads.length; j++)
    	{
    		reducedVal.threads.push(threads[j]);
    	}
    }

    return reducedVal;
}


db[MONTH + '_threads'].mapReduce(
                     map,
                     reduce,
                     { out: {replace: "op_mark_behiviour", db: 'temp'} }
                   );


var tempdb = db.getSiblingDB('temp');


var cursor = tempdb.op_mark_behiviour.find().sort( { 'value.count': -1 } );


cursor.forEach(function(data) {
    // set mark data to asker_activities
    var cursor = db.asker_activities.find({id: data._id, month: MONTH}).limit(1);

    if(cursor.hasNext())
    {
    	var item = cursor.next();

    	db.asker_activities.update({_id: item._id}, {$set: {marked: data.value.count, marked_threads: data.value.threads }});

    	print('update activity of ' + item.display_name);
    }else{
    	print('not found');
    }
});
