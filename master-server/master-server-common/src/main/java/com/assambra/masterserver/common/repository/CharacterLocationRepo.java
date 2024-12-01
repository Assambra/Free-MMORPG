package com.assambra.masterserver.common.repository;

import com.assambra.masterserver.common.entity.CharacterLocation;
import com.tvd12.ezydata.mongodb.EzyMongoRepository;
import com.tvd12.ezyfox.database.annotation.EzyRepository;

@EzyRepository("characterLocationRepo")
public interface CharacterLocationRepo extends EzyMongoRepository<Long, CharacterLocation> {
}
