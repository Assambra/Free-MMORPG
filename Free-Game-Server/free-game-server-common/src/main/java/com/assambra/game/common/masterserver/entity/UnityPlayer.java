package com.assambra.game.common.masterserver.entity;

import com.tvd12.gamebox.entity.Player;
import lombok.Getter;
import lombok.Setter;

public class UnityPlayer extends Player {
    public UnityPlayer(String name) {
        super(name);
    }

    @Getter @Setter
    protected Long id;
    @Getter @Setter
    protected String username;

    protected UnityPlayer(Builder builder) {
        super(builder);
    }

    public static Builder builder() {
        return new Builder();
    }

    public static class Builder extends Player.Builder<Builder> {

        @Override
        protected Player newProduct() {
            return new UnityPlayer(this);
        }

        @Override
        public UnityPlayer build() {
            return (UnityPlayer) super.build();
        }
    }
}
