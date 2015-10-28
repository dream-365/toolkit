var cursor = db.thread_profiles.aggregate([
  {$match: { tags: { $all: [ "uwp" ] }}},
  {$project: { _id: 0, tags: 1 } },
  {$unwind: "$tags" },
  {$group: { _id: "$tags", freq: { $sum: 1 } }},
  {$project: { _id: 0, tag: "$_id", freq: 1 } },
  {$sort: { freq: -1 } }
]);

cursor.forEach(function(item){
	printjson(item);
});