package com.assambra.account.common.mail.modes;

import com.assambra.account.common.mail.interfaces.MailMode;
import com.tvd12.ezyfox.util.EzyLoggable;

import javax.mail.Session;
import java.util.Properties;

public class MailModeNoAuthentication extends EzyLoggable implements MailMode
{
    private final String host;


    public MailModeNoAuthentication(String host)
    {
        this.host = host;
    }

    public Session sendMail()
    {
        logger.info("Start MailMode NoAuth");

        Properties mailprops = System.getProperties();

        mailprops.put("mail.smtp.host", host);

        return Session.getInstance(mailprops, null);
    }
}
