package com.assambra.game.app.service;

import com.assambra.game.common.entity.Account;
import com.assambra.game.common.entity.Character;
import com.assambra.game.common.repository.AccountRepo;
import com.assambra.game.common.repository.CharacterRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
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

    public List<Character> getAllCharacters(EzyUser user) {
        Account account = accountRepo.findByField("username", user);

        return characterRepo.findListByField("accountId", account.getId());
    }

    public void createCharacter(EzyUser user, String name, String sex, String model) {
        Account account = accountRepo.findByField("username", user.getName());
        Character character = new Character();

        character.setId(maxIdService.incrementAndGet("character"));
        character.setAccountId(account.getId());
        character.setName(name);
        character.setSex(sex);
        character.setModel(model);

        characterRepo.save(character);

        List<Character> characters = getAllCharacters(user);

        for (Character c : characters) {
            logger.info(c.getName());

        }
    }
}
