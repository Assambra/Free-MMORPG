package com.assambra.masterserver.app.controller;

import com.assambra.masterserver.common.masterserver.entity.UnityRoom;
import com.tvd12.ezyfox.bean.annotation.EzyAutoBind;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.core.annotation.EzyEventHandler;
import com.tvd12.ezyfoxserver.context.EzyAppContext;
import com.tvd12.ezyfoxserver.controller.EzyAbstractAppEventController;
import com.tvd12.ezyfoxserver.event.EzyServerReadyEvent;
import com.tvd12.gamebox.manager.RoomManager;

import static com.tvd12.ezyfoxserver.constant.EzyEventNames.SERVER_READY;

@EzySingleton
@EzyEventHandler(SERVER_READY)
public class ServerReadyController extends EzyAbstractAppEventController<EzyServerReadyEvent> {

    @EzyAutoBind
    private RoomManager<UnityRoom> globalRoomManager;

    @Override
    public void handle(EzyAppContext ctx, EzyServerReadyEvent event) {
        logger.info("Initialize Server: World");
        globalRoomManager.addRoom(world());
        logger.info("Initialize Server: Newcomer");
        globalRoomManager.addRoom(newcomer());
    }

    // Don't use Room with id 0 because if we remove a Player from a room we set player.setCurrentRoomId(0);

    private UnityRoom newcomer()
    {
        return UnityRoom.builder()
                .id(1)
                .name("Newcomer")
                .isStatic(true)
                .maxPlayer(10000)
                .build();
    }

    private UnityRoom world()
    {
        return UnityRoom.builder()
                .id(2)
                .name("World")
                .isStatic(true)
                .maxPlayer(10000)
                .build();
    }
}
