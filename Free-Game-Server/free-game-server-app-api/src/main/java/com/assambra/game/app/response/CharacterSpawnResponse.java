package com.assambra.game.app.response;

import com.tvd12.ezyfox.binding.annotation.EzyObjectBinding;
import com.tvd12.ezyfox.entity.EzyArray;
import lombok.Builder;
import lombok.Getter;

@Getter
@Builder
@EzyObjectBinding
public class CharacterSpawnResponse {
    private String accountUsername;
    private Long roomId;
    private Boolean isLocalPlayer;
    private String characterName;
    private String characterModel;
    private EzyArray position;
    private EzyArray rotation;
}
