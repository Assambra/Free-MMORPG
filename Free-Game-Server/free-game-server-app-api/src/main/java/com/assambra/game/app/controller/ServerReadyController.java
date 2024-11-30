package com.assambra.game.app.controller;

import com.assambra.game.common.masterserver.entity.UnityRoom;
import com.tvd12.ezyfox.bean.annotation.EzyAutoBind;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.core.annotation.EzyEventHandler;
import com.tvd12.ezyfoxserver.context.EzyAppContext;
import com.tvd12.ezyfoxserver.controller.EzyAbstractAppEventController;
import com.tvd12.ezyfoxserver.event.EzyServerReadyEvent;
import com.tvd12.gamebox.manager.RoomManager;

import static com.tvd12.ezyfoxserver.constant.EzyEventNames.SERVER_READY;

@EzySingleton
@EzyEventHandler(SERVER_READY) // refer EzyEventType
public class ServerReadyController
        extends EzyAbstractAppEventController<EzyServerReadyEvent> {

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
        return UnityRoom.builder(true)
                .id(1)
                .name("Newcomer")
                .maxPlayer(10000)
                .build();
    }

    private UnityRoom world()
    {
        return UnityRoom.builder(true)
                .id(2)
                .name("World")
                .maxPlayer(10000)
                .build();
    }
}
