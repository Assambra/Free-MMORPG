package com.assambra.game.app.model;

import com.tvd12.ezyfox.entity.EzyArray;
import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
public class CharacterSpawnModel {
    private String accountUsername;
    private Long roomId;
    private Boolean isLocalPlayer;
    private String characterName;
    private String characterModel;
    private EzyArray position;
    private EzyArray rotation;
}
