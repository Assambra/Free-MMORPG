package com.assambra.masterserver.plugin.controller;

import com.assambra.masterserver.common.entity.Account;
import com.assambra.masterserver.common.masterserver.entity.UnityRoom;
import com.assambra.masterserver.plugin.service.ServerService;
import com.assambra.masterserver.plugin.service.UserService;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.core.annotation.EzyEventHandler;
import com.tvd12.ezyfox.security.EzySHA256;
import com.tvd12.ezyfoxserver.constant.EzyLoginError;
import com.tvd12.ezyfoxserver.context.EzyPluginContext;
import com.tvd12.ezyfoxserver.controller.EzyAbstractPluginEventController;
import com.tvd12.ezyfoxserver.event.EzyUserLoginEvent;
import com.tvd12.ezyfoxserver.exception.EzyLoginErrorException;
import lombok.AllArgsConstructor;

import static com.tvd12.ezyfoxserver.constant.EzyEventNames.USER_LOGIN;

@EzySingleton
@EzyEventHandler(USER_LOGIN)
@AllArgsConstructor
public class UserLoginController extends EzyAbstractPluginEventController<EzyUserLoginEvent> {

    private final UserService userService;
    private final ServerService serverServicePlugin;

    @Override
    public void handle(EzyPluginContext ctx, EzyUserLoginEvent event)
    {
        String username = event.getUsername();
        String password = encodePassword(event.getPassword());
        Account account = userService.getUser(username);

        if(event.getUsername().contains("Guest#"))
        {
            if(event.getPassword().contains("Assambra"))
                logger.info("Guest logged in: {}", event.getUsername());
            else
                throw new EzyLoginErrorException(EzyLoginError.INVALID_PASSWORD);
        }
        else if(!serverServicePlugin.getServerUsernames().contains(username))
        {
            if (account == null) {
                throw new EzyLoginErrorException(EzyLoginError.INVALID_USERNAME);
            }

            if (!account.getPassword().equals(password)) {
                throw new EzyLoginErrorException(EzyLoginError.INVALID_PASSWORD);
            }

            logger.info("user and password match, accept user: {}", username);
        }
        else
        {
            for(UnityRoom server : serverServicePlugin.getServers())
            {
                if(server.getName().equals(username))
                {
                    if(server.getServerUserPassword().equals(event.getPassword()))
                        logger.info("Server: {}, logged in", username);
                    else
                    {
                        logger.info("Server: {}, use wrong password", username);
                        throw new EzyLoginErrorException(EzyLoginError.INVALID_PASSWORD);
                    }
                }
            }
        }
    }

    private String encodePassword(String password) {
        return EzySHA256.cryptUtfToLowercase(password);
    }
}
