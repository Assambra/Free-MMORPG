package com.assambra.game.common.repository;

import com.assambra.game.common.entity.Character;
import com.tvd12.ezydata.mongodb.EzyMongoRepository;
import com.tvd12.ezyfox.database.annotation.EzyRepository;

@EzyRepository("characterRepo")
public interface CharacterRepo extends EzyMongoRepository<Long, Character> {

}
