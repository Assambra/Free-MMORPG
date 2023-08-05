package com.assambra.common.mail.modes;

import com.assambra.common.mail.interfaces.MailMode;
import com.tvd12.ezyfox.util.EzyLoggable;

import javax.mail.Authenticator;
import javax.mail.PasswordAuthentication;
import javax.mail.Session;
import java.util.Properties;

public class MailModeTLS extends EzyLoggable implements MailMode
{
    private final String host;
    private final String port;
    private final String username;
    private final String password;

    public MailModeTLS(String host,String port,String username, String password)
    {
        this.host = host;
        this.port = port;
        this.username = username;
        this.password = password;
    }

    public Session sendMail()
    {
        logger.info("Start MailMode TLS");
        Properties mailprops = new Properties();

        mailprops.put("mail.smtp.host", host);
        mailprops.put("mail.smtp.port", port);
        mailprops.put("mail.smtp.auth", "true");
        mailprops.put("mail.smtp.starttls.enable", "true");

        Authenticator auth = new Authenticator() {
            protected PasswordAuthentication getPasswordAuthentication()
            {
                return new PasswordAuthentication(username, password);
            }
        };

        return Session.getInstance(mailprops, auth);
    }
}
