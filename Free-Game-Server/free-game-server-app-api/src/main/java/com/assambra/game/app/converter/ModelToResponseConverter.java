package com.assambra.game.app.converter;

import com.assambra.game.app.model.CharacterSpawnModel;
import com.assambra.game.app.response.CharacterSpawnResponse;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfoxserver.support.command.EzyObjectResponse;
import lombok.AllArgsConstructor;

import java.util.List;

@EzySingleton
@AllArgsConstructor
public class ModelToResponseConverter {

    public CharacterSpawnResponse toResponse(CharacterSpawnModel model)
    {
        return CharacterSpawnResponse.builder()
                .accountUsername(model.getAccountUsername())
                .roomId(model.getRoomId())
                .isLocalPlayer(model.getIsLocalPlayer())
                .characterName(model.getCharacterName())
                .characterModel(model.getCharacterModel())
                .position(model.getPosition())
                .rotation(model.getRotation())
                .build();
    }
}
