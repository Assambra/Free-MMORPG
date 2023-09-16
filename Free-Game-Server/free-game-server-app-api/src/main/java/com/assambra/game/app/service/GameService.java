package com.assambra.game.app.service;

import com.assambra.game.app.entity.CharacterEntity;
import com.assambra.game.app.model.CharacterSpawnModel;
import com.assambra.game.app.model.PlayModel;
import com.assambra.game.common.entity.Character;
import com.tvd12.ezyfox.bean.annotation.EzyAutoBind;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.gamebox.entity.MMOPlayer;
import com.tvd12.gamebox.entity.MMORoom;
import com.tvd12.gamebox.entity.Player;
import com.tvd12.gamebox.manager.PlayerManager;
import com.tvd12.gamebox.math.Vec3;
import lombok.Setter;

import java.util.ArrayList;
import java.util.List;
import java.util.SortedMap;
import java.util.stream.Collectors;


@Setter
@EzySingleton
public class GameService extends EzyLoggable {

    @EzyAutoBind
    RoomService roomService;

    @EzyAutoBind
    private PlayerManager<Player> globalPlayerManager;

    @EzyAutoBind
    List<CharacterEntity> characterList;

    public PlayModel play(EzyUser user) {
        MMORoom currentRoom = (MMORoom) roomService.getCurrentRoom(user.getName());

        List<String> userNames = roomService.getRoomPlayerNames(currentRoom);
        List<CharacterEntity> characters = new ArrayList<>();

        for(String userName : userNames)
        {
            for(CharacterEntity entity : characterList)
            {
                if(entity.accountUsername.equals(userName))
                    characters.add(entity);
            }
        }

        List<CharacterSpawnModel> characterSpawns = spawnCharacters(user, characters);
        return PlayModel.builder()
                .userNames(userNames)
                .characterSpawns(characterSpawns)
                .build();
    }

    public List<CharacterSpawnModel> spawnCharacters(EzyUser user, List<CharacterEntity> characters)
    {
        List<CharacterSpawnModel> answer = characters.stream().map(
                character -> CharacterSpawnModel.builder()
                    .accountUsername(character.accountUsername)
                    .roomId(character.roomId)
                    .isLocalPlayer(user.getName().equals(character.accountUsername))
                    .characterName(character.characterName)
                    .characterModel(character.characterModel)
                    .position(character.position.toArray())
                    .rotation(character.rotation.toArray())
                    .build()
        ).collect(Collectors.toList());

        answer.forEach(characterSpawnData -> {
            MMOPlayer player = (MMOPlayer) globalPlayerManager.getPlayer(characterSpawnData.getAccountUsername());
            synchronized (player) {
                Vec3 initialPosition = new Vec3(
                        characterSpawnData.getPosition().get(0),
                        characterSpawnData.getPosition().get(1),
                        characterSpawnData.getPosition().get(2)
                );
                Vec3 initialRotation = new Vec3(
                        characterSpawnData.getRotation().get(0),
                        characterSpawnData.getRotation().get(1),
                        characterSpawnData.getRotation().get(2)
                );
                roomService.setPlayerPosition(player, initialPosition);
                player.setRotation(initialRotation);
                player.setClientTimeTick(0);

                /*
                SortedMap<Integer, Vec3> playerPositionHistory
                        = positionHistoryByPlayerName.get(
                        player.getName());
                playerPositionHistory.put(0, initialPosition);
                */
            }
        });

        return answer;
    }

    public CharacterSpawnModel characterSpawned(EzyUser user) {

        CharacterEntity myCharacter = null;

        for(CharacterEntity entity : characterList)
        {
            if(entity.accountUsername.equals(user.getName()))
                myCharacter = entity;
        }

        return CharacterSpawnModel.builder()
                .accountUsername(user.getName())
                .roomId(myCharacter.roomId)
                .isLocalPlayer(false)
                .characterName(myCharacter.characterName)
                .characterModel(myCharacter.characterModel)
                .position(myCharacter.position.toArray())
                .rotation(myCharacter.rotation.toArray())
                .build();
    }
}
