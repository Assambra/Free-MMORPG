package com.assambra.masterserver.common.masterserver.manager;

import com.assambra.masterserver.common.masterserver.constant.Commands;
import com.assambra.masterserver.common.masterserver.entity.UnityRoom;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import com.tvd12.gamebox.manager.AbstractRoomManager;

public class UnityRoomManager <R extends UnityRoom> extends AbstractRoomManager<R> {

    protected EzyResponseFactory responseFactory;

    public UnityRoomManager() { this(10000); }

    public UnityRoomManager(int maxRoom) { super(maxRoom); }

    protected UnityRoomManager(Builder<?, ?> builder){ super(builder); }

    public static Builder<?, ?> builder() { return new Builder<>(); }

    @Override
    public void removeRoom(R room) {
        super.removeRoom(room);

        sendServerStop(room.getName());
    }

    @Override
    public void removeRoom(long id) {
        super.removeRoom(id);

        sendServerStop(getRoom(id).getName());
    }

    @Override
    public void removeRoom(String name) {
        super.removeRoom(name);

        sendServerStop(getRoom(name).getName());
    }

    @Override
    public void removeRooms(Iterable<R> rooms)
    {
        super.removeRooms(rooms);

        for (R room : rooms) {
            sendServerStop(room.getName());
        }
    }

    private void sendServerStop(String username)
    {
        responseFactory.newObjectResponse()
                .command(Commands.SERVER_STOP)
                .username(username)
                .execute();
    }

    public static class Builder<R extends UnityRoom, B extends Builder<R, B>> extends AbstractRoomManager.Builder<R, B>{

        @Override
        public UnityRoomManager<R> build() { return new UnityRoomManager<>(this); }
    }
}
