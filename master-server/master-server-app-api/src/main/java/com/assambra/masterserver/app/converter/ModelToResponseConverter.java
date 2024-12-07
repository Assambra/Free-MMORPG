package com.assambra.masterserver.app.converter;

import com.assambra.masterserver.app.model.CharacterInfoModel;
import com.assambra.masterserver.app.model.CreateCharacterModel;
import com.assambra.masterserver.app.model.PlayerDespawnModel;
import com.assambra.masterserver.app.model.PlayerSpawnModel;
import com.assambra.masterserver.app.response.CharacterInfoResponse;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfoxserver.support.command.EzyObjectResponse;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import lombok.AllArgsConstructor;

@EzySingleton
@AllArgsConstructor
public class ModelToResponseConverter {

    private final EzyResponseFactory responseFactory;

    public CharacterInfoResponse toResponse(CharacterInfoModel model){
        return CharacterInfoResponse.builder()
                .id(model.getId())
                .name(model.getName())
                .sex(model.getSex())
                .race(model.getRace())
                .model(model.getModel())
                .room(model.getRoom())
                .build();
    }

    public EzyObjectResponse toResponse(PlayerSpawnModel model)
    {
        return responseFactory.newObjectResponse()
                .param("id", model.getId())
                .param("name", model.getName())
                .param("username", model.getUsername())
                .param("sex", model.getSex())
                .param("race", model.getRace())
                .param("model", model.getModel())
                .param("position", model.getPosition())
                .param("rotation", model.getRotation());
    }

    public EzyObjectResponse toResponse(PlayerDespawnModel model)
    {
        return responseFactory.newObjectResponse()
                .param("id", model.getId());
    }

    public EzyObjectResponse toResponse(CreateCharacterModel model)
    {
        return responseFactory.newObjectResponse()
                .param("id", model.getId())
                .param("result", model.getResult());
    }
}
