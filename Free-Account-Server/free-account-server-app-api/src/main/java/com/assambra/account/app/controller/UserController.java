package com.assambra.account.app.controller;

import com.assambra.account.app.request.*;
import com.assambra.account.common.mail.MailBuilder;
import com.assambra.account.common.mail.SMTP_EMail;
import com.assambra.account.app.helper.RandomString;
import com.assambra.account.app.service.UserService;
import com.assambra.account.app.constant.Commands;
import com.assambra.account.app.constant.ServerVariables;
import com.assambra.account.common.entity.User;
import com.assambra.account.common.mail.mailbodys.AccountActivationCodeMailBody;
import com.assambra.account.common.mail.mailbodys.ForgotPasswordMailBody;
import com.assambra.account.common.mail.mailbodys.ForgotUsernameMailBody;
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
public class UserController extends EzyLoggable {

    private final UserService userService;
    private final EzyResponseFactory responseFactory;
    private final SMTP_EMail mail = new SMTP_EMail();

    @EzyDoHandle(Commands.CREATE_USER)
    public void createAccount(EzyUser ezyUser, CreateAccountRequest request) throws IOException, TemplateException
    {
        logger.info("Account: Receive CREATE_ACCOUNT for new ezyUser {}", request.getUsername());

        String resultMessage = "";

        User user = userService.getUserByUsername(request.getUsername());
        if(user == null)
            user = userService.getUserByEMail(request.getEmail());

        if(user == null)
        {
            if(!request.getUsername().toLowerCase().contains("guest"))
            {
                String randomstring = RandomString.getAlphaNumericString(8);

                userService.createUser(request.getEmail().toLowerCase(), request.getUsername(), encodePassword(request.getPassword()), randomstring);

                if(ServerVariables.SERVER_CAN_SEND_MAIL)
                {
                    AccountActivationCodeMailBody accountActivationCodeMailBody = new AccountActivationCodeMailBody();
                    MailBuilder mailBuilder = new MailBuilder();
                    mailBuilder.setBodyTemplate(accountActivationCodeMailBody);
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
                logger.info("Account: {} created successfully a new user", request.getUsername());
            }
            else
            {
                resultMessage = "username_are_not_allowed";
                logger.info("Account: {} tried to create a new user but username are not allowed!", request.getUsername());
            }
        }
        else
        {
            if(user.getEmail().equals(request.getEmail().toLowerCase()))
            {
                resultMessage = "email_already_registered";
                logger.info("Account: {} tried to create a new user but email already registered!", request.getUsername());
            }
            else if(user.getUsername().equals(request.getUsername()))
            {
                resultMessage = "username_already_in_use";
                logger.info("Account: {} tried to create a new user but username already in use!", request.getUsername());
            }
        }

        responseFactory.newObjectResponse()
                .command(Commands.CREATE_USER)
                .param("result", resultMessage)
                .user(ezyUser)
                .execute();
    }

    @EzyDoHandle(Commands.ACTIVATE_USER)
    public void activateAccount(EzyUser user, ActivateAccountRequest request)
    {
        boolean activated = userService.activateUser(request.getUsername(), request.getActivationCode());
        String result;

        if(activated)
            result = "successful";
        else
            result = "wrong_activation_code";

        responseFactory.newObjectResponse()
                .command(Commands.ACTIVATE_USER)
                .param("result", result)
                .user(user)
                .execute();
    }

    @EzyDoHandle(Commands.RESEND_ACTIVATION_MAIL)
    public void resendActivationMail(EzyUser user, ResendActivationMailRequest request) throws IOException, TemplateException
    {
        logger.info("Account: Receive RESEND_ACTIVATION_MAIL for user {}", request.getUsername());

        User account =  userService.getUserByUsername(request.getUsername());

        if(account != null)
        {
            String activationCode = account.getActivationCode();

            if(ServerVariables.SERVER_CAN_SEND_MAIL)
            {
                AccountActivationCodeMailBody accountActivationCodeMailBody = new AccountActivationCodeMailBody();
                MailBuilder mailBuilder = new MailBuilder();
                mailBuilder.setBodyTemplate(accountActivationCodeMailBody);
                mailBuilder.setVariable("activationCode", activationCode);

                // Todo set subject as variable
                mail.sendMail(account.getEmail(), "Your activation code", mailBuilder.buildEmail());

                logger.info("Account: Send activation code to {} for account: {}", account.getEmail(), account.getUsername());
            }
            else
            {
                logger.warn("Warning: Setup the server to send emails!");
                logger.info("Activation code: {} for account: {}", activationCode, account.getUsername());
            }

            responseFactory.newObjectResponse()
                    .command(Commands.RESEND_ACTIVATION_MAIL)
                    .user(user)
                    .execute();
        }
        else
            logger.error("Account: Resend activation code user {} unknown user!", request.getUsername());
    }

    @EzyDoHandle(Commands.FORGOT_PASSWORD)
    public void forgotPassword(EzyUser user, ForgotPasswordRequest request) throws IOException, TemplateException
    {
        logger.info("Account: Receive FORGOT_PASSWORD for user or email-address {}", request.getUsernameOrEMail());

        String resultMessage;

        User account = userService.getUserByUsername(request.getUsernameOrEMail());
        if(account == null)
            account = userService.getUserByEMail(request.getUsernameOrEMail().toLowerCase());

        if (account == null)
        {
            resultMessage = "no_account";

            logger.info("Account: User or email-address {} tried to get a new password but no username or email address found!", request.getUsernameOrEMail());
        }
        else
        {
            String randomstring = RandomString.getAlphaNumericString(8);

            userService.updateStringFieldById(account.getId(), "password", encodePassword(randomstring));

            if(ServerVariables.SERVER_CAN_SEND_MAIL)
            {
                ForgotPasswordMailBody forgotPasswordMailBody = new ForgotPasswordMailBody();
                MailBuilder mailBuilder = new MailBuilder();
                mailBuilder.setBodyTemplate(forgotPasswordMailBody);
                mailBuilder.setVariable("password", randomstring);

                // Todo set subject as variable
                mail.sendMail(account.getEmail(), "Your new password", mailBuilder.buildEmail());

                logger.info("Account: Send new password to {} for account: {}", account.getEmail(), account.getUsername());
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
                .user(user)
                .execute();
    }

    @EzyDoHandle(Commands.FORGOT_USERNAME)
    public void forgotUsername(EzyUser user, ForgotUsernameRequest request) throws IOException, TemplateException
    {
        logger.info("Account: Receive FORGOT_USERNAME for email {}", request.getEmail());

        String resultMessage;

        User account = userService.getUserByEMail(request.getEmail().toLowerCase());

        if(account == null)
        {
            resultMessage = "not_found";
            logger.info("Account: {} tried to get username but no email address found!", request.getEmail());
        }
        else
        {
            String username = account.getUsername();

            if(ServerVariables.SERVER_CAN_SEND_MAIL)
            {
                ForgotUsernameMailBody forgotUsernameMailBody = new ForgotUsernameMailBody();
                MailBuilder mailBuilder = new MailBuilder();
                mailBuilder.setBodyTemplate(forgotUsernameMailBody);
                mailBuilder.setVariable("username", username);

                mail.sendMail(account.getEmail(), "Your Username", mailBuilder.buildEmail());

                logger.info("Account: Send username to {} for account: {}", account.getEmail(), account.getUsername());
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
                .user(user)
                .execute();
    }

    private String encodePassword(String password)
    {
        return EzySHA256.cryptUtfToLowercase(password);
    }
}
