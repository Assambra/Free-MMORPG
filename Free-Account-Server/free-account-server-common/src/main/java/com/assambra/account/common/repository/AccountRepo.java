package com.assambra.account.common.repository;

import com.assambra.account.common.entity.Account;
import com.tvd12.ezydata.mongodb.EzyMongoRepository;
import com.tvd12.ezyfox.database.annotation.EzyQuery;
import com.tvd12.ezyfox.database.annotation.EzyRepository;

@EzyRepository("accountRepo")
public interface AccountRepo extends EzyMongoRepository<Long, Account> {

    @EzyQuery("{$query: {_id:?0}, $update: {$set: {?1:?2}}}")
    void updateStringFieldById(Long id, String field, String value);

    @EzyQuery("{$query: {?0:?1}, $fields: [?2], $orderBy: {_id:1, ?2:1}}")
    Account getFieldValueByFieldAndValue(String field, String value, String retValue);
}
