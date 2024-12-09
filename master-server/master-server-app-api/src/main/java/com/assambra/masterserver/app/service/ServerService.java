package com.assambra.masterserver.app.service;


import com.assambra.masterserver.common.masterserver.constant.UnityRoomStatus;
import com.assambra.masterserver.common.masterserver.entity.UnityRoom;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;

import com.tvd12.gamebox.manager.RoomManager;
import lombok.AllArgsConstructor;
import lombok.Setter;

import java.util.List;

@Setter
@AllArgsConstructor
@EzySingleton("serverServiceApp")
public class ServerService extends EzyLoggable {

    private final RoomManager<UnityRoom> globalRoomManager;
    private final List<EzyUser> globalServerEzyUsers;

    public List<UnityRoom> getServers()
    {
        return globalRoomManager.getRoomList();
    }

    public void setServerReady(EzyUser user)
    {
        for(UnityRoom room : globalRoomManager.getRoomList())
        {
            if(room.getName().equals(user.getName()))
            {
                logger.info("Set room: {} to ready", room.getName());
                room.setStatus(UnityRoomStatus.READY);
            }
        }
    }
    public List<EzyUser> getServersAsEzyUser()
    {
        return globalServerEzyUsers;
    }

    public void setServerStatus(EzyUser user, UnityRoomStatus status)
    {
        for(UnityRoom room : globalRoomManager.getRoomList())
        {
            if(room.getName().equals(user.getName()))
            {
                logger.info("Set status of room: {} to UnityRoomStatus.{}", room.getName(), status.getName());
                room.setStatus(status);
            }
        }
    }
}

