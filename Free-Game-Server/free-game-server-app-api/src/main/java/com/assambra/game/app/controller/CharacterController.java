package com.assambra.game.app.controller;

import com.assambra.game.app.constant.Commands;
import com.assambra.game.app.request.CreateCharacterRequest;
import com.assambra.game.app.service.CharacterService;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import lombok.AllArgsConstructor;

@AllArgsConstructor
@EzyRequestController
public class CharacterController extends EzyLoggable {

    private final EzyResponseFactory responseFactory;

    private final CharacterService characterService;

    @EzyDoHandle(Commands.CHARACTER_LIST)
    public void characterList(EzyUser user)
    {
        logger.info("user {} request character list", user);
    }

    @EzyDoHandle(Commands.CREATE_CHARACTER)
    public void createCharacter(EzyUser user, CreateCharacterRequest request)
    {
        logger.info("user {} request create character", user);
    }
}
