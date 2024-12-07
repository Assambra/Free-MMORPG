package com.assambra.masterserver.app.controller;

import com.assambra.masterserver.app.converter.RequestToModelConverter;
import com.assambra.masterserver.app.model.request.RequestAccountActivationModel;
import com.assambra.masterserver.app.model.request.RequestCreateAccountModel;
import com.assambra.masterserver.app.model.request.RequestForgotPasswordModel;
import com.assambra.masterserver.app.model.request.RequestForgotUsernameModel;
import com.assambra.masterserver.common.config.ServerConfig;
import com.assambra.masterserver.app.request.*;
import com.assambra.masterserver.common.entity.Account;
import com.assambra.masterserver.common.mail.MailBuilder;
import com.assambra.masterserver.common.mail.SMTP_EMail;
import com.assambra.masterserver.app.helper.RandomString;
import com.assambra.masterserver.app.service.AccountService;
import com.assambra.masterserver.app.constant.Commands;

import com.assambra.masterserver.common.mail.mailbodys.UserActivationCodeMailBody;
import com.assambra.masterserver.common.mail.mailbodys.ForgotPasswordMailBody;
import com.assambra.masterserver.common.mail.mailbodys.ForgotUsernameMailBody;
import com.tvd12.ezyfox.bean.annotation.EzyAutoBind;
import com.tvd12.ezyfox.core.annotation.EzyDoHandle;
import com.tvd12.ezyfox.core.annotation.EzyRequestController;
import com.tvd12.ezyfox.security.EzySHA256;
import com.tvd12.ezyfox.util.EzyLoggable;
import com.tvd12.ezyfoxserver.entity.EzyUser;
import com.tvd12.ezyfoxserver.support.factory.EzyResponseFactory;

import freemarker.template.TemplateException;
import lombok.AllArgsConstructor;

import java.io.IOException;

@AllArgsConstructor
@EzyRequestController
public class AccountController extends EzyLoggable {

    private final AccountService accountService;
    private final EzyResponseFactory responseFactory;
    private final RequestToModelConverter requestToModelConverter;
    @EzyAutoBind
    private ServerConfig serverConfig;
    private final SMTP_EMail mail = new SMTP_EMail();


    @EzyDoHandle(Commands.CREATE_ACCOUNT)
    public void createAccount(EzyUser ezyUser, CreateAccountRequest request) throws IOException, TemplateException
    {
        logger.info("Account: Receive CREATE_ACCOUNT for new user {}", request.getUsername());

        RequestCreateAccountModel requestCreateAccountModel = requestToModelConverter.toModel(request);

        String resultMessage = "";

        Account account = accountService.getAccountByUsername(requestCreateAccountModel.getUsername());
        if(account == null)
            account = accountService.getAccountByEMail(requestCreateAccountModel.getEmail());

        if(account == null)
        {
            if(!request.getUsername().toLowerCase().contains("guest"))
            {
                String randomstring = RandomString.getAlphaNumericString(8);

                accountService.createAccount(requestCreateAccountModel, randomstring);

                if(serverConfig.getCan_send_mail())
                {
                    UserActivationCodeMailBody userActivationCodeMailBody = new UserActivationCodeMailBody();
                    MailBuilder mailBuilder = new MailBuilder();
                    mailBuilder.setBodyTemplate(userActivationCodeMailBody);
                    mailBuilder.setVariable("activationCode", randomstring);

                    // Todo set subject as variable
                    mail.sendMail(request.getEmail(), "Your activation code", mailBuilder.buildEmail());

                    logger.info("Account: Send activation code to {} for user: {}", request.getEmail(), request.getUsername());
                }
                else
                {
                    logger.warn("Warning: Setup the server to send emails!");
                    logger.info("Activation code: {} for user: {}", randomstring, request.getUsername());
                }

                resultMessage = "successful";
                logger.info("Account: created successfully a new account for user {}", request.getUsername());
            }
            else
            {
                resultMessage = "username_are_not_allowed";
                logger.info("Account: {} tried to create a new account but username are not allowed!", request.getUsername());
            }
        }
        else
        {
            if(account.getEmail().equals(request.getEmail().toLowerCase()))
            {
                resultMessage = "email_already_registered";
                logger.info("Account: {} tried to create a new account but email: {} already registered!", request.getUsername(), request.getEmail());
            }
            else if(account.getUsername().equals(request.getUsername()))
            {
                resultMessage = "username_already_in_use";
                logger.info("Account: {} tried to create a new account but username already in use!", request.getUsername());
            }
        }

        responseFactory.newObjectResponse()
                .command(Commands.CREATE_ACCOUNT)
                .param("result", resultMessage)
                .user(ezyUser)
                .execute();
    }

    @EzyDoHandle(Commands.ACTIVATE_ACCOUNT)
    public void activateAccount(EzyUser ezyUser, ActivateAccountRequest request)
    {
        logger.info("Account: Receive ACTIVATE_USER for user {}", ezyUser.getName());

        RequestAccountActivationModel requestAccountActivationModel = requestToModelConverter.toModel(request);

        boolean activated = accountService.activateAccount(ezyUser.getName(), requestAccountActivationModel.getActivationCode());
        String result;

        if(activated)
            result = "successful";
        else
            result = "wrong_activation_code";

        responseFactory.newObjectResponse()
                .command(Commands.ACTIVATE_ACCOUNT)
                .param("result", result)
                .user(ezyUser)
                .execute();
    }

