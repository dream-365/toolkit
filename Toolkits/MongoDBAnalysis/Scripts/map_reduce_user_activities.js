var MONTH = 'sep';

var map = function () {
	var authorId = this.authorId;

	var threadId = this.id;

    emit(authorId, { activities: [{date: new Date(this.createdOn), type: 'Ask', to: threadId}] });

	var messages = this.messages;

	for(var index = 1; index < messages.length; index++)
	{
        var msg = messages[index];

		var histories = msg.histories;

        emit(authorId, { activities: [{date: new Date(msg.createdOn), type: 'Reply', to: threadId}] });

		for(var i = 0; i < histories.length; i++)
		{
			var record = histories[i];

            emit(record.user, { activities: [{date: new Date(record.date), type: record.type, to: threadId}] });
		}
	}
}

var reduce = function (key, values) {
	reducedVal = { activities: [] };

    for (var idx = 0; idx < values.length; idx++) {

    	var activities = values[idx].activities;

    	for(var j =0; j < activities.length; j++)
    	{
    		reducedVal.activities.push(activities[j]);
    	}
    }

    return reducedVal;
}


db[MONTH + '_threads'].mapReduce(
                     map,
                     reduce,
                     { out: {replace: "user_activies", db: 'temp'} }
                   );


var tempdb = db.getSiblingDB('temp');

var cursor = tempdb.user_activies.find();

cursor.forEach(function(data) {
    db.user_profiles.update(
       { "_id": data._id},
       {
           $addToSet: { activities: { $each: data.value.activities } }
       },
       { upsert: true }
    );
});


cursor = db.user_profiles.find();

cursor.forEach(function(profile) {
    var summary = {
      ask: 0,
      reply: 0,
      mark: 0
    };

    for(var i = 0; i < profile.activities.length; i++)
    {
      var activity = profile.activities[i];

      if(activity.type === 'MarkAnswer')
      {
        summary.mark = summary.mark + 1;
      }

      if(activity.type === 'Ask')
      {
        summary.ask = summary.ask + 1;
      }

      if(activity.type === 'Reply')
      {
        summary.reply = summary.reply + 1;
      }
    }

    db.user_profiles.update(
       { "_id": profile._id},
       {
          $set: { summary: summary }
       }
    );
});


cursor = db.user_profiles.find({display_name: { $exists: false } });

cursor.forEach(function(profile) {
    var result = db.users.find({id: profile._id});

    if(result.hasNext())
    {
      var user = result.next();

      db.user_profiles.update(
       { "_id": profile._id},
       {
          $set: { display_name: user.display_name }
       }
    );
    }
});
