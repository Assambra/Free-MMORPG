﻿using System;
using System.Collections.Generic;
using com.tvd12.ezyfoxserver.client.evt;
using com.tvd12.ezyfoxserver.client.handler;
using com.tvd12.ezyfoxserver.client.manager;

namespace com.tvd12.ezyfoxserver.client.setup
{
    public class EzySimpleSetup : EzySetup
    {
        protected readonly EzyHandlerManager handlerManager;
        protected readonly IDictionary<String, EzyAppSetup> appSetups;
        protected readonly IDictionary<String, EzyPluginSetup> pluginSetups;

        public EzySimpleSetup(EzyHandlerManager handlerManager)
        {
            this.handlerManager = handlerManager;
            this.appSetups = new Dictionary<String, EzyAppSetup>();
            this.pluginSetups = new Dictionary<String, EzyPluginSetup>();
        }

        public EzySetup addDataHandler(Object cmd, EzyDataHandler dataHandler)
        {
            handlerManager.addDataHandler(cmd, dataHandler);
            return this;
        }

        public EzySetup addEventHandler(EzyEventType eventType, EzyEventHandler eventHandler)
        {
            handlerManager.addEventHandler(eventType, eventHandler);
            return this;
        }

        public EzyAppSetup setupApp(String appName)
        {
            EzyAppSetup appSetup = null;
            if (appSetups.ContainsKey(appName))
            {
                appSetup = appSetups[appName];
            }
            else
            {
                EzyAppDataHandlers dataHandlers = handlerManager.getAppDataHandlers(appName);
                appSetup = new EzySimpleAppSetup(dataHandlers, this);
                appSetups[appName] = appSetup;
            }
            return appSetup;
        }

        public EzyPluginSetup setupPlugin(String pluginName)
        {
            EzyPluginSetup pluginSetup = null;
            if (appSetups.ContainsKey(pluginName))
            {
                pluginSetup = pluginSetups[pluginName];
            }
            else
            {
                EzyPluginDataHandlers dataHandlers = handlerManager.getPluginDataHandlers(pluginName);
                pluginSetup = new EzySimplePluginSetup(dataHandlers, this);
                pluginSetups[pluginName] = pluginSetup;
            }
            return pluginSetup;
        }
    }
}

