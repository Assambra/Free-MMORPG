package com.assambra.masterserver.common.repository;

import com.assambra.masterserver.common.entity.User;
import com.tvd12.ezydata.mongodb.EzyMongoRepository;
import com.tvd12.ezyfox.database.annotation.EzyQuery;
import com.tvd12.ezyfox.database.annotation.EzyRepository;

@EzyRepository("userRepo")
public interface UserRepo extends EzyMongoRepository<Long, User> {

    @EzyQuery("{$query: {_id:?0}, $update: {$set: {?1:?2}}}")
    void updateStringFieldById(Long id, String field, String value);

    @EzyQuery("{$query: {?0:?1}, $fields: [?2], $orderBy: {_id:1, ?2:1}}")
    User getFieldValueByFieldAndValue(String field, String value, String retValue);
}
