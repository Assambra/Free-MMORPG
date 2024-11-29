package com.assambra.game.app.model;

import com.tvd12.ezyfox.entity.EzyArray;
import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class PlayerSpawnModel {
    private Long id;
    private String username;
    private String name;
    private String sex;
    private String race;
    private String model;
    private EzyArray position;
    private EzyArray rotation;
}
