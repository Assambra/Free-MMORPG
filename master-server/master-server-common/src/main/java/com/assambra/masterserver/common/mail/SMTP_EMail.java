package com.assambra.masterserver.common.mail;

import com.assambra.masterserver.common.mail.interfaces.MailMode;
import com.assambra.masterserver.common.mail.modes.MailModeNoAuthentication;
import com.assambra.masterserver.common.mail.modes.MailModeSSL;
import com.assambra.masterserver.common.mail.modes.MailModeTLS;
import com.tvd12.ezyfox.stream.EzyAnywayInputStreamLoader;
import com.tvd12.ezyfox.util.EzyLoggable;

import javax.mail.Message;
import javax.mail.Session;
import javax.mail.Transport;
import javax.mail.internet.InternetAddress;
import javax.mail.internet.MimeMessage;
import java.io.IOException;
import java.io.InputStream;
import java.util.Date;
import java.util.Properties;

public class SMTP_EMail extends EzyLoggable
{
    private final Properties properties = new Properties();
    private InputStream inputStream;
    private MailMode mailMode;

    //region <E-MAIL SETTINGS>

    // To change some of this values do this in resources/free-server-common-config.properties
    private String host; // e.g mail.example.com, smtp.example.com
    private String porttls; // tls = 587, ssl 465
    private String portssl; // tls = 587, ssl 465
    private String auth; // true = server need authentication, false = no authentication
    private String tls; // true = using tls // set port to 587, false disable tls
    private String ssl; // true = using ssl // set port to 465, false disable ssl
    private String emailaddress; // e-mail address we're sending mails from
    private String sendername; // e.g Company Account Management
    private String password; // smtp auth password
    private String username; // smtp auth user in most case the the e-mail address
    private String usereplayaddress; // true to use a replay e-mail address
    private String mailreplayaddress; // the e-mail address the user sending a replayed/answer e-mail

    // E-Mail, Header and other settings leave default
    private String charset;
    private String contenttypeprimary;
    private String contenttypesub;
    private String format;
    private String contenttransferencoding;

    //endregion

    public SMTP_EMail()
    {
        getMailProperties();
        if(auth.contains("true"))
        {
            if(tls.contains("true"))
                mailMode = new MailModeTLS(host, porttls, username, password);
            if(ssl.contains("true"))
                mailMode = new MailModeSSL(host, portssl, username, password);
        }
        else
            mailMode = new MailModeNoAuthentication(host);
    }

    private void getMailProperties()
    {
        try(InputStream inputStream = EzyAnywayInputStreamLoader.builder()
                .context(getClass())
                .build()
                .load("master-server-common-config.properties")
        )
        {
            properties.load(inputStream);

            host = properties.getProperty("mail.host");
            porttls = properties.getProperty("mail.port.tls");
            portssl = properties.getProperty("mail.port.ssl");
            auth = properties.getProperty("mail.authentication");
            username = properties.getProperty("mail.username");
            password = properties.getProperty("mail.password");
            ssl = properties.getProperty("mail.ssl");
            tls = properties.getProperty("mail.tls");
            emailaddress = properties.getProperty("mail.mail.address");
            sendername = properties.getProperty("mail.mail.sender.name");
            usereplayaddress = properties.getProperty("mail.use.replay.address");
            mailreplayaddress = properties.getProperty("mail.mail.replay.address");

            charset = properties.getProperty("mail.charset");
            contenttypeprimary = properties.getProperty("mail.content.type.primary");
            contenttypesub = properties.getProperty("mail.content.type.sub");
            format = properties.getProperty("mail.format");
            contenttransferencoding = properties.getProperty("mail.content.transfer.encoding");
        }
        catch (IOException e)
        {
            e.printStackTrace();
        }
    }

    private void send(Session session, String recipient, String subject, String mailbody)
    {
        try
        {
            MimeMessage message = new MimeMessage(session);

            message.addHeader("Content-type", contenttypeprimary + "; " + contenttypesub);
            message.addHeader("format", format);
            message.addHeader("Content-Transfer-Encoding", contenttransferencoding);

            message.setFrom(new InternetAddress(emailaddress, sendername));

            if(usereplayaddress.equals("true"))
                message.setReplyTo(InternetAddress.parse(mailreplayaddress, false));

            message.setSubject(subject, charset);

            message.setContent(mailbody,contenttypeprimary+"; " + contenttypesub);
            message.setSentDate(new Date());

            message.setRecipient(Message.RecipientType.TO, new InternetAddress(recipient));

            logger.info("Email message is ready");

            Transport.send(message);
            logger.info("Email sent successfully");
        }
        catch (Exception e)
        {
            e.printStackTrace();
        }
    }

    public void sendMail(String recipient, String subject, String mailbody)
    {
        send(mailMode.sendMail(), recipient, subject, mailbody);
    }
}


