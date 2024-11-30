package com.assambra.game.plugin.service;

import com.assambra.game.common.masterserver.entity.UnityRoom;
import com.tvd12.ezyfox.bean.EzyBeanContext;
import com.tvd12.ezyfox.bean.annotation.EzyAutoBind;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfoxserver.context.EzyPluginContext;
import com.tvd12.gamebox.manager.RoomManager;
import lombok.AllArgsConstructor;
import lombok.Setter;

import java.util.ArrayList;
import java.util.List;

@Setter
@AllArgsConstructor
@EzySingleton("serverServicePlugin")
public class ServerService {

    @EzyAutoBind
    private EzyPluginContext pluginContext;

    public List<UnityRoom> getServers()
    {
        RoomManager<UnityRoom> globalRoomManager = pluginContext
                .getParent()
                .getAppContext("free-game-server")
                .getProperty(EzyBeanContext.class)
                .getSingleton("globalRoomManager", RoomManager.class);

        return globalRoomManager.getRoomList();
    }

    public List<String> getServerUsernames() {

        List<String> serverUsernames = new ArrayList<>();

        for(UnityRoom room : getServers())
        {
            serverUsernames.add(room.getName());
        }

        return serverUsernames;
    }
}
