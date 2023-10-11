package com.assambra.game.app.config;

import com.tvd12.ezyfox.bean.annotation.EzyAutoBind;
import com.tvd12.ezyfox.bean.annotation.EzyConfigurationBefore;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import com.tvd12.gamebox.entity.MMOVirtualWorld;
import com.tvd12.gamebox.handler.MMORoomUpdatedHandler;
import com.tvd12.gamebox.handler.SyncPositionRoomUpdatedHandler;

@EzyConfigurationBefore(priority = 1)
public class MMOVirtualWorldConfig extends EzyLoggable {

    @EzyAutoBind
    private EzyResponseFactory responseFactory;

    @EzySingleton
    public MMOVirtualWorld mmoVirtualWorld() {
        logger.info("Initialize MMO Virtual World");
        return MMOVirtualWorld.builder().build();
    }

    @EzySingleton
    public MMORoomUpdatedHandler mmoRoomUpdatedHandler() {
        SyncPositionRoomUpdatedHandler handler = new SyncPositionRoomUpdatedHandler();
        handler.setResponseFactory(responseFactory);
        return handler;
    }
}
