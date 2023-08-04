package com.assambra.app.controller;

import com.assambra.app.constant.Commands;
import com.assambra.app.constant.ServerVariables;
import com.assambra.app.helper.RandomString;
import com.assambra.app.request.CreateAccountRequest;
import com.assambra.app.request.ForgotPasswordRequest;
import com.assambra.app.request.ForgotUsernameRequest;
import com.assambra.app.service.AccountService;
import com.assambra.common.entity.Account;
import com.assambra.common.mail.MailBuilder;
import com.assambra.common.mail.SMTP_EMail;
import com.assambra.common.mail.mailbodys.NewPasswordMailBody;
import com.assambra.common.mail.mailbodys.YourUsernameMailBody;
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
    public void createAccount(EzyUser user, CreateAccountRequest request)
    {
        String resultMessage = "";

        Account account = accountService.getAccountByUsername(request.getUsername());
        if(account == null)
            account = accountService.getAccountByEMail(request.getEmail());

        if(account == null)
        {
            getLogger().info("Account doesn't exist in db, create new one -> E-Mail: {}, Username: {}, Password: {}", request.getEmail(), request.getUsername(), request.getPassword());
            if(!request.getUsername().toLowerCase().contains("guest"))
            {
                accountService.createAccount(request.getEmail().toLowerCase(), request.getUsername(), encodePassword(request.getPassword()));
                resultMessage = "successfully";
            }
            else
            {
                resultMessage = "username_are_not_allowed";
            }
        }
        else
        {
            if(account.getEmail().equals(request.getEmail().toLowerCase()))
            {
                logger.info("E-Mail already registered");
                resultMessage = "email_already_registered";
            }
            else if(account.getUsername().equals(request.getUsername()))
            {
                logger.info("Username already in use");
                resultMessage ="username_already_in_use";
            }
        }

        responseFactory.newObjectResponse()
                .command(Commands.CREATE_ACCOUNT)
                .param("result", resultMessage)
                .user(user)
                .execute();
    }

    @EzyDoHandle(Commands.FORGOT_PASSWORD)
    public void forgotPassword(EzyUser user, ForgotPasswordRequest request) throws IOException, TemplateException
    {
        String password;
        String resultMessage;

        logger.info("Receive forgot password request for user {}, username or email {}", user.getName(), request.getUsernameOrEMail());

        Account account = accountService.getAccountByUsername(request.getUsernameOrEMail());
        if(account == null)
            account = accountService.getAccountByEMail(request.getUsernameOrEMail().toLowerCase());

        if (account == null)
        {
            resultMessage = "no_account";
            password = "";

            logger.info("Forgot password request for user: {}, no username or email address found", user.getName());
        }
        else
        {
            if(ServerVariables.SERVER_CAN_SEND_MAIL)
            {
                logger.info("Forgot password request for account: {}", account.getUsername());

                String randomstring = RandomString.getAlphaNumericString(8);
                logger.info("Create random password {} for account: {}", randomstring, account.getUsername());

                accountService.updateStringFieldById(account.getId(), "password", encodePassword(randomstring));

                NewPasswordMailBody resetPasswordMailBody = new NewPasswordMailBody();
                MailBuilder mailBuilder = new MailBuilder();
                mailBuilder.setBodyTemplate(resetPasswordMailBody);
                mailBuilder.setVariable("password", randomstring);
                mailBuilder.setVariable("username", account.getUsername());

                // Todo set subject as variable
                mail.sendMail(account.getEmail(), "Reset Password", mailBuilder.buildEmail());

                resultMessage ="sending_email";
                password = "";

                logger.info("Forgot password request for user: {}, found account: {}, sending email to: {}",user.getName(), account.getUsername(), account.getEmail());

            }
            else
            {
                logger.info("Forgot password request for account: {}", account.getUsername());

                String randomstring = RandomString.getAlphaNumericString(8);
                logger.info("Create random password {} for account: {}", randomstring, account.getUsername());

                accountService.updateStringFieldById(account.getId(), "password", encodePassword(randomstring));

                resultMessage ="sending_password";
                password = randomstring;
            }
        }

        responseFactory.newObjectResponse()
                .command(Commands.FORGOT_PASSWORD)
                .param("result", resultMessage)
                .param("password", password)
                .user(user)
                .execute();
    }

    @EzyDoHandle(Commands.FORGOT_USERNAME)
    public void forgotUsername(EzyUser user, ForgotUsernameRequest request) throws IOException, TemplateException {
        String resultMessage;
        String username;

        Account account = accountService.getFieldValueByFieldAndValue("email", request.getEmail().toLowerCase(), "username");

        if(account == null)
        {
            resultMessage = "not_found";
            username = "";
        }
        else
        {
            if(ServerVariables.SERVER_CAN_SEND_MAIL)
            {
                resultMessage = "success";
                username = "";

                account = accountService.getAccountByUsername(account.getUsername());

                YourUsernameMailBody yourUsernameMailBody = new YourUsernameMailBody();
                MailBuilder mailBuilder = new MailBuilder();
                mailBuilder.setBodyTemplate(yourUsernameMailBody);
                mailBuilder.setVariable("username", account.getUsername());

                mail.sendMail(account.getEmail(), "Your Username", mailBuilder.buildEmail());
            }
            else
            {
                resultMessage = "success";
                username = account.getUsername();
            }
        }

        responseFactory.newObjectResponse()
                .command(Commands.FORGOT_USERNAME)
                .param("result", resultMessage)
                .param("username", username)
                .user(user)
                .execute();
    }

    private String encodePassword(String password)
    {
        return EzySHA256.cryptUtfToLowercase(password);
    }
}
