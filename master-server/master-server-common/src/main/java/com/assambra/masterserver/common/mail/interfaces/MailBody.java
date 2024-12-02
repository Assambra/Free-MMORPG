package com.assambra.masterserver.common.mail.interfaces;

import freemarker.template.Configuration;
import freemarker.template.TemplateException;

import java.io.IOException;
import java.util.Map;

public interface MailBody {

    void setConfiguration(Configuration configuration);
    void loadTemplate()throws IOException;
    String buildBody() throws IOException, TemplateException;
    void setDataModel(Map<String, Object> dataModel);
}
