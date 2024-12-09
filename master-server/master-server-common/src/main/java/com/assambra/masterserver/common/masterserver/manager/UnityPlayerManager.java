package com.assambra.masterserver.common.masterserver.manager;

import com.assambra.masterserver.common.masterserver.entity.UnityPlayer;
import com.tvd12.gamebox.manager.AbstractPlayerManager;
import com.tvd12.gamebox.manager.PlayerManager;

public class UnityPlayerManager <P extends UnityPlayer> extends AbstractPlayerManager<P>{

    public UnityPlayerManager() {
        this(999999999);
    }

    public UnityPlayerManager(int maxPlayer) {
        super(maxPlayer);
    }

    protected UnityPlayerManager(Builder<?, ?> builder) {
        super(builder);
    }

    public static Builder<?, ?> builder() {
        return new Builder<>();
    }

    public static class Builder<U extends UnityPlayer, B extends Builder<U, B>> extends AbstractPlayerManager.Builder<U, B> {

        @Override
        protected PlayerManager<U> newProduct() {
            return new UnityPlayerManager<>(this);
        }
    }
}
