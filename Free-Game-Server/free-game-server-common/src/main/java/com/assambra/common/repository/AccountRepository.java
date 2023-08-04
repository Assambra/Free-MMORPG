package com.assambra.common.repository;

import com.assambra.common.entity.Account;
import com.tvd12.ezydata.mongodb.EzyMongoRepository;
import com.tvd12.ezyfox.database.annotation.EzyRepository;

@EzyRepository("accountRepository")
public interface AccountRepository extends EzyMongoRepository<Long, Account>
{

}
