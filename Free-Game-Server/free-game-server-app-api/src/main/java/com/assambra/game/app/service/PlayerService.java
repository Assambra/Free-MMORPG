package com.assambra.game.app.service;

import com.assambra.game.app.model.PlayerInputModel;
import com.assambra.game.app.terrain.Terrain;
import com.assambra.game.app.utils.PlayerMovementUtils;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.core.exception.EzyBadRequestException;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.gamebox.entity.MMOPlayer;
import com.tvd12.gamebox.math.Vec3;
import lombok.AllArgsConstructor;

import static com.assambra.game.app.constant.Errors.NOT_A_PLAYER;

@EzySingleton
@AllArgsConstructor
public class PlayerService extends EzyLoggable {

    private final RoomService roomService;

    private final CharacterService characterService;

    private final Terrain worldTerrain;

    public void handlePlayerInput(String playerName, PlayerInputModel model) {
        MMOPlayer player = roomService.getPlayer(playerName);
        // if user hasn't joined any game, reject the request
        if (player == null) {
            throw new EzyBadRequestException(
                NOT_A_PLAYER,
                "your_are_not_a_player"
            );
        }
        synchronized (player){
            Vec3 currentRotation = player.getRotation();
            Vec3 nextRotation = PlayerMovementUtils.getNextRotation(currentRotation, model);
            player.setRotation(nextRotation);

            Vec3 currentPosition = player.getPosition();

            Vec3 nextPosition = PlayerMovementUtils.getNextPosition(currentPosition, nextRotation, model);
            
            nextPosition = new Vec3(nextPosition.x, worldTerrain.getHeightValue(player.getPosition().x, player.getPosition().z), nextPosition.z );
            //logger.info("Player: {} current y position: {}", player.getName(), worldTerrain.getHeightValue(player.getPosition().x, player.getPosition().z));

            roomService.setPlayerPosition(player, nextPosition);
            player.setClientTimeTick(model.getTime());

            characterService.SavePlayerPositionInCharacterEntity(playerName, nextPosition, currentRotation);
        }
    }
}
