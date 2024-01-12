package com.assambra.account.app.controller;

import com.assambra.account.app.request.ActivateAccountRequest;
import com.assambra.account.common.mail.MailBuilder;
import com.assambra.account.common.mail.SMTP_EMail;
import com.assambra.account.app.helper.RandomString;
import com.assambra.account.app.request.CreateAccountRequest;
import com.assambra.account.app.request.ForgotPasswordRequest;
import com.assambra.account.app.request.ForgotUsernameRequest;
import com.assambra.account.app.service.AccountService;
import com.assambra.account.app.constant.Commands;
import com.assambra.account.app.constant.ServerVariables;
import com.assambra.account.common.entity.Account;
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
public class AccountController extends EzyLoggable {

    private final AccountService accountService;
    private final EzyResponseFactory responseFactory;
    private final SMTP_EMail mail = new SMTP_EMail();

    @EzyDoHandle(Commands.CREATE_ACCOUNT)
    public void createAccount(EzyUser user, CreateAccountRequest request) throws IOException, TemplateException
    {
        logger.info("Account: Receive CREATE_ACCOUNT for new user {}", request.getUsername());

        String resultMessage = "";

        Account account = accountService.getAccountByUsername(request.getUsername());
        if(account == null)
            account = accountService.getAccountByEMail(request.getEmail());

        if(account == null)
        {
            if(!request.getUsername().toLowerCase().contains("guest"))
            {
                String randomstring = RandomString.getAlphaNumericString(8);

                accountService.createAccount(request.getEmail().toLowerCase(), request.getUsername(), encodePassword(request.getPassword()), randomstring);

                if(ServerVariables.SERVER_CAN_SEND_MAIL)
                {
                    AccountActivationCodeMailBody accountActivationCodeMailBody = new AccountActivationCodeMailBody();
                    MailBuilder mailBuilder = new MailBuilder();
                    mailBuilder.setBodyTemplate(accountActivationCodeMailBody);
                    mailBuilder.setVariable("activationCode", randomstring);

                    // Todo set subject as variable
                    mail.sendMail(account.getEmail(), "Your activation code", mailBuilder.buildEmail());

                    logger.info("Account: Send activation code to {} for account: {}", request.getEmail(), request.getUsername());
                }
                else
                {
                    logger.warn("Warning: Setup the server to send emails!");
                    logger.info("Activation code: {} for account: {}", randomstring, request.getUsername());
                }

                resultMessage = "successful";
                logger.info("Account: {} created successfully a new account", request.getUsername());
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
                logger.info("Account: {} tried to create a new account but email already registered!", request.getUsername());
            }
            else if(account.getUsername().equals(request.getUsername()))
            {
                resultMessage ="username_already_in_use";
                logger.info("Account: {} tried to create a new account but username already in use!", request.getUsername());
            }
        }

        responseFactory.newObjectResponse()
                .command(Commands.CREATE_ACCOUNT)
                .param("result", resultMessage)
                .user(user)
                .execute();
    }

    @EzyDoHandle(Commands.ACTIVATE_ACCOUNT)
    public void activateAccount(EzyUser user, ActivateAccountRequest request)
    {
        boolean activated = accountService.activateAccount(request.getUsername(), request.getActivationCode());
        String result;

        if(activated)
            result = "successful";
        else
            result = "wrong_activation_code";

        responseFactory.newObjectResponse()
                .command(Commands.ACTIVATE_ACCOUNT)
                .param("result", result)
                .user(user)
                .execute();
    }

    @EzyDoHandle(Commands.FORGOT_PASSWORD)
    public void forgotPassword(EzyUser user, ForgotPasswordRequest request) throws IOException, TemplateException
    {
        logger.info("Account: Receive FORGOT_PASSWORD for user or email-address {}", request.getUsernameOrEMail());

        String resultMessage;

        Account account = accountService.getAccountByUsername(request.getUsernameOrEMail());
        if(account == null)
            account = accountService.getAccountByEMail(request.getUsernameOrEMail().toLowerCase());

        if (account == null)
        {
            resultMessage = "no_account";

            logger.info("Account: User or email-address {} tried to get a new password but no username or email address found!", request.getUsernameOrEMail());
        }
        else
        {
            String randomstring = RandomString.getAlphaNumericString(8);

            accountService.updateStringFieldById(account.getId(), "password", encodePassword(randomstring));

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

        Account account = accountService.getFieldValueByFieldAndValue("email", request.getEmail().toLowerCase(), "username");

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
