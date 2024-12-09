package com.assambra.masterserver.app.controller;


import com.assambra.masterserver.app.constant.Commands;
import com.assambra.masterserver.app.converter.RequestToModelConverter;
import com.assambra.masterserver.app.model.PlayerSpawnModel;
import com.assambra.masterserver.app.model.request.RequestPlayModel;
import com.assambra.masterserver.app.request.PlayRequest;
import com.assambra.masterserver.app.service.CharacterService;
import com.assambra.masterserver.app.service.PlayerService;
import com.assambra.masterserver.app.service.RoomService;
import com.assambra.masterserver.app.service.AccountService;
import com.assambra.masterserver.common.entity.Account;
import com.assambra.masterserver.common.entity.Character;
import com.assambra.masterserver.common.entity.CharacterLocation;
import com.assambra.masterserver.common.masterserver.entity.UnityPlayer;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.core.exception.EzyBadRequestException;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import lombok.AllArgsConstructor;

import static com.assambra.masterserver.app.constant.Errors.TRY_TO_CHEAT;

@AllArgsConstructor
@EzyRequestController
public class GameController extends EzyLoggable {

    private final AccountService accountService;
    private final CharacterService characterService;
    private final PlayerService playerService;
    private final RoomService roomService;
    private final RequestToModelConverter requestToModelConverter;
    private final EzyResponseFactory responseFactory;

    @EzyDoHandle(Commands.CHECK)
    public void check(EzyUser ezyUser)
    {
        Account account = accountService.getAccountByUsername(ezyUser.getName());
        String result;

        if(account.getActivated())
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

        RequestPlayModel requestPlayModel = requestToModelConverter.toModel(request);

        Character character = characterService.getCharacter(requestPlayModel.getPlayerId());

        // Check if the given character exist
        if (character == null) {
            throw new EzyBadRequestException(
                TRY_TO_CHEAT,
                "you_are_trying_to_cheat"
            );
        }

        Account account = accountService.getAccountByUsername(ezyUser.getName());

        // Check if the given account exist
        // Check if the account are the owner of the requested character
        if (account == null || !character.getAccountId().equals(account.getId())) {
            throw new EzyBadRequestException(
                TRY_TO_CHEAT,
                "you_are_trying_to_cheat"
            );
        }

        CharacterLocation characterLocation = characterService.getCharacterLocation(character.getId());

        UnityPlayer player = UnityPlayer.builder()
                .name(character.getName())
                .id(character.getId())
                .username(ezyUser.getName())
                .build();

        playerService.addPlayerToGlobalPlayerList(player);

        PlayerSpawnModel serverPlayerSpawnModel = playerService.getPlayerSpawnModel(character, characterLocation);

        roomService.addPlayerToRoom(player, characterLocation.getRoom(), serverPlayerSpawnModel);
    }
}
