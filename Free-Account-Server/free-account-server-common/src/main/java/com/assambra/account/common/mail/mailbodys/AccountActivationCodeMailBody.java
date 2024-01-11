package com.assambra.account.common.mail.mailbodys;

import com.assambra.account.common.mail.interfaces.MailBody;
import freemarker.template.Configuration;
import freemarker.template.Template;
import freemarker.template.TemplateException;

import java.io.IOException;
import java.io.StringWriter;
import java.util.Map;

public class AccountActivationCodeMailBody implements MailBody {

    private Configuration configuration;
    private Template bodyTemplate;
    private Map<String, Object> dataModel;

    @Override
    public void setConfiguration(Configuration configuration) {
        this.configuration = configuration;
    }

    @Override
    public void loadTemplate() throws IOException {
        bodyTemplate = configuration.getTemplate("account_activation_code_body.ftl");
    }

    @Override
    public String buildBody() throws IOException, TemplateException {
        StringWriter bodyWriter = new StringWriter();
        bodyTemplate.process(dataModel, bodyWriter);
        return bodyWriter.toString();
    }

    @Override
    public void setDataModel(Map<String, Object> dataModel) {
        this.dataModel = dataModel;
    }
}
