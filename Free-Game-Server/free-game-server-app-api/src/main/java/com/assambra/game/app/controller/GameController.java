package com.assambra.game.app.controller;

import com.assambra.game.app.converter.ModelToResponseConverter;
import com.assambra.game.app.model.CharacterSpawnModel;
import com.assambra.game.app.model.PlayModel;
import com.assambra.game.app.service.CharacterService;
import com.assambra.game.app.service.GameService;
import com.assambra.game.app.service.RoomService;
import com.assambra.game.common.entity.Account;
import com.assambra.game.common.entity.Character;
import com.assambra.game.common.repository.AccountRepo;
import com.assambra.game.common.repository.CharacterRepo;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.entity.EzyObject;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.assambra.game.app.constant.Commands;
import com.assambra.game.app.request.PlayRequest;

import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import com.tvd12.gamebox.entity.MMOPlayer;
import com.tvd12.gamebox.entity.MMORoom;
import com.tvd12.gamebox.entity.NormalRoom;
import lombok.AllArgsConstructor;

import java.util.List;

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
    public void play(EzyUser user, PlayRequest request)
    {
        Character character = characterRepo.findById(request.getCharacterId());
        Account account = accountRepo.findByField("username", user.getName());

        // Check if the given account and character exist
        if(account != null && character != null)
        {
            // Check if the account are the owner of the requested character
            if(character.getAccountId().equals(account.getId()))
            {
                logger.info("User {} request play with characterId {}", user.getName(), request.getCharacterId());

                MMOPlayer player = roomService.addUserToRoom(user, character.getRoomId());

                characterService.addPlayerToCharacterList(user, character);

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
                List<String> usernames = roomService.getRoomPlayerNames(room);
                usernames.remove(user.getName());

                responseFactory.newObjectResponse()
                        .command(Commands.CHARACTER_SPAWNED)
                        .data(
                                modelToResponseConverter.toResponse(spawnModel)
                        )
                        .usernames(usernames)
                        .execute();
            }
            else
                logger.info("User {} try to cheat", user);
        }
        else
            logger.info("User {} try to cheat", user);
    }
}
