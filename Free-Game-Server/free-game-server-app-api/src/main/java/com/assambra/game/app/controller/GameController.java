package com.assambra.game.app.controller;

import com.assambra.game.app.constant.Commands;
import com.assambra.game.app.converter.ModelToResponseConverter;
import com.assambra.game.app.model.CharacterSpawnModel;
import com.assambra.game.app.model.PlayModel;
import com.assambra.game.app.request.PlayRequest;
import com.assambra.game.app.service.CharacterService;
import com.assambra.game.app.service.GameService;
import com.assambra.game.app.service.RoomService;
import com.assambra.game.common.entity.Account;
import com.assambra.game.common.entity.Character;
import com.assambra.game.common.repository.AccountRepo;
import com.assambra.game.common.repository.CharacterRepo;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.core.exception.EzyBadRequestException;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import com.tvd12.gamebox.entity.MMOPlayer;
import com.tvd12.gamebox.entity.NormalRoom;
import lombok.AllArgsConstructor;

import static com.assambra.game.app.constant.Errors.TRY_TO_CHEAT;
import static com.tvd12.ezyfox.io.EzyLists.newArrayList;

@AllArgsConstructor
@EzyRequestController
public class GameController extends EzyLoggable {

    private final GameService gameService;
    private final CharacterRepo characterRepo;
    private final RoomService roomService;
    private final CharacterService characterService;
    private final EzyResponseFactory responseFactory;
    private final ModelToResponseConverter modelToResponseConverter;
    private final AccountRepo accountRepo;

    @EzyDoHandle(Commands.PLAY)
    public void play(EzyUser user, PlayRequest request) {
        Character character = characterRepo.findById(request.getCharacterId());

        // Check if the given character exist
        if (character == null) {
            throw new EzyBadRequestException(
                TRY_TO_CHEAT,
                "you_are_trying_to_cheat"
            );
        }
        String username = user.getName();
        Account account = accountRepo.findByField("username", username);

        // Check if the given account exist
        // Check if the account are the owner of the requested character
        if (account == null || !character.getAccountId().equals(account.getId())) {
            throw new EzyBadRequestException(
                TRY_TO_CHEAT,
                "you_are_trying_to_cheat"
            );
        }
        logger.info("User {} request play with characterId {}", username, request.getCharacterId());
        MMOPlayer currentPlayer = roomService.getPlayer(username);
        if (currentPlayer != null) {
            PlayModel play = gameService.play(user);

            // TODO put more reconnection data here
            responseFactory.newArrayResponse()
                .command(Commands.RECONNECT)
                .data(
                    newArrayList(
                        play.getCharacterSpawns(),
                        modelToResponseConverter::toResponse
                    )
                )
                .user(user)
                .execute();
        } else {
            characterService.addPlayerToCharacterList(user, character);
            MMOPlayer player = roomService.addUserToRoom(user, character.getRoomId());
            PlayModel play = gameService.play(user);
            responseFactory.newArrayResponse()
                .command(Commands.PLAY)
                .data(
                    newArrayList(
                        play.getCharacterSpawns(),
                        modelToResponseConverter::toResponse
                    )
                )
                .user(user)
                .execute();

            CharacterSpawnModel spawnModel = gameService.characterSpawned(user);
            NormalRoom room = roomService.getCurrentRoom(player);
            responseFactory.newObjectResponse()
                .command(Commands.CHARACTER_SPAWNED)
                .data(
                    modelToResponseConverter.toResponse(spawnModel)
                )
                .usernames(roomService.getRoomPlayerNames(room))
                .username(username, true)
                .execute();
        }
    }
}
