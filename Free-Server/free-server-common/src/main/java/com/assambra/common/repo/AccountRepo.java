package com.assambra.common.repo;

import com.assambra.common.entity.Account;
import com.tvd12.ezydata.mongodb.EzyMongoRepository;
import com.tvd12.ezyfox.database.annotation.EzyRepository;

@EzyRepository("accountRepo")
public interface AccountRepo extends EzyMongoRepository<Long, Account>{

}
