Webcache: load the cache data from forums
Content Analyze: anlyze the metadata from web page

database: forums
collections:
users
asker_prifles
answer_prifles
uwp_{month}_threads
uwp_asker_activities
uwp_user_tags

*Askers Activities Analysis*

# Import new threads to uwp_{month}_threads collection
[in]: uwp_{month}_threads.json > import to mongo db > db.uwp_{month}_threads

# Import new users to current users collection 

# Run asker analysis to extract the actions of askers
[output]: db.uwp_asker_activities

# Map-reduce user tags
in: db.uwp_asker_activities
output: uwp_user_tags

# Update user tags to db.uwp_asker_activities

# Update/create asker profile base on the asker activities



