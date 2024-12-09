package com.assambra.masterserver.app.controller;

import com.assambra.masterserver.app.model.PlayerDespawnModel;
import com.assambra.masterserver.app.service.PlayerService;
import com.assambra.masterserver.app.service.RoomService;
import com.assambra.masterserver.common.masterserver.entity.UnityPlayer;
import com.assambra.masterserver.common.masterserver.manager.UnityPlayerManager;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.core.annotation.EzyEventHandler;
import com.tvd12.ezyfoxserver.context.EzyAppContext;
import com.tvd12.ezyfoxserver.controller.EzyAbstractAppEventController;
import com.tvd12.ezyfoxserver.event.EzyUserRemovedEvent;
import lombok.AllArgsConstructor;

import java.util.List;

import static com.tvd12.ezyfoxserver.constant.EzyEventNames.USER_REMOVED;

@EzySingleton
@AllArgsConstructor
@EzyEventHandler(USER_REMOVED)
public class UserRemovedController extends EzyAbstractAppEventController<EzyUserRemovedEvent> {

    private final UnityPlayerManager<UnityPlayer> globalPlayerManager;
    private final PlayerService playerService;
    private final RoomService roomService;

    @Override
    public void handle(EzyAppContext ctx, EzyUserRemovedEvent event) {

        List<UnityPlayer> players = globalPlayerManager.getPlayerList();

        for(UnityPlayer player : players)
        {
            if(player.getUsername().equals(event.getUser().getName()))
            {
                PlayerDespawnModel playerDespawnModel = playerService.getPlayerDespawnModel(player.getId());
                roomService.removePlayerFromRoom(player, playerDespawnModel);
                playerService.removePlayerFromGlobalPlayerList(player);
            }
        }
    }
}
