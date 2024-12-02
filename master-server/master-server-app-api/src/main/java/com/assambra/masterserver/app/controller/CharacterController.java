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
    public void characterList(EzyUser ezyUser)
    {
        logger.info("Receive: Commands.CHARACTER_LIST from user: {}", ezyUser.getName());

        CharacterInfoListModel characterInfoListModel = characterService.getCharacterInfoListModel(ezyUser);

        responseFactory.newArrayResponse()
                .command(Commands.CHARACTER_LIST)
                .data(
                        newArrayList(
                                characterInfoListModel.getCharacters(),
                                modelToResponseConverter::toResponse
                        )
                )
                .user(ezyUser)
                .execute();
    }

    @EzyDoHandle(Commands.CREATE_CHARACTER)
    public void createCharacter(EzyUser ezyUser, CreateCharacterRequest request)
    {
        logger.info("Receive: Commands.CREATE_CHARACTER from user: {}", ezyUser.getName());

        if(!characterService.characterExist(request.getName()))
        {
            logger.info("User: {}, successfully create new character: {}", ezyUser.getName(), request.getName());
            characterService.createCharacter(ezyUser, request);
        }
        else
        {
            logger.info("User: {}, tried to create new character: {}, but it already exists.", ezyUser.getName(), request.getName());
        }
    }
}
