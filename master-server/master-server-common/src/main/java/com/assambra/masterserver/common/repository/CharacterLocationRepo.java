package com.assambra.masterserver.common.repository;

import com.assambra.masterserver.common.entity.CharacterLocation;
import com.tvd12.ezydata.mongodb.EzyMongoRepository;
import com.tvd12.ezyfox.database.annotation.EzyQuery;
import com.tvd12.ezyfox.database.annotation.EzyRepository;

import java.util.List;

@EzyRepository("characterLocationRepo")
public interface CharacterLocationRepo extends EzyMongoRepository<Long, CharacterLocation> {

    @EzyQuery("{$query: {characterId: {$in: ?0}}}")
    List<CharacterLocation> findByCharacterIds(List<Long> characterIds);
}
