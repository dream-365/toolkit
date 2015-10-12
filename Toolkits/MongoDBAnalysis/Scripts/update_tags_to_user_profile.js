var db = db.getSiblingDB('test');

db.user_profiles.find().forEach(function(item){
	print(item.id);

	var tags = [];

	db.map_reduce_user_tags.find({"_id.user": item.id}).forEach(function(tag){

		tags.push({name: tag._id.tag, count: tag.value.count});
	});

	db.user_profiles.update({"_id": item._id}, {$set:{tags: tags}});
});
