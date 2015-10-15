Webcache: load the cache data from forums
Content Analyze: anlyze the metadata from web page

database: forum_data
collections:
users
asker_prifles
answer_prifles
{repository}_{month}_threads
{repository}_asker_activities
{repository}_user_tags

*Askers Activities Analysis*

# Import new threads to {repository}_{month}_threads collection
[in]: {repository}_{month}_threads.json > import to mongo db > {repository}_{month}_threads
cmd: mongoimport /db forum_data /collection {repository}_{month}_threads /file {repository}_{month}_threads /jsonArray

# Import new users to current users collection 

# Run asker analysis to extract the actions of askers
[output]: {repository}_asker_activities

# Map-reduce user tags
in: db.{repository}_asker_activities
output: {repository}_user_tags

# Update user tags to db.uwp_asker_activities

# Update/create asker profile base on the asker activities

