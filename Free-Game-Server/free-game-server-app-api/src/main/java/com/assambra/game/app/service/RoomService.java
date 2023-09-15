package com.assambra.game.app.service;

import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.gamebox.entity.*;
import com.tvd12.gamebox.manager.PlayerManager;
import com.tvd12.gamebox.manager.RoomManager;
import com.tvd12.gamebox.math.Vec3;
import lombok.AllArgsConstructor;

import java.util.List;

@EzySingleton
@AllArgsConstructor
@SuppressWarnings({"unchecked"})
public class RoomService extends EzyLoggable
{
    private PlayerManager<Player> globalPlayerManager;
    private RoomManager<NormalRoom> globalRoomManager;

    public MMOPlayer addUserToRoom(EzyUser user, Long roomId)
    {
        MMOPlayer player = new MMOPlayer(user.getName());
        globalPlayerManager.addPlayer(player);

        MMORoom room = (MMORoom)globalRoomManager.getRoom(roomId);

        room.addPlayer(player);
        player.setCurrentRoomId(roomId);

        return player;
    }

    public NormalRoom getCurrentRoom(String playerName) {
        Player player = globalPlayerManager.getPlayer(playerName);
        return getCurrentRoom(player);
    }

    public NormalRoom getCurrentRoom(Player player) {
        long currentRoomId = player.getCurrentRoomId();
        return globalRoomManager.getRoom(currentRoomId);
    }

    public List<String> getRoomPlayerNames(NormalRoom room) {
        synchronized (room) {
            return room.getPlayerManager().getPlayerNames();
        }
    }

    public void setPlayerPosition(MMOPlayer player, Vec3 nextPosition) {
        MMOGridRoom currentRoom = (MMOGridRoom) getCurrentRoom(player);
        currentRoom.setPlayerPosition(player, nextPosition);
    }
}
