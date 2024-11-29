package com.assambra.game.app.converter;

import com.assambra.game.app.model.CharacterInfoModel;
import com.assambra.game.app.model.PlayerDespawnModel;
import com.assambra.game.app.model.PlayerSpawnModel;
import com.assambra.game.app.response.CharacterInfoResponse;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfoxserver.support.command.EzyObjectResponse;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import lombok.AllArgsConstructor;

import java.util.List;

@EzySingleton
@AllArgsConstructor
public class ModelToResponseConverter {

    private final EzyResponseFactory responseFactory;

    public CharacterInfoResponse toResponse(CharacterInfoModel model){
        return CharacterInfoResponse.builder()
                .id(model.getId())
                .name(model.getName())
                .room(model.getRoom())
                .build();
    }

    public EzyObjectResponse toResponse(PlayerSpawnModel model)
    {
        return responseFactory.newObjectResponse()
                .param("id", model.getId())
                .param("name", model.getName())
                .param("username", model.getUsername())
                .param("position", model.getPosition())
                .param("rotation", model.getRotation());
    }

    public EzyObjectResponse toResponse(PlayerDespawnModel model)
    {
        return responseFactory.newObjectResponse()
                .param("id", model.getId());
    }
}
