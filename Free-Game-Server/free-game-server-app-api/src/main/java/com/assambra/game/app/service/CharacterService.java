package com.assambra.game.app.service;

import com.assambra.game.app.entity.CharacterEntity;
import com.assambra.game.common.entity.Account;
import com.assambra.game.common.entity.Character;
import com.assambra.game.common.repository.AccountRepo;
import com.assambra.game.common.repository.CharacterRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.gamebox.math.Vec3;
import lombok.AllArgsConstructor;
import lombok.Setter;

import java.util.ArrayList;
import java.util.List;

@Setter
@AllArgsConstructor
@EzySingleton("characterService")
public class CharacterService extends EzyLoggable {

    private final CharacterRepo characterRepo;
    private final MaxIdService maxIdService;
    private final AccountRepo accountRepo;
    private final List<CharacterEntity> characterList;

    public List<Character> getAllCharacters(EzyUser user) {
        Account account = accountRepo.findByField("username", user.getName());

        return characterRepo.findListByField("accountId", account.getId());
    }

    public void createCharacter(EzyUser user, String name, String sex, String race, String model) {
        Account account = accountRepo.findByField("username", user.getName());
        Character character = new Character();

        double[] startPosition = new double[]{280.0,50.0,254.0};
        double[] startRotation = new double[]{0,-130,0};

        character.setId(maxIdService.incrementAndGet("character"));
        character.setAccountId(account.getId());
        character.setName(name);
        character.setSex(sex);
        character.setRace(race);
        character.setModel(model);
        character.setRoomId((long)1);
        character.setPosition(startPosition);
        character.setRotation(startRotation);
        characterRepo.save(character);

        List<Character> characters = getAllCharacters(user);

        for (Character c : characters) {
            logger.info(c.getName());
        }
    }

    public void addPlayerToCharacterList(EzyUser user, Character character) {
        CharacterEntity characterEntity = new CharacterEntity(
                user.getName(),
                character.getAccountId(),
                character.getRoomId(),
                character.getName(),
                character.getModel(),
                new Vec3((float)character.getPosition()[0], (float)character.getPosition()[1], (float)character.getPosition()[2]),
                new Vec3((float)character.getRotation()[0], (float)character.getRotation()[1], (float)character.getRotation()[2])
                );
        characterList.add(characterEntity);
    }

    public void SavePlayerPositionInCharacterEntity(String userName, Vec3 position, Vec3 rotation)
    {
        for(CharacterEntity characterEntity : characterList)
        {
            if(characterEntity.accountUsername.contains(userName))
            {
                characterEntity.position = position;
                characterEntity.rotation = rotation;
                break;
            }
        }
    }

    public void SavePlayerPositionInDatabase(EzyUser user)
    {
        for(CharacterEntity characterEntity : characterList)
        {
            if(characterEntity.accountUsername.contains(user.getName()))
            {
                Character character = characterRepo.findByField("accountId", characterEntity.accountId);
                double[] position = { characterEntity.position.x, characterEntity.position.y, characterEntity.position.z };
                double[] rotation = { characterEntity.rotation.x, characterEntity.rotation.y, characterEntity.rotation.z };
                character.setPosition(position);
                character.setRotation(rotation);
                character.setRoomId(characterEntity.roomId);

                characterRepo.save(character);

                break;
            }
        }
    }
}
