package com.assambra.game.common.repository;

import com.assambra.game.common.entity.User;
import com.tvd12.ezydata.mongodb.EzyMongoRepository;
import com.tvd12.ezyfox.database.annotation.EzyRepository;

@EzyRepository("accountRepo")
public interface UserRepo extends EzyMongoRepository<Long, User>
{

}
