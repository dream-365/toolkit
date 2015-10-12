var db = db.getSiblingDB('test');

var map_user_tags = function() {
                       for (var idx = 0; idx < this.threads.length; idx++) {

                           var tags = this.threads[idx].tags;

                           if(tags === undefined)
                           {
                          		continue;
                           }

                           for(var j = 0; j < tags.length; j++)
                           {
                           	   var key = {user: this.id, tag: tags[j]};

	                           var value = {
	                                         count: 1,
	                                       };

	                           emit(key, value);
                           }
                       }
                    };


var count_user_tags = function(key, values) {

                     reducedVal = { count: 0 };

                     for (var idx = 0; idx < values.length; idx++) {
                         reducedVal.count += values[idx].count;
                     }

                     return reducedVal;
                  };


db.user_profiles.mapReduce(
                     map_user_tags,
                     count_user_tags,
                     { out: "map_reduce_user_tags" }
                   )