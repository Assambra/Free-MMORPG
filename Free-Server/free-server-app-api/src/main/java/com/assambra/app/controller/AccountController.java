package com.assambra.app.controller;

import com.assambra.app.constant.Commands;
import com.assambra.app.request.CreateAccountRequest;
import com.assambra.app.service.AccountService;
import com.assambra.common.entity.Account;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;
import lombok.AllArgsConstructor;

@AllArgsConstructor
@EzyRequestController
public class AccountController extends EzyLoggable {

    private final AccountService accountService;
    private final EzyResponseFactory responseFactory;

    @EzyDoHandle(Commands.CREATE_ACCOUNT)
    public void createAccount(EzyUser user, CreateAccountRequest request)
    {
        Account account = accountService.getAccount(request.getUsername());
        String resultmessage = "";

        if(account == null)
        {
            getLogger().info("Account doesn't exist in db, create new one -> E-Mail: {}, Username: {}, Password: {}", request.getEmail(), request.getUsername(), request.getPassword());
            accountService.createAccount(request.getEmail(), request.getUsername(), request.getPassword());
            resultmessage = "successfully";
        }
        else
        {
            if(account.getEmail().equals(request.getEmail().toLowerCase()))
            {
                logger.info("E-Mail already registered");
                resultmessage = "email_already_registered";
            }
            else if(account.getUsername().equals(request.getUsername()))
            {
                logger.info("Username already in use");
                resultmessage ="username_already_in_use";
            }
        }

        responseFactory.newObjectResponse()
                .command(Commands.CREATE_ACCOUNT)
                .param("result", resultmessage)
                .user(user)
                .execute();
    }
}
