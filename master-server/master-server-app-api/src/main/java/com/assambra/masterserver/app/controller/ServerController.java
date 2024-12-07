package com.assambra.masterserver.app.controller;

import com.assambra.masterserver.app.converter.RequestToModelConverter;
import com.assambra.masterserver.app.model.request.RequestServerReadyModel;
import com.assambra.masterserver.app.request.ServerReadyRequest;
import com.assambra.masterserver.app.service.RoomService;
import com.assambra.masterserver.app.service.ServerService;
import com.assambra.masterserver.common.masterserver.constant.Commands;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.entity.EzyArray;
import com.tvd12.ezyfox.entity.EzyObject;
import com.tvd12.ezyfox.util.EzyEntityArrays;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.constant.EzyDisconnectReason;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.command.EzyObjectResponse;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import lombok.AllArgsConstructor;

import java.util.List;
import java.util.Map;

@AllArgsConstructor
@EzyRequestController
public class ServerController extends EzyLoggable {

    private final ServerService serverService;
    private final RoomService roomService;
    private final RequestToModelConverter requestToModelConverter;
    private final EzyResponseFactory responseFactory;

    private final List<EzyUser> globalServerEzyUsers;

    @EzyDoHandle(Commands.SERVER_READY)
    public void serverReady(EzyUser ezyUser, ServerReadyRequest request)
    {
        RequestServerReadyModel requestServerReadyModel = requestToModelConverter.toModel(request);

        if(roomService.checkRoomPassword(ezyUser.getName(), requestServerReadyModel.getPassword()))
        {
            logger.info("Receive Commands.SERVER_READY from Server: {} with correct password", ezyUser.getName());
            globalServerEzyUsers.add(ezyUser);
            serverService.setServerReady(ezyUser);
        }
        else
        {
            logger.info("Cheat: Commands.SERVER_READY from User: {} wrong password!", ezyUser.getName());
            ezyUser.disconnect(EzyDisconnectReason.ADMIN_KICK);
        }
    }

    @EzyDoHandle(Commands.CLIENT_TO_SERVER)
    public void clientToServer(EzyUser ezyUser, EzyObject request)
    {
        logger.info("Receive Commands.CLIENT_TO_SERVER from Client: {}", ezyUser.getName());

        Map<String, Object> requestData = request.toMap();

        String command = (String) requestData.remove("command");
        String room = (String) requestData.remove("room");

        EzyObjectResponse response = responseFactory.newObjectResponse()
                .command(command)
                .username(room)
                .param("username", ezyUser.getName());


        for (Map.Entry<String, Object> entry : requestData.entrySet()) {
            response.param(entry.getKey(), entry.getValue());
        }

        response.execute();
    }

    @EzyDoHandle(Commands.SERVER_TO_CLIENT)
    public void serverToClient(EzyUser ezyUser, EzyObject request)
    {
        if(serverService.getServersAsEzyUser().contains(ezyUser))
        {
            logger.info("Receive Commands.SERVER_TO_CLIENT from Server: {}", ezyUser.getName());
            Map<String, Object> requestData = request.toMap();

            String command = (String) requestData.remove("command");
            String recipient = (String) requestData.remove("recipient");

            EzyObjectResponse response = responseFactory.newObjectResponse()
                    .command(command)
                    .username(recipient);

            for (Map.Entry<String, Object> entry : requestData.entrySet()) {
                response.param(entry.getKey(), entry.getValue());
            }

            response.execute();
        }
        else
        {
            logger.info("Cheat: Receive Commands.SERVER_TO_CLIENT from User: {}", ezyUser.getName());
            ezyUser.disconnect(EzyDisconnectReason.ADMIN_KICK);
        }
    }

    @EzyDoHandle(Commands.SERVER_TO_CLIENTS)
    public void serverToClients(EzyUser ezyUser, EzyObject request)
    {
        if(serverService.getServersAsEzyUser().contains(ezyUser))
        {
            logger.info("Receive Commands.SERVER_TO_CLIENTS from Server: {}", ezyUser.getName());
            Map<String, Object> requestData = request.toMap();

            String command = (String) requestData.remove("command");
            List<Object> recipientsList = (List<Object>) requestData.remove("recipients");

            EzyArray recipients = EzyEntityArrays.newArray(recipientsList);

            EzyObjectResponse response = responseFactory.newObjectResponse()
                    .command(command)
                    .usernames(recipients.toList());

            for (Map.Entry<String, Object> entry : requestData.entrySet()) {
                response.param(entry.getKey(), entry.getValue());
            }

            response.execute();
        }
        else
        {
            logger.info("Cheat: Receive Commands.SERVER_TO_CLIENTS from User: {}", ezyUser.getName());
            ezyUser.disconnect(EzyDisconnectReason.ADMIN_KICK);
        }
    }
}