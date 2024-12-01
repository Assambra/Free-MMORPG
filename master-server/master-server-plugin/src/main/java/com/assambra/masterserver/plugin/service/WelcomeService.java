package com.assambra.masterserver.plugin.service;

import com.assambra.masterserver.common.service.CommonService;
import com.assambra.masterserver.plugin.config.PluginConfig;

import com.tvd12.ezyfox.bean.annotation.EzyAutoBind;
import com.tvd12.ezyfox.bean.annotation.EzySingleton;

@EzySingleton
public class WelcomeService {

    @EzyAutoBind
    private PluginConfig config;

    @EzyAutoBind
    private CommonService commonService;

    public String welcome(String username) {
        return config.getWelcomePrefix() + " " + username + "!";
    }
}
