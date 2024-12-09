package com.assambra.masterserver.common.masterserver.entity;

import com.assambra.masterserver.common.masterserver.constant.UnityRoomStatus;
import com.assambra.masterserver.common.masterserver.server.UnityServer;
import com.assambra.masterserver.common.masterserver.util.RandomStringUtil;
import com.tvd12.gamebox.entity.Player;
import com.tvd12.gamebox.entity.Room;
import com.tvd12.gamebox.manager.DefaultPlayerManager;
import com.tvd12.gamebox.manager.PlayerManager;
import lombok.AccessLevel;
import lombok.Getter;
import lombok.Setter;

import java.io.IOException;

@Getter
@SuppressWarnings({"unchecked", "rawtypes"})
public class UnityRoom extends Room {

    @Setter(AccessLevel.NONE)
    protected final PlayerManager playerManager;
    @Getter
    protected final boolean isStatic;
    @Getter
    protected final String serverUserPassword;
    @Getter
    protected Process unityProcess;
    protected final UnityServer unityServer;

    public UnityRoom(Builder<?> builder) {
        super(builder);
        this.playerManager = builder.playerManager;
        this.isStatic = builder.isStatic;
        this.status = UnityRoomStatus.STARTING;
        this.serverUserPassword = RandomStringUtil.getAlphaNumericString(6);

        this.unityServer = new UnityServer.Builder()
                .username(this.name)
                .password(serverUserPassword)
                .room(this.name)
                .build();

        try {
            this.unityProcess = unityServer.start();
        } catch (IOException e) {
            e.printStackTrace();
            // Todo create room exception
        }
    }

    public static Builder<?> builder() {
        return new Builder<>();
    }

    public void addPlayer(Player player) {
        player.setCurrentRoomId(id);
        playerManager.addPlayer(player);
    }

    public void removePlayer(Player player) {
        playerManager.removePlayer(player.getName());
    }

    public int getMaxPlayer(){
        return this.getPlayerManager().getMaxPlayer();
    }

    public <T extends PlayerManager> T getPlayerManager() {
        return (T) playerManager;
    }

    public static class Builder<B extends Builder<B>> extends Room.Builder<B> {

        protected int maxPlayer = 2;
        protected PlayerManager playerManager;
        protected boolean isStatic;

        @Override
        public B id(long id) {
            super.id(id);
            return (B) this;
        }

        @Override
        public B name(String name) {
            super.name(name);
            return (B) this;
        }

        public B maxPlayer(int maxPlayer) {
            this.maxPlayer = maxPlayer;
            return (B) this;
        }

        public B playerManager(PlayerManager playerManager) {
            this.playerManager = playerManager;
            return (B) this;
        }

        public B defaultPlayerManager(int maxPlayer) {
            this.playerManager = new DefaultPlayerManager<>(maxPlayer);
            return (B) this;
        }

        public B isStatic(boolean isStatic) {
            this.isStatic = isStatic;
            return (B) this;
        }

        @Override
        protected void preBuild() {
            if (playerManager == null) {
                playerManager = new DefaultPlayerManager<>(maxPlayer);
            }
        }

        @Override
        public UnityRoom build() {
            return (UnityRoom) super.build();
        }

        @Override
        protected UnityRoom newProduct() {
            return new UnityRoom(this);
        }
    }
}
