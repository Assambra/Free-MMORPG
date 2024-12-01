package com.assambra.masterserver.app.controller;


import com.assambra.masterserver.app.constant.Commands;
import com.assambra.masterserver.app.converter.ModelToResponseConverter;
import com.assambra.masterserver.app.model.CharacterInfoListModel;
import com.assambra.masterserver.app.request.CreateCharacterRequest;
import com.assambra.masterserver.app.service.CharacterService;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import lombok.AllArgsConstructor;

import static com.tvd12.ezyfox.io.EzyLists.newArrayList;

@AllArgsConstructor
@EzyRequestController
public class CharacterController extends EzyLoggable {

    private final EzyResponseFactory responseFactory;
    private final CharacterService characterService;
    private final ModelToResponseConverter modelToResponseConverter;

    @EzyDoHandle(Commands.CHARACTER_LIST)
    public void characterList(EzyUser ezyuser)
    {
        logger.info("Receive: Commands.CHARACTER_LIST from user: {}", ezyuser.getName());

        CharacterInfoListModel characterInfoListModel = characterService.getCharacterInfoListModel(ezyuser);

        responseFactory.newArrayResponse()
                .command(Commands.CHARACTER_LIST)
                .data(
                        newArrayList(
                                characterInfoListModel.getCharacters(),
                                modelToResponseConverter::toResponse
                        )
                )
                .user(ezyuser)
                .execute();
    }

    @EzyDoHandle(Commands.CREATE_CHARACTER)
    public void createCharacter(EzyUser ezyuser, CreateCharacterRequest request)
    {
        logger.info("Receive: Commands.CREATE_CHARACTER from user: {}", ezyuser.getName());

        if(!characterService.characterExist(request.getName()))
        {
            logger.info("User: {}, successfully create new character: {}", ezyuser.getName(), request.getName());
            characterService.createCharacter(ezyuser, request);
        }
        else
        {
            logger.info("User: {}, tried to create new character: {}, but it already exists.", ezyuser.getName(), request.getName());
        }
    }
}
