package com.assambra.masterserver.common.masterserver.server;

import com.assambra.masterserver.common.config.ServerConfig;
import com.tvd12.ezyfox.bean.annotation.EzyAutoBind;
import com.tvd12.ezyfox.util.EzyLoggable;
import lombok.Setter;

import java.io.IOException;

@Setter
public class UnityServer extends EzyLoggable {
    @EzyAutoBind
    private ServerConfig serverConfig;
    private final String username;
    private final String password;
    private final String room;

    private UnityServer(Builder builder) {
        this.username = builder.username;
        this.password = builder.password;
        this.room = builder.room;
    }

    public static class Builder {
        private String username;
        private String password;
        private String room;

        public Builder username(String username) {
            this.username = username;
            return this;
        }

        public Builder password(String password) {
            this.password = password;
            return this;
        }

        public Builder room(String room) {
            this.room = room;
            return this;
        }

        public UnityServer build() {
            return new UnityServer(this);
        }
    }

    public Process start() throws IOException {
        try {
            ProcessBuilder processBuilder = new ProcessBuilder(
                    serverConfig.getServer_executable_path(),
                    "--username", username,
                    "--password", password,
                    "--room", room);

            processBuilder.inheritIO();
            Process process = processBuilder.start();

            Runtime.getRuntime().addShutdownHook(new Thread(() -> destroy(process)));

            return process;
        } catch (IOException e) {
            e.printStackTrace();
            return null;
        }
    }

    public static void destroy(Process process) {
        if (process != null) {
            process.destroy();
            try {
                process.waitFor();
            } catch (InterruptedException e) {
                e.printStackTrace();
                Thread.currentThread().interrupt();
            }
        }
    }
}
