package com.assambra.masterserver.common.masterserver.entity;

import com.tvd12.gamebox.entity.Player;
import lombok.Getter;

@Getter
public class UnityPlayer extends Player {

    protected final Long id;
    protected final String username;

    public UnityPlayer(Builder<?> builder) {
        super(builder);
        this.id = builder.id;
        this.username = builder.username;
    }

    public static Builder<?> builder() {
        return new Builder<>();
    }

    public static class Builder<B extends Builder<B>> extends  Player.Builder<B> {

        protected long id;
        protected String username;

        public B id(long id) {
            this.id = id;
            return (B) this;
        }

        @Override
        public B name(String name){
            super.name(name);
            return (B) this;
        }

        public B username(String username) {
            this.username = username;
            return (B) this;
        }

        @Override
        public UnityPlayer build() {
            return (UnityPlayer) super.build();
        }

        @Override
        protected UnityPlayer newProduct() {
            return new UnityPlayer(this);
        }
    }
}
