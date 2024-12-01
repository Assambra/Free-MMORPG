package com.assambra.masterserver.common.config;

import com.tvd12.ezyfox.bean.annotation.EzyPropertiesBean;
import com.tvd12.ezyfox.util.EzyLoggable;
import lombok.Data;

@Data
@EzyPropertiesBean(prefix = "server")
public class ServerConfig extends EzyLoggable {

    private Boolean can_send_mail;
    private String server_executable_path;
}
