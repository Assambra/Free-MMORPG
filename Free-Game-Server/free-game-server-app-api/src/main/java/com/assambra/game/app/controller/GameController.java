package com.assambra.game.app.controller;

import com.assambra.game.app.constant.Commands;
import com.assambra.game.app.model.PlayerSpawnModel;
import com.assambra.game.app.request.PlayRequest;
import com.assambra.game.app.service.*;
import com.assambra.game.common.entity.CharacterLocation;
import com.assambra.game.common.entity.User;
import com.assambra.game.common.entity.Character;
import com.assambra.game.common.masterserver.entity.UnityPlayer;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.core.exception.EzyBadRequestException;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import lombok.AllArgsConstructor;

import static com.assambra.game.app.constant.Errors.TRY_TO_CHEAT;

@AllArgsConstructor
@EzyRequestController
public class GameController extends EzyLoggable {

    private final UserService userService;
    private final CharacterService characterService;
    private final PlayerService playerService;
    private final RoomService roomService;
    private final EzyResponseFactory responseFactory;

    @EzyDoHandle(Commands.CHECK)
    public void check(EzyUser ezyUser)
    {
        User user = userService.GetUserByUsername(ezyUser.getName());
        String result;

        if(user.getActivated())
            result = "ok";
        else
            result = "need_activation";

        responseFactory.newObjectResponse()
                .command(Commands.CHECK)
                .param("result", result)
                .user(ezyUser)
                .execute();
    }

    @EzyDoHandle(Commands.PLAY)
    public void play(EzyUser ezyUser, PlayRequest request) {
        Character character = characterService.getCharacter(request.getId());

        // Check if the given character exist
        if (character == null) {
            throw new EzyBadRequestException(
                TRY_TO_CHEAT,
                "you_are_trying_to_cheat"
            );
        }

        User user = userService.GetUserByUsername(ezyUser.getName());

        // Check if the given account exist
        // Check if the account are the owner of the requested character
        if (user == null || !character.getUserId().equals(user.getId())) {
            throw new EzyBadRequestException(
                TRY_TO_CHEAT,
                "you_are_trying_to_cheat"
            );
        }

        CharacterLocation characterLocation = characterService.getCharacterLocation(character.getId());

        UnityPlayer player = new UnityPlayer(character.getName());
        player.setUsername(ezyUser.getName());
        player.setId(character.getId());
        playerService.addPlayerToGlobalPlayerList(player);

        PlayerSpawnModel serverPlayerSpawnModel = playerService.getPlayerSpawnModel(character, characterLocation);

        roomService.addPlayerToRoom(player, characterLocation.getRoom(), serverPlayerSpawnModel);
    }
}
