package com.assambra.game.app.config;

import com.assambra.game.common.masterserver.entity.UnityPlayer;
import com.assambra.game.common.masterserver.entity.UnityRoom;
import com.assambra.game.common.masterserver.manager.SynchronizedUnityPlayerManager;
import com.assambra.game.common.masterserver.manager.SynchronizedUnityRoomManager;
import com.tvd12.ezyfox.bean.annotation.EzyConfigurationBefore;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.gamebox.manager.PlayerManager;
import com.tvd12.gamebox.manager.RoomManager;

import java.util.ArrayList;
import java.util.List;

@EzyConfigurationBefore(priority = 1)
public class GlobalManagerConfig extends EzyLoggable {

    @EzySingleton("globalRoomManager")
    public RoomManager<UnityRoom> globalRoomManager() {
        return new SynchronizedUnityRoomManager<>();
    }

    @EzySingleton("globalPlayerManager")
    public PlayerManager<UnityPlayer> globalPlayerManager() {
        return new SynchronizedUnityPlayerManager<>();
    }

    @EzySingleton("globalServerEzyUsers")
    public List<EzyUser> globalServerEzyUsers = new ArrayList<>();
}
