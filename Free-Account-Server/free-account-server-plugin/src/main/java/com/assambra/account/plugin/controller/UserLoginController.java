package com.assambra.account.plugin.controller;

import com.tvd12.ezyfox.bean.annotation.EzySingleton;
import com.tvd12.ezyfox.core.annotation.EzyEventHandler;
import com.tvd12.ezyfoxserver.constant.EzyLoginError;
import com.tvd12.ezyfoxserver.context.EzyPluginContext;
import com.tvd12.ezyfoxserver.controller.EzyAbstractPluginEventController;
import com.tvd12.ezyfoxserver.event.EzyUserLoginEvent;
import com.tvd12.ezyfoxserver.exception.EzyLoginErrorException;

import static com.tvd12.ezyfoxserver.constant.EzyEventNames.USER_LOGIN;

@EzySingleton
@EzyEventHandler(USER_LOGIN)
public class UserLoginController extends EzyAbstractPluginEventController<EzyUserLoginEvent>
{
    @Override
    public void handle(EzyPluginContext ctx, EzyUserLoginEvent event)
    {
        if(event.getUsername().contains("Guest#") && event.getPassword().contains("Assambra"))
            logger.info("Guest logged in: {}", event.getUsername());
        else
            throw new EzyLoginErrorException(EzyLoginError.INVALID_PASSWORD);
    }
}
