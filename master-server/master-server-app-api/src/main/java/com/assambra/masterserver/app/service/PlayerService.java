package com.assambra.masterserver.app.service;


import com.assambra.masterserver.app.model.PlayerDespawnModel;
import com.assambra.masterserver.app.model.PlayerSpawnModel;
import com.assambra.masterserver.common.entity.Character;
import com.assambra.masterserver.common.entity.CharacterLocation;
import com.assambra.masterserver.common.masterserver.entity.UnityPlayer;
import com.assambra.masterserver.common.repository.CharacterRepo;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.gamebox.manager.PlayerManager;
import com.tvd12.gamebox.math.Vec3;
import lombok.AllArgsConstructor;
import lombok.Setter;

import java.util.List;


@Setter
@AllArgsConstructor
@EzySingleton("playerService")
public class PlayerService extends EzyLoggable {

    private final PlayerManager<UnityPlayer> globalPlayerManager;

    public void addPlayerToGlobalPlayerList(UnityPlayer player)
    {
        globalPlayerManager.addPlayer(player);
    }

    public void removePlayerFromGlobalPlayerList(UnityPlayer player)
    {
        globalPlayerManager.removePlayer(player);
    }

    private final CharacterRepo characterRepo;

    /**
     * This method found a UnityPlayer in the globalPlayerList by its "Character character.getName()" name
     * or "UnityPlayer player.getName())" name. It takes a {@code String} parameter name;
     *
     * @param name
     *        Character "character.getName()" or "UnityPlayer player.getName()"
     *
     * @return A UnityPlayer object if found else returns {@code NULL}
     */

    public UnityPlayer getPlayerByNameFromGlobalPlayerManager(String name)
    {
        return (UnityPlayer) globalPlayerManager.getPlayer(name);
    }

    /**
     * This method found a UnityPlayer in the globalPlayerList by its "EzyUser ezyuser.getName()" username
     * or "User user.getUsername" username. It takes a username as {@code String}
     *
     * @param username
     *        "EzyUser ezyuser.getName()" or "User user.getUsername" or Character character.getUsername()
     *
     * @return A UnityPlayer object if found else returns {@code NULL}
     */

    public UnityPlayer getPlayerByUsernameFromGlobalPlayerManager(String username) {
        List<UnityPlayer> players = globalPlayerManager.getPlayerList();
        return players.stream()
                .filter(p -> p.getUsername().equals(username))
                .findFirst()
                .orElse(null);
    }

    /**
     * This method found a UnityPlayer in the globalPlayerList by its Id
     * It takes a id as {@code Long}
     *
     * @param id
     *        Player database id
     *
     * @return A UnityPlayer object if found else returns {@code NULL}
     */
    public UnityPlayer getPlayerByIdFromGlobalPlayerManager(Long id)
    {
        List<UnityPlayer> players = globalPlayerManager.getPlayerList();
        return players.stream()
                .filter(p -> p.getId().equals(id))
                .findFirst()
                .orElse(null);
    }

    public PlayerSpawnModel getPlayerSpawnModel (Character character, CharacterLocation characterLocation)
    {
        double[] pos = characterLocation.getPosition();
        double[] rot = characterLocation.getRotation();

        return PlayerSpawnModel.builder()
                .id(character.getId())
                .username(character.getUsername())
                .name(character.getName())
                .sex(character.getSex())
                .race(character.getRace())
                .model(character.getModel())
                .position(
                        new Vec3(
                                (float)pos[0],
                                (float)pos[1],
                                (float)pos[2]
                        ).toArray()
                )
                .rotation(
                        new Vec3(
                                (float)rot[0],
                                (float)rot[1],
                                (float)rot[2]
                        ).toArray()
                )
                .build();
    }

    public PlayerSpawnModel getPlayerSpawnModel(UnityPlayer player, Vec3 position, Vec3 rotation)
    {
        Character character = characterRepo.findById(player.getId());

        return PlayerSpawnModel.builder()
                .id(player.getId())
                .username(player.getUsername())
                .name(player.getName())
                .sex(character.getSex())
                .race(character.getRace())
                .model(character.getModel())
                .position(position.toArray())
                .rotation(rotation.toArray())
                .build();
    }

    public PlayerDespawnModel getPlayerDespawnModel(Long id)
    {
        return PlayerDespawnModel.builder()
                .id(id)
                .build();
    }
}
