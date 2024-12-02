package com.assambra.masterserver.plugin;

import com.assambra.masterserver.common.constant.CommonConstants;

import com.tvd12.ezyfox.bean.EzyBeanContextBuilder;
import com.tvd12.ezyfoxserver.context.EzyPluginContext;
import com.tvd12.ezyfoxserver.context.EzyZoneContext;
import com.tvd12.ezyfoxserver.setting.EzyPluginSetting;
import com.tvd12.ezyfoxserver.support.entry.EzyDefaultPluginEntry;

import java.util.Properties;

public class PluginEntry extends EzyDefaultPluginEntry {

    @Override
    protected void preConfig(EzyPluginContext ctx) {
        logger.info("\n=================== master-server PLUGIN START CONFIG ================\n");
    }

    @Override
    protected void postConfig(EzyPluginContext ctx) {
        logger.info("\n=================== master-server PLUGIN END CONFIG ================\n");
    }

    @Override
    protected void setupBeanContext(EzyPluginContext context, EzyBeanContextBuilder builder) {
        EzyPluginSetting setting = context.getPlugin().getSetting();
        builder.addProperties("master-server-common-config.properties");
        builder.addProperties(getConfigFile(setting));

        Properties properties = builder.getProperties();
        EzyZoneContext zoneContext = context.getParent();
        zoneContext.setProperty(CommonConstants.PLUGIN_PROPERTIES, properties);
    }

    protected String getConfigFile(EzyPluginSetting setting) {
        return setting.getConfigFile();
    }

    @Override
    protected String[] getScanablePackages() {
        return new String[]{
            "com.assambra.masterserver.common",
            "com.assambra.masterserver.plugin"
        };
    }
}
