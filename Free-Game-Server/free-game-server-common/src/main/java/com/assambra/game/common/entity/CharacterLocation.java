package com.assambra.game.common.entity;

import com.tvd12.ezyfox.annotation.EzyId;
import com.tvd12.ezyfox.database.annotation.EzyCollection;
import lombok.Data;

@Data
@EzyCollection
public class CharacterLocation {
    @EzyId
    Long id;
    Long characterId;
    String room;
    double[] position;
    double[] rotation;
}
