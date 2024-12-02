package com.assambra.masterserver.common.mail;

import com.assambra.masterserver.common.mail.interfaces.MailBody;
import freemarker.template.Configuration;
import freemarker.template.Template;
import freemarker.template.TemplateException;

import java.io.IOException;
import java.io.StringWriter;
import java.util.HashMap;
import java.util.Map;

public class MailBuilder {

    private Configuration configuration;
    private Map<String, Object> dataModel;
    private Template headerTemplate;
    private MailBody bodyTemplate;
    private Template footerTemplate;

    public MailBuilder() throws IOException {
        configuration = new Configuration(Configuration.VERSION_2_3_32);
        configuration.setClassForTemplateLoading(MailBuilder.class, "/mail-templates");

        headerTemplate = configuration.getTemplate("header_template.ftl");
        footerTemplate = configuration.getTemplate("footer_template.ftl");

        dataModel = new HashMap<>();
    }

    public void setBodyTemplate(MailBody bodyTemplate) {
        this.bodyTemplate = bodyTemplate;
    }

    public void setVariable(String name, Object value) {
        dataModel.put(name, value);
    }

    public String buildEmail() throws IOException, TemplateException {
        StringWriter writer = new StringWriter();

        // hardcoded mail variables
        dataModel.put("title","Your new password");
        dataModel.put("companyName","Assambra");

        headerTemplate.process(dataModel, writer);

        bodyTemplate.setConfiguration(configuration);
        bodyTemplate.loadTemplate();
        bodyTemplate.setDataModel(dataModel);

        writer.append(bodyTemplate.buildBody());
        footerTemplate.process(dataModel, writer);

        return writer.toString();
    }
}
