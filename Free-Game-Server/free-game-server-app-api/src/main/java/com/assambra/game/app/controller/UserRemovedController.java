package com.assambra.game.app.controller;

import com.assambra.game.app.constant.Commands;
import com.assambra.game.app.entity.CharacterEntity;
import com.assambra.game.app.service.RoomService;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.core.annotation.EzyEventHandler;
import com.tvd12.ezyfoxserver.context.EzyAppContext;
import com.tvd12.ezyfoxserver.controller.EzyAbstractAppEventController;
import com.tvd12.ezyfoxserver.event.EzyUserRemovedEvent;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import com.tvd12.gamebox.entity.MMORoom;
import com.tvd12.gamebox.entity.NormalRoom;
import lombok.AllArgsConstructor;

import java.util.List;

import static com.tvd12.ezyfoxserver.constant.EzyEventNames.USER_REMOVED;

@EzySingleton
@AllArgsConstructor
@EzyEventHandler(USER_REMOVED)
public class UserRemovedController extends EzyAbstractAppEventController<EzyUserRemovedEvent> {

    private final RoomService roomService;
    private final EzyResponseFactory responseFactory;
    private final List<CharacterEntity> characterList;


    @Override
    public void handle(EzyAppContext ctx, EzyUserRemovedEvent event) {
        logger.info("EzySmashers app: user {} removed", event.getUser());

        CharacterEntity entityToRemove = null;
        for(CharacterEntity entity : characterList)
        {
            if(entity.accountUsername.equals(event.getUser().getName()))
            {
                entityToRemove = entity;
                break;
            }
        }
        if(entityToRemove != null)
            characterList.remove(entityToRemove);


        String userName = event.getUser().getName();
        NormalRoom room = roomService.removePlayer(userName);

        if (!(room instanceof MMORoom)) {
            return;
        }

        List<String> playerNames = roomService.getRoomPlayerNames(room);

        responseFactory.newObjectResponse()
                .command(Commands.CHARACTER_DESPAWNED)
                .param("userName", userName)
                .usernames(playerNames)
                .execute();
    }
}
