package com.assambra.game.common.entity;

import com.tvd12.ezyfox.annotation.EzyId;
import com.tvd12.ezyfox.binding.annotation.EzyArrayBinding;
import com.tvd12.ezyfox.database.annotation.EzyCollection;
import lombok.Data;

@Data
@EzyCollection
public class Character {
    @EzyId
    Long id;
    Long accountId;
    String name;
    String sex;
    String race;
    String model;
    Long roomId;
    double[] position;
    double[] rotation;
}
