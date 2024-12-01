package com.assambra.masterserver.app.controller;

import com.assambra.masterserver.app.config.ServerConfig;
import com.assambra.masterserver.app.request.*;
import com.assambra.masterserver.common.mail.MailBuilder;
import com.assambra.masterserver.common.mail.SMTP_EMail;
import com.assambra.masterserver.app.helper.RandomString;
import com.assambra.masterserver.app.service.UserService;
import com.assambra.masterserver.app.constant.Commands;

import com.assambra.masterserver.common.entity.User;
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
public class UserController extends EzyLoggable {

    private final UserService userService;
    private final EzyResponseFactory responseFactory;
    @EzyAutoBind
    private ServerConfig serverConfig;
    private final SMTP_EMail mail = new SMTP_EMail();


    @EzyDoHandle(Commands.CREATE_USER)
    public void createUser(EzyUser ezyUser, CreateUserRequest request) throws IOException, TemplateException
    {
        logger.info("User: Receive CREATE_USER for new user {}", request.getUsername());

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

                if(serverConfig.getCan_send_mail())
                {
                    UserActivationCodeMailBody userActivationCodeMailBody = new UserActivationCodeMailBody();
                    MailBuilder mailBuilder = new MailBuilder();
                    mailBuilder.setBodyTemplate(userActivationCodeMailBody);
                    mailBuilder.setVariable("activationCode", randomstring);

                    // Todo set subject as variable
                    mail.sendMail(request.getEmail(), "Your activation code", mailBuilder.buildEmail());

                    logger.info("User: Send activation code to {} for user: {}", request.getEmail(), request.getUsername());
                }
                else
                {
                    logger.warn("Warning: Setup the server to send emails!");
                    logger.info("Activation code: {} for user: {}", randomstring, request.getUsername());
                }

                resultMessage = "successful";
                logger.info("User: {} created successfully a new user", request.getUsername());
            }
            else
            {
                resultMessage = "username_are_not_allowed";
                logger.info("User: {} tried to create a new user but username are not allowed!", request.getUsername());
            }
        }
        else
        {
            if(user.getEmail().equals(request.getEmail().toLowerCase()))
            {
                resultMessage = "email_already_registered";
                logger.info("User: {} tried to create a new user but email already registered!", request.getUsername());
            }
            else if(user.getUsername().equals(request.getUsername()))
            {
                resultMessage = "username_already_in_use";
                logger.info("User: {} tried to create a new user but username already in use!", request.getUsername());
            }
        }

        responseFactory.newObjectResponse()
                .command(Commands.CREATE_USER)
                .param("result", resultMessage)
                .user(ezyUser)
                .execute();
    }

    @EzyDoHandle(Commands.ACTIVATE_USER)
    public void activateUser(EzyUser ezyUser, ActivateUserRequest request)
    {
        logger.info("User: Receive ACTIVATE_USER for user {}", request.getUsername());

        boolean activated = userService.activateUser(request.getUsername(), request.getActivationCode());
        String result;

        if(activated)
            result = "successful";
        else
            result = "wrong_activation_code";

        responseFactory.newObjectResponse()
                .command(Commands.ACTIVATE_USER)
                .param("result", result)
                .user(ezyUser)
                .execute();
    }

    @EzyDoHandle(Commands.RESEND_ACTIVATION_MAIL)
    public void resendActivationMail(EzyUser ezyUser, ResendActivationMailRequest request) throws IOException, TemplateException
    {
        logger.info("User: Receive RESEND_ACTIVATION_MAIL for user {}", request.getUsername());

        User user =  userService.getUserByUsername(request.getUsername());

        if(user != null)
        {
            String activationCode = user.getActivationCode();

            if(serverConfig.getCan_send_mail())
            {
                UserActivationCodeMailBody userActivationCodeMailBody = new UserActivationCodeMailBody();
                MailBuilder mailBuilder = new MailBuilder();
                mailBuilder.setBodyTemplate(userActivationCodeMailBody);
                mailBuilder.setVariable("activationCode", activationCode);

                // Todo set subject as variable
                mail.sendMail(user.getEmail(), "Your activation code", mailBuilder.buildEmail());

                logger.info("User: Send activation code to {} for account: {}", user.getEmail(), user.getUsername());
            }
            else
            {
                logger.warn("Warning: Setup the server to send emails!");
                logger.info("Activation code: {} for account: {}", activationCode, user.getUsername());
            }

            responseFactory.newObjectResponse()
                    .command(Commands.RESEND_ACTIVATION_MAIL)
                    .user(ezyUser)
                    .execute();
        }
        else
            logger.error("User: Resend activation code user {} unknown user!", request.getUsername());
    }

    @EzyDoHandle(Commands.FORGOT_PASSWORD)
    public void forgotPassword(EzyUser ezyUser, ForgotPasswordRequest request) throws IOException, TemplateException
    {
        logger.info("User: Receive FORGOT_PASSWORD for user or email-address {}", request.getUsernameOrEMail());

        String resultMessage;

        User user = userService.getUserByUsername(request.getUsernameOrEMail());
        if(user == null)
            user = userService.getUserByEMail(request.getUsernameOrEMail().toLowerCase());

        if (user == null)
        {
            resultMessage = "no_account";

            logger.info("User: User or email-address {} tried to get a new password but no username or email address found!", request.getUsernameOrEMail());
        }
        else
        {
            String randomstring = RandomString.getAlphaNumericString(8);

            userService.updateStringFieldById(user.getId(), "password", encodePassword(randomstring));

            if(serverConfig.getCan_send_mail())
            {
                ForgotPasswordMailBody forgotPasswordMailBody = new ForgotPasswordMailBody();
                MailBuilder mailBuilder = new MailBuilder();
                mailBuilder.setBodyTemplate(forgotPasswordMailBody);
                mailBuilder.setVariable("password", randomstring);

                // Todo set subject as variable
                mail.sendMail(user.getEmail(), "Your new password", mailBuilder.buildEmail());

                logger.info("User: Send new password to {} for user: {}", user.getEmail(), user.getUsername());
            }
            else
            {
                logger.warn("Warning: Setup the server to send emails!");
                logger.info("User: New password: {} for user or e-mail-address: {}", randomstring, request.getUsernameOrEMail());
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
        logger.info("User: Receive FORGOT_USERNAME for email {}", request.getEmail());

        String resultMessage;

        User user = userService.getUserByEMail(request.getEmail().toLowerCase());

        if(user == null)
        {
            resultMessage = "not_found";
            logger.info("User: {} tried to get username but no email address found!", request.getEmail());
        }
        else
        {
            String username = user.getUsername();

            if(serverConfig.getCan_send_mail())
            {
                ForgotUsernameMailBody forgotUsernameMailBody = new ForgotUsernameMailBody();
                MailBuilder mailBuilder = new MailBuilder();
                mailBuilder.setBodyTemplate(forgotUsernameMailBody);
                mailBuilder.setVariable("username", username);

                mail.sendMail(user.getEmail(), "Your Username", mailBuilder.buildEmail());

                logger.info("User: Send username to {} for user: {}", user.getEmail(), user.getUsername());
            }
            else
            {
                logger.warn("Warning: Setup the server to send emails!");
                logger.info("User: Username: {} for e-mail-address: {}", username, request.getEmail());
            }

            resultMessage = "successful";
        }

        responseFactory.newObjectResponse()
                .command(Commands.FORGOT_USERNAME)
                .param("result", resultMessage)
                .user(ezyUser)
                .execute();
    }

    private String encodePassword(String password)
    {
        return EzySHA256.cryptUtfToLowercase(password);
    }
}
