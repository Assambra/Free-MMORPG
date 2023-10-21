package com.assambra.game.test;

import com.assambra.game.ApplicationStartup;
import com.assambra.game.app.constant.Commands;
import com.tvd12.ezyfox.core.constant.EzyResponseCommands;
import com.tvd12.ezyfox.entity.EzyArray;
import com.tvd12.ezyfox.entity.EzyData;
import com.tvd12.ezyfox.entity.EzyObject;
import com.tvd12.ezyfox.factory.EzyEntityFactory;
import com.tvd12.ezyfoxserver.client.EzyClient;
import com.tvd12.ezyfoxserver.client.EzyClients;
import com.tvd12.ezyfoxserver.client.EzyUTClient;
import com.tvd12.ezyfoxserver.client.config.EzyClientConfig;
import com.tvd12.ezyfoxserver.client.constant.EzyCommand;
import com.tvd12.ezyfoxserver.client.entity.EzyApp;
import com.tvd12.ezyfoxserver.client.handler.EzyAppAccessHandler;
import com.tvd12.ezyfoxserver.client.handler.EzyHandshakeHandler;
import com.tvd12.ezyfoxserver.client.handler.EzyLoginSuccessHandler;
import com.tvd12.ezyfoxserver.client.handler.EzyUdpHandshakeHandler;
import com.tvd12.ezyfoxserver.client.request.EzyAppAccessRequest;
import com.tvd12.ezyfoxserver.client.request.EzyLoginRequest;
import com.tvd12.ezyfoxserver.client.request.EzyRequest;
import com.tvd12.ezyfoxserver.client.setup.EzyAppSetup;
import com.tvd12.ezyfoxserver.client.setup.EzySetup;
import com.tvd12.ezyfoxserver.client.socket.EzyMainEventsLoop;

import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

public class ClientTest {

    public static void main(String[] args) {
        EzyClientConfig config = EzyClientConfig.builder()
            .zoneName(ApplicationStartup.ZONE_APP_NAME)
            .enableDebug()
            .build();
        EzyClient client = new EzyUTClient(config);
        setupClient(client);
        EzyClients.getInstance().addClient(client);
        client.connect("127.0.0.1", 3005);
        EzyMainEventsLoop mainEventsLoop = new EzyMainEventsLoop();
        mainEventsLoop.start(5);
    }

    public static void setupClient(EzyClient client) {
        EzySetup setup = client.setup();
        setup.addDataHandler(EzyCommand.HANDSHAKE, new EzyHandshakeHandler() {

            @Override
            protected EzyRequest getLoginRequest() {
                return new EzyLoginRequest(
                    ApplicationStartup.ZONE_APP_NAME,
                    "Assambra",
                    "123456"
                );
            }
        });
        setup.addDataHandler(EzyCommand.LOGIN, new EzyLoginSuccessHandler() {
            @Override
            protected void handleLoginSuccess(EzyData responseData) {
                client.udpConnect(2611);
            }
        });
        setup.addDataHandler(EzyCommand.UDP_HANDSHAKE, new EzyUdpHandshakeHandler() {
            @Override
            protected void onAuthenticated(EzyArray data) {
                client.send(new EzyAppAccessRequest(ApplicationStartup.ZONE_APP_NAME));
            }
        });
        setup.addDataHandler(EzyCommand.APP_ACCESS, new EzyAppAccessHandler() {
            @Override
            protected void postHandle(EzyApp app, EzyArray data) {
                System.out.println("access app: " + data);
                app.send(Commands.HELLO, EzyEntityFactory.EMPTY_OBJECT);
                app.send(Commands.HELLO, EzyEntityFactory.newObjectBuilder()
                    .append("nickName", "Dzung")
                    .build());
                app.send(Commands.GO, EzyEntityFactory.EMPTY_OBJECT);
                app.send(Commands.GO, EzyEntityFactory.newObjectBuilder()
                    .append("nickName", "Dzung")
                    .build());
                app.send(
                    "createCharacter",
                    EzyEntityFactory.newObjectBuilder()
                        .append("name", "Dung")
                        .append("sex", "male")
                        .append("race", "test")
                        .append("model", "test")
                        .build()
                );
            }
        });

        EzyAppSetup appSetup = setup.setupApp(ApplicationStartup.ZONE_APP_NAME);
        appSetup.addDataHandler(EzyResponseCommands.ERROR, (app, data) ->
            System.out.println("error: " + data)
        );
        appSetup.addDataHandler(Commands.HELLO, (app, data) ->
            System.out.println("hello: " + data)
        );
        appSetup.addDataHandler(Commands.GO, (app, data) ->
            System.out.println("hello: " + data)
        );
        appSetup.addDataHandler("characterList", (app, data) -> {
            EzyArray characterList = (EzyArray) data;
            if (characterList.size() > 0) {
                System.out.println("start to play");
                app.send(
                    "play",
                    EzyEntityFactory
                        .newObjectBuilder()
                        .append(
                            "characterId",
                            characterList
                                .get(0, EzyArray.class)
                                .get(0, Integer.class))
                        .build()
                );
            }
        });
        appSetup.addDataHandler("createCharacter", (app, data) -> {
            System.out.println("createCharacter: " + data);
            EzyObject objData = (EzyObject) data;
            String result = objData.get("result");
            if ("name_already_in_use".equals(result)) {
                app.send("characterList");
            } else {
                app.send(
                    "play",
                    EzyEntityFactory
                        .newObjectBuilder()
                        .append("characterId", objData.get("characterId", Integer.class))
                        .build()
                );
            }
        });
        appSetup.addDataHandler("play", (app, data) -> {
            System.out.println("play: " + data);
            startSyncPositionSchedule(app);
        });
        appSetup.addDataHandler("s", (app, data) ->
            System.out.println("position: " + data)
        );
    }

    private static void startSyncPositionSchedule(EzyApp app) {
        ScheduledExecutorService service = Executors.newSingleThreadScheduledExecutor();
        service.scheduleAtFixedRate(
            () -> app.udpSend(
                "playerInput",
                EzyEntityFactory
                    .newObjectBuilder()
                    .append("t", 1)
                    .append("i", new boolean[] {true, false, true, false})
                    .append("r", new float[] {0.0F, 762.0F, 0.0F})
                    .build()
            ),
            1,
            1,
            TimeUnit.SECONDS
        );
    }
}
