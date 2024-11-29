package com.assambra.game.common.masterserver.server;

import com.tvd12.ezyfox.stream.EzyAnywayInputStreamLoader;
import com.tvd12.ezyfox.util.EzyLoggable;
import lombok.Setter;

import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

@Setter
public class UnityServer extends EzyLoggable {
    private final Properties properties = new Properties();
    private String path;
    private final String username;
    private final String password;
    private final String room;

    private UnityServer(Builder builder) {
        this.username = builder.username;
        this.password = builder.password;
        this.room = builder.room;

        loadProperties();
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


    private void loadProperties() {
        try (InputStream inputStream = EzyAnywayInputStreamLoader.builder()
                .context(getClass())
                .build()
                .load("application.properties")) {
            properties.load(inputStream);
            this.path = properties.getProperty("server.path");
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public Process start() throws IOException {
        try {
            ProcessBuilder processBuilder = new ProcessBuilder(
                    path,
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
