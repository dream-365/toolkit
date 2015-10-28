var test_db = db.getSiblingDB('test');

var cursor = db.office_asker_activities.aggregate(
    [{
        $group: {
            _id: '$display_name',
            history: {
                $push: {
                    month: "$month",
                    total: "$total",
                    answered: '$answered'
                }
            }
        }
    }]
);


var result = [];

cursor.forEach(function(data) {
    result.push(data);
});

test_db.temp_asker_history.drop();

test_db.temp_asker_history.insert(result);


var this_month = 'sep',
	previous_month = 'aug';


var active_user_group_filter = {
    $and: [{
        history: {
            $elemMatch: {
                month: this_month
            }
        }
    }, {
        history: {
            $elemMatch: {
                month: previous_month
            }
        }
    }]
};

var new_user_group_filter = {
    $and: [{
        history: {
            $elemMatch: {
                month: this_month
            }
        }
    }, {
        history: {
            $not: {
                $elemMatch: {
                    month: previous_month
                }
            }
        }
    }]
};

var strike_user_group_filter = {
    $and: [{
        history: {
            $elemMatch: {
                month: previous_month
            }
        }
    }, {
        history: {
            $not: {
                $elemMatch: {
                    month: this_month
                }
            }
        }
    }]
};


var active_user_group_count = test_db.temp_asker_history.find(active_user_group_filter).count();

var new_user_group_count = test_db.temp_asker_history.find(new_user_group_filter).count();

var strike_user_group_count = test_db.temp_asker_history.find(strike_user_group_filter).count();

print('active_user_group: ' + active_user_group_count);
print('new_user_group: ' + new_user_group_count);
print('strike_user_group: ' + strike_user_group_count);