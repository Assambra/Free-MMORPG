package com.assambra.game.common.entity;

import com.tvd12.ezyfox.annotation.EzyId;
import com.tvd12.ezyfox.database.annotation.EzyCollection;
import lombok.Data;

@Data
@EzyCollection
public class Character {
    @EzyId
    Long id;
    Long userId;
    String username;
    String name;
    String sex;
    String race;
    String model;
}
