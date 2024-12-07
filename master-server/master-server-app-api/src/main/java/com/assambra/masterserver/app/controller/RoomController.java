package com.assambra.masterserver.app.controller;

import com.assambra.masterserver.app.constant.Commands;
import com.assambra.masterserver.app.converter.RequestToModelConverter;
import com.assambra.masterserver.app.model.PlayerDespawnModel;
import com.assambra.masterserver.app.model.PlayerSpawnModel;
import com.assambra.masterserver.app.model.request.RequestChangeServerModel;
import com.assambra.masterserver.app.request.ChangeServerRequest;
import com.assambra.masterserver.app.service.PlayerService;
import com.assambra.masterserver.app.service.RoomService;
import com.assambra.masterserver.common.masterserver.entity.UnityPlayer;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.gamebox.math.Vec3;
import lombok.AllArgsConstructor;

@AllArgsConstructor
@EzyRequestController
public class RoomController extends EzyLoggable {

    private final RoomService roomService;
    private final PlayerService playerService;
    private final RequestToModelConverter requestToModelConverter;

    @EzyDoHandle(Commands.CHANGE_SERVER)
    public void changeServer(EzyUser ezyUser, ChangeServerRequest request)
    {
        RequestChangeServerModel requestChangeServerModel = requestToModelConverter.toModel(request);

        UnityPlayer player = playerService.getPlayerByIdFromGlobalPlayerManager(requestChangeServerModel.getPlayerId());

        Vec3 position = new Vec3(
                requestChangeServerModel.getPosition().get(0, float.class),
                requestChangeServerModel.getPosition().get(1, float.class),
                requestChangeServerModel.getPosition().get(2, float.class)
        );

        Vec3 rotation = new Vec3(
                requestChangeServerModel.getRotation().get(0, float.class),
                requestChangeServerModel.getRotation().get(1, float.class),
                requestChangeServerModel.getRotation().get(2, float.class)
        );

        PlayerDespawnModel playerDespawnModel = playerService.getPlayerDespawnModel(requestChangeServerModel.getPlayerId());
        roomService.removePlayerFromRoom(player, playerDespawnModel);

        PlayerSpawnModel playerSpawnModel = playerService.getPlayerSpawnModel(player, position, rotation);
        roomService.addPlayerToRoom(player, request.getRoom(), playerSpawnModel);
    }
}
