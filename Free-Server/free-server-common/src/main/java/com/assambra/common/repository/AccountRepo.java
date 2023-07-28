package com.assambra.common.repository;

import com.assambra.common.entity.Account;
import com.tvd12.ezydata.mongodb.EzyMongoRepository;
import com.tvd12.ezyfox.database.annotation.EzyQuery;
import com.tvd12.ezyfox.database.annotation.EzyRepository;

@EzyRepository("accountRepo")
public interface AccountRepo extends EzyMongoRepository<Long, Account>{

    @EzyQuery("{$query: {_id:?0}, $update: {$set: {?1:?2}}}")
    void updateStringFieldById(long id, String field, String value);
}
