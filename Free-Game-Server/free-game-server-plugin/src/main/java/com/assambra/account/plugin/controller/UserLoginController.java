package com.assambra.account.plugin.controller;

import com.assambra.account.common.entity.Account;
import com.assambra.account.plugin.service.AccountService;
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
    private AccountService accountService;

    @Override
    public void handle(EzyPluginContext ctx, EzyUserLoginEvent event) {

        String username = event.getUsername();
        String password = encodePassword(event.getPassword());

        Account account = accountService.getAccount(username);

        if(account == null)
            throw new EzyLoginErrorException(EzyLoginError.INVALID_USERNAME);

        if(!account.getPassword().equals(password))
            throw new EzyLoginErrorException(EzyLoginError.INVALID_PASSWORD);

        logger.info("user and password match, accept user {}", username);
    }

    private String encodePassword(String password)
    {
        return EzySHA256.cryptUtfToLowercase(password);
    }
}