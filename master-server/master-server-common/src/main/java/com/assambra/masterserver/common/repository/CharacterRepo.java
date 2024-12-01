package com.assambra.masterserver.common.repository;

import com.assambra.masterserver.common.entity.Character;
import com.tvd12.ezydata.mongodb.EzyMongoRepository;
import com.tvd12.ezyfox.database.annotation.EzyRepository;

@EzyRepository("characterRepo")
public interface CharacterRepo extends EzyMongoRepository<Long, Character> {
}
