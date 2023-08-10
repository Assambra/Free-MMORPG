package com.assambra.game.app.controller;

import com.assambra.game.app.constant.Commands;
import com.assambra.game.app.model.CharacterListModel;
import com.assambra.game.app.request.CreateCharacterRequest;
import com.assambra.game.app.service.CharacterService;
import com.assambra.game.common.entity.Character;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import lombok.AllArgsConstructor;

import java.util.List;

@AllArgsConstructor
@EzyRequestController
public class CharacterController extends EzyLoggable {

    private final EzyResponseFactory responseFactory;
    private final CharacterService characterService;

    @EzyDoHandle(Commands.CHARACTER_LIST)
    public void characterList(EzyUser user)
    {
        logger.info("user {} request character list", user.getName());

        responseFactory.newArrayResponse()
                .command(Commands.CHARACTER_LIST)
                .data(
                        convert(characterService.getAllCharacters(user))
                )
                .user(user)
                .execute();
    }

    @EzyDoHandle(Commands.CREATE_CHARACTER)
    public void createCharacter(EzyUser user, CreateCharacterRequest request)
    {
        logger.info("user {} request create character", user);

        characterService.createCharacter(user, request.getName(), request.getSex(), request.getRace(), request.getModel());
    }

    private CharacterListModel[] convert(List<Character> characters)
    {
        CharacterListModel[] chars = new CharacterListModel[characters.size()];

        int i=0;
        for (Character character : characters) {
            CharacterListModel characterListModel = CharacterListModel.builder()
                    .id(character.getId())
                    .accountId(character.getAccountId())
                    .name(character.getName())
                    .sex(character.getSex())
                    .race(character.getRace())
                    .model(character.getModel())
                    .build();

            chars[i] = characterListModel;
            i++;
        }
        return chars;
    }
}
