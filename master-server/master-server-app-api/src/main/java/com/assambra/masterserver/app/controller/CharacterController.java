package com.assambra.masterserver.app.controller;

import com.assambra.masterserver.app.constant.Commands;
import com.assambra.masterserver.app.converter.ModelToResponseConverter;
import com.assambra.masterserver.app.model.CharacterInfoListModel;
import com.assambra.masterserver.app.model.CreateCharacterModel;
import com.assambra.masterserver.app.request.CreateCharacterRequest;
import com.assambra.masterserver.app.service.CharacterService;
import com.assambra.masterserver.app.service.UserService;
import com.assambra.masterserver.common.entity.Character;
import com.assambra.masterserver.common.entity.User;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import lombok.AllArgsConstructor;

import java.util.List;

import static com.tvd12.ezyfox.io.EzyLists.newArrayList;

@AllArgsConstructor
@EzyRequestController
public class CharacterController extends EzyLoggable {

    private final EzyResponseFactory responseFactory;
    private final CharacterService characterService;
    private final ModelToResponseConverter modelToResponseConverter;
    private final UserService userService;

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

        String result = "";
        Long id = null;

        if(!characterService.characterExist(request.getName()))
        {
            User user = userService.getUserByUsername(ezyUser.getName());
            int maxAllowedCharacters = user.getMaxAllowedCharacters();
            List<Character> characters = characterService.getAllCharactersOfUser(ezyUser);

            if(characters.size() < maxAllowedCharacters)
            {
                logger.info("User: {}, successfully create new character: {}", ezyUser.getName(), request.getName());

                characterService.createCharacter(ezyUser, request);

                id = characterService.getIdByName(request.getName());
                result = "successfully";
            }
            else
            {
                result = "max_allowed_characters";
                id = 0L;
            }
        }
        else
        {
            logger.info("User: {}, tried to create new character: {}, but character name is already in use.", ezyUser.getName(), request.getName());

            result = "name_already_in_use";
            id = 0L;
        }

        CreateCharacterModel createCharacterModel = characterService.getCreateCharacterModel(id, result);
        modelToResponseConverter.toResponse(createCharacterModel)
                .command(Commands.CREATE_CHARACTER)
                .username(ezyUser.getName())
                .execute();
    }
}
