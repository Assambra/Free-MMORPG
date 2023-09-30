package com.assambra.game.app.controller;

import com.assambra.game.app.constant.Commands;
import com.assambra.game.app.converter.RequestToModelConverter;
import com.assambra.game.app.request.PlayerInputRequest;
import com.assambra.game.app.service.PlayerService;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import lombok.AllArgsConstructor;

@AllArgsConstructor
@EzyRequestController
public class PlayerController extends EzyLoggable {

    private final RequestToModelConverter requestToModelConverter;
    private final PlayerService playerService;

    @EzyDoHandle(Commands.PLAYER_INPUT)
    public void playerInput(EzyUser user, PlayerInputRequest request) {
        //logger.info("user {} send input data {}", user.getName(), request);

        playerService.handlePlayerInput(
                user.getName(),
                requestToModelConverter.toModel(request)
        );
    }
}
