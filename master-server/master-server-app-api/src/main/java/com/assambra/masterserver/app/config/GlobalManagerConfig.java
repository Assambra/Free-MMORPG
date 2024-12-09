package com.assambra.masterserver.app.config;

import com.assambra.masterserver.common.masterserver.entity.UnityPlayer;
import com.assambra.masterserver.common.masterserver.entity.UnityRoom;
import com.assambra.masterserver.common.masterserver.manager.UnityPlayerManager;
import com.assambra.masterserver.common.masterserver.manager.UnityRoomManager;
import com.tvd12.ezyfox.bean.annotation.EzyConfigurationBefore;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;


import java.util.ArrayList;
import java.util.List;

@EzyConfigurationBefore(priority = 1)
public class GlobalManagerConfig extends EzyLoggable {

    @EzySingleton("globalRoomManager")
    public UnityRoomManager<UnityRoom> globalRoomManager() {
        return new UnityRoomManager<>();
    }

    @EzySingleton("globalPlayerManager")
    public UnityPlayerManager<UnityPlayer> globalPlayerManager() {
        return new UnityPlayerManager<>();
    }

    @EzySingleton("globalServerEzyUsers")
    public List<EzyUser> globalServerEzyUsers = new ArrayList<>();
}
