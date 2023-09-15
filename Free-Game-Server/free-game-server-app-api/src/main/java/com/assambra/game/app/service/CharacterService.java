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

        double[] startPosition = new double[]{0,0,0};
        double[] startRotation = new double[]{0,0,0};

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
                character.getRoomId(),
                character.getName(),
                character.getModel(),
                new Vec3((float)character.getPosition()[0], (float)character.getPosition()[1], (float)character.getPosition()[2]),
                new Vec3((float)character.getRotation()[0], (float)character.getRotation()[1], (float)character.getRotation()[2])
                );
        characterList.add(characterEntity);
    }
}
