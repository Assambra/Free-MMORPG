package com.assambra.masterserver.common.masterserver.manager;

import com.assambra.masterserver.common.masterserver.entity.UnityPlayer;
import com.tvd12.gamebox.manager.AbstractPlayerManager;
import com.tvd12.gamebox.manager.PlayerManager;

import java.util.Collection;
import java.util.List;
import java.util.function.Consumer;
import java.util.function.Predicate;
import java.util.function.Supplier;

public class SynchronizedUnityPlayerManager<P extends UnityPlayer> extends AbstractPlayerManager<P> {

    public SynchronizedUnityPlayerManager() {
        this(999999999);
    }

    public SynchronizedUnityPlayerManager(int maxPlayer) {
        super(maxPlayer);
    }

    protected SynchronizedUnityPlayerManager(Builder<?, ?> builder) {
        super(builder);
    }

    @SuppressWarnings("rawtypes")
    public static Builder builder() {
        return new Builder<>();
    }

    @Override
    public P getPlayer(String username) {
        synchronized (this) {
            return super.getPlayer(username);
        }
    }

    @Override
    public P getPlayer(String username, Supplier<P> playerSupplier) {
        synchronized (this) {
            return super.getPlayer(username, playerSupplier);
        }
    }

    @Override
    public P getFirstPlayer() {
        synchronized (this) {
            return super.getFirstPlayer();
        }
    }

    @Override
    public List<P> getPlayerList() {
        synchronized (this) {
            return super.getPlayerList();
        }
    }

    @Override
    public List<P> getPlayerList(Predicate<P> predicate) {
        synchronized (this) {
            return super.getPlayerList(predicate);
        }
    }

    @Override
    public void getPlayerList(List<P> buffer) {
        synchronized (this) {
            super.getPlayerList(buffer);
        }
    }

    @Override
    public List<String> getPlayerNames() {
        synchronized (this) {
            return super.getPlayerNames();
        }
    }

    @Override
    public boolean containsPlayer(String username) {
        synchronized (this) {
            return super.containsPlayer(username);
        }
    }

    @Override
    public void addPlayer(P player, boolean failIfAdded) {
        synchronized (this) {
            super.addPlayer(player, failIfAdded);
        }
    }

    @Override
    public void addPlayers(Collection<P> players, boolean failIfAdded) {
        synchronized (this) {
            super.addPlayers(players, failIfAdded);
        }
    }

    @Override
    public P removePlayer(P player) {
        synchronized (this) {
            return super.removePlayer(player);
        }
    }

    @Override
    public void removePlayers(Collection<P> players) {
        synchronized (this) {
            super.removePlayers(players);
        }
    }

    @Override
    public int getPlayerCount() {
        synchronized (this) {
            return super.getPlayerCount();
        }
    }

    @Override
    public boolean available() {
        synchronized (this) {
            return super.available();
        }
    }

    @Override
    public int countPlayers(Predicate<P> tester) {
        synchronized (this) {
            return super.countPlayers(tester);
        }
    }

    @Override
    public void forEach(Consumer<P> consumer) {
        synchronized (this) {
            super.forEach(consumer);
        }
    }

    @Override
    public List<P> filterPlayers(Predicate<P> tester) {
        synchronized (this) {
            return super.filterPlayers(tester);
        }
    }

    @Override
    public boolean isEmpty() {
        synchronized (this) {
            return super.isEmpty();
        }
    }

    @Override
    public void clear() {
        synchronized (this) {
            super.clear();
        }
    }

    public static class Builder<U extends UnityPlayer, B extends Builder<U, B>>
            extends AbstractPlayerManager.Builder<U, B> {

        @Override
        protected PlayerManager<U> newProduct() {
            return new SynchronizedUnityPlayerManager<>(this);
        }
    }
}