    @EzyDoHandle(Commands.RESEND_ACTIVATION_MAIL)
    public void resendActivationMail(EzyUser ezyUser, ResendActivationMailRequest request) throws IOException, TemplateException
    {
        logger.info("Account: Receive RESEND_ACTIVATION_MAIL for user {}", request.getUsername());

        Account account =  accountService.getAccountByUsername(request.getUsername());

        if(account != null)
        {
            String activationCode = account.getActivationCode();

            if(serverConfig.getCan_send_mail())
            {
                UserActivationCodeMailBody userActivationCodeMailBody = new UserActivationCodeMailBody();
                MailBuilder mailBuilder = new MailBuilder();
                mailBuilder.setBodyTemplate(userActivationCodeMailBody);
                mailBuilder.setVariable("activationCode", activationCode);

                // Todo set subject as variable
                mail.sendMail(account.getEmail(), "Your activation code", mailBuilder.buildEmail());

                logger.info("Account: Send activation code to {} for account: {}", account.getEmail(), account.getUsername());
            }
            else
            {
                logger.warn("Warning: Setup the server to send emails!");
                logger.info("Account: Activation code: {} for account: {}", activationCode, account.getUsername());
            }

            responseFactory.newObjectResponse()
                    .command(Commands.RESEND_ACTIVATION_MAIL)
                    .user(ezyUser)
                    .execute();
        }
        else
            logger.error("Account: Resend activation code user {} unknown user!", request.getUsername());
    }

    @EzyDoHandle(Commands.FORGOT_PASSWORD)
    public void forgotPassword(EzyUser ezyUser, ForgotPasswordRequest request) throws IOException, TemplateException
    {
        logger.info("Account: Receive FORGOT_PASSWORD for account or email-address {}", request.getUsernameOrEMail());

        String resultMessage;

        RequestForgotPasswordModel requestForgotPasswordModel = requestToModelConverter.toModel(request);

        Account account = accountService.getAccountByUsernameOrEMail(requestForgotPasswordModel.getUsernameOrEMail());

        if (account == null)
        {
            resultMessage = "no_account";

            logger.info("Account: User or email-address {} tried to get a new password but no username or email address found!", request.getUsernameOrEMail());
        }
        else
        {
            String randomstring = RandomString.getAlphaNumericString(8);

            accountService.updatePassword(account.getId(), randomstring);

            if(serverConfig.getCan_send_mail())
            {
                ForgotPasswordMailBody forgotPasswordMailBody = new ForgotPasswordMailBody();
                MailBuilder mailBuilder = new MailBuilder();
                mailBuilder.setBodyTemplate(forgotPasswordMailBody);
                mailBuilder.setVariable("password", randomstring);

                // Todo set subject as variable
                mail.sendMail(account.getEmail(), "Your new password", mailBuilder.buildEmail());

                logger.info("Account: Send new password to {} for user: {}", account.getEmail(), account.getUsername());
            }
            else
            {
                logger.warn("Warning: Setup the server to send emails!");
                logger.info("Account: New password: {} for user or e-mail-address: {}", randomstring, request.getUsernameOrEMail());
            }

            resultMessage = "successful";
        }

        responseFactory.newObjectResponse()
                .command(Commands.FORGOT_PASSWORD)
                .param("result", resultMessage)
                .user(ezyUser)
                .execute();
    }

    @EzyDoHandle(Commands.FORGOT_USERNAME)
    public void forgotUsername(EzyUser ezyUser, ForgotUsernameRequest request) throws IOException, TemplateException
    {
        logger.info("Account: Receive FORGOT_USERNAME for email {}", request.getEmail());

        RequestForgotUsernameModel requestForgotUsernameModel = requestToModelConverter.toModel(request);

        String resultMessage;

        Account account = accountService.getAccountByEMail(requestForgotUsernameModel.getEmail().toLowerCase());

        if(account == null)
        {
            resultMessage = "not_found";
            logger.info("Account: {} tried to get username but no email address found!", request.getEmail());
        }
        else
        {
            String username = account.getUsername();

            if(serverConfig.getCan_send_mail())
            {
                ForgotUsernameMailBody forgotUsernameMailBody = new ForgotUsernameMailBody();
                MailBuilder mailBuilder = new MailBuilder();
                mailBuilder.setBodyTemplate(forgotUsernameMailBody);
                mailBuilder.setVariable("username", username);

                mail.sendMail(account.getEmail(), "Your Username", mailBuilder.buildEmail());

                logger.info("Account: Send username to {} for user: {}", account.getEmail(), account.getUsername());
            }
            else
            {
                logger.warn("Warning: Setup the server to send emails!");
                logger.info("Account: Username: {} for e-mail-address: {}", username, request.getEmail());
            }

            resultMessage = "successful";
        }

        responseFactory.newObjectResponse()
                .command(Commands.FORGOT_USERNAME)
                .param("result", resultMessage)
                .user(ezyUser)
                .execute();
    }


}
