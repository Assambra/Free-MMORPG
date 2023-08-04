package com.assambra.plugin.controller;

import com.assambra.common.entity.Account;
import com.assambra.common.service.AccountService;
import com.assambra.plugin.service.WelcomeService;

import com.tvd12.ezyfox.bean.annotation.EzyAutoBind;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.core.annotation.EzyEventHandler;
import com.tvd12.ezyfox.security.EzySHA256;
import com.tvd12.ezyfoxserver.constant.EzyLoginError;
import com.tvd12.ezyfoxserver.context.EzyPluginContext;
import com.tvd12.ezyfoxserver.controller.EzyAbstractPluginEventController;
import com.tvd12.ezyfoxserver.event.EzyUserLoginEvent;
import com.tvd12.ezyfoxserver.exception.EzyLoginErrorException;

import static com.tvd12.ezyfoxserver.constant.EzyEventNames.USER_LOGIN;

@EzySingleton
@EzyEventHandler(USER_LOGIN)
public class UserLoginController extends EzyAbstractPluginEventController<EzyUserLoginEvent> {

    @EzyAutoBind
    private WelcomeService welcomeService;

    @EzyAutoBind
    private AccountService accountService;


    @Override
    public void handle(EzyPluginContext ctx, EzyUserLoginEvent event) {
        logger.info("{} login in", welcomeService.welcome(event.getUsername()));

        String username = event.getUsername();
        String password = encodePassword(event.getPassword());

        Account account = accountService.getAccount(username);

        if(event.getUsername().contains("Guest#"))
            logger.info("Guest logged in: {}", event.getUsername());
        else
        {
            if(!account.getPassword().equals(password))
            {
                throw new EzyLoginErrorException(EzyLoginError.INVALID_PASSWORD);
            }

            logger.info("user and password match, accept user {}", username);
        }
    }

    private String encodePassword(String password)
    {
        return EzySHA256.cryptUtfToLowercase(password);
    }
}
