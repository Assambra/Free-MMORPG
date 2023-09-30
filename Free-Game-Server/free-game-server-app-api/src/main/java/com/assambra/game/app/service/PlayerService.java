package com.assambra.game.app.service;

import com.assambra.game.app.model.PlayerInputModel;
import com.assambra.game.app.utils.PlayerMovementUtils;
import com.tvd12.ezyfox.bean.annotation.EzyAutoBind;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.gamebox.entity.MMOPlayer;
import com.tvd12.gamebox.math.Vec3;
import lombok.Setter;

@Setter
@EzySingleton
public class PlayerService extends EzyLoggable {

    @EzyAutoBind
    RoomService roomService;
    @EzyAutoBind
    CharacterService characterService;

    public void handlePlayerInput(String playerName, PlayerInputModel model)
    {
        MMOPlayer player = roomService.getPlayer(playerName);
        synchronized (player){
            Vec3 currentRotation = player.getRotation();
            Vec3 nextRotation = PlayerMovementUtils.getNextRotation(currentRotation, model);
            player.setRotation(nextRotation);

            Vec3 currentPosition = player.getPosition();

            Vec3 nextPosition = PlayerMovementUtils.getNextPosition(currentPosition, nextRotation, model);
            roomService.setPlayerPosition(player, nextPosition);
            player.setClientTimeTick(model.getTime());

            characterService.SavePlayerPositionInCharacterEntity(playerName, nextPosition, currentRotation);

            //logger.info("Forward: {}", PlayerMovementUtils.GetForwardDirection(nextRotation));
            //logger.info("next position = {}", nextPosition);
        }
    }
}
