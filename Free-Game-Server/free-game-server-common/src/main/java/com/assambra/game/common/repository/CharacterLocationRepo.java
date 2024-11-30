package com.assambra.game.common.repository;

import com.assambra.game.common.entity.CharacterLocation;
import com.tvd12.ezydata.mongodb.EzyMongoRepository;
import com.tvd12.ezyfox.database.annotation.EzyRepository;

@EzyRepository("characterLocationRepo")
public interface CharacterLocationRepo extends EzyMongoRepository<Long, CharacterLocation> {
}
