
# free-game-server

# Require

1. Java 8 or higher
2. Apache Maven 3.3+

# Description

1. free-game-server-app-api: contains app's request controllers, app's event handlers and components that are related to
   app
2. free-game-server-app-entry: contains AppEntryLoader class (you should not add any classes in this module)
3. free-game-server-app-entry/config/config.properties: app's configuration file
4. free-game-server-common: contains components that are used by both app and plugin
5. free-game-server-plugin: contains plugin's event handlers, plugin's request controllers and components that are related
   to plugin. You will need handle `USER_LOGIN` event here
6. free-game-server-plugin/config/config.properties: plugin's configuration file
7. free-game-server-startup: contains ApplicationStartup class to run on local (you should not add any classes in this
   module)
8. free-game-server-startup/src/main/resources/logback.xml: logback configuration file

# How to build?

You can build by:

1. Running `mvn clean install` on your terminal
2. Opening `build.sh` file and set `EZYFOX_SERVER_HOME` by your `ezyfox-server` folder path and run `bash build.sh` file
   on your terminal

# How to run?

## Run on your IDE

You just move to `free-game-server-startup` module and run `ApplicationStartup`

## Run by ezyfox-server

To run by `ezyfox-server` you need to follow by steps:

1. Download [ezyfox-sever](https://resources.tvd12.com/) (the standard version is for IoT servers and the full version
   is for normal servers)
2. Open `build.sh` file and set `EZYFOX_SERVER_HOME` variable, let's say you place `ezyfox-server`
   at `/Programs/ezyfox-server` so `EZYFOX_SERVER_HOME=/Programs/ezyfox-server`
3. Run `build.sh` file on your terminal
4. Open file `EZYFOX_SERVER_HOME/settings/ezy-settings.xml` and add to `<zones>` tag:

```xml
<zone>
	<name>free-game-server</name>
	<config-file>free-game-server-zone-settings.xml</config-file>
	<active>true</active>
</zone>
```

5. Run `console.sh` in `EZYFOX_SERVER_HOME` on your terminal, if you want to run `ezyfox-server` in backgroud you will
   need to run `start-server.sh` on your terminal

## Run without ezyfox-server

To run without `ezyfox-server` you need follow by steps:

1. Run `bash export.sh` command
2. Move to `free-game-server-startup/deploy` folder

### On Windows

You just need run `console.bat`

### On Linux

1. To run to debug, you just need run `bash console.sh` on your terminal
2. To run in background, you just need run `bash start-service.sh` on your terminal
3. To stop your service, you just need run `bash stop-service.sh` on your terminal

## Run with specific configuration profile

You can [read this guide](https://youngmonkeys.org/ezyfox-server-project-configuration/) to know how to
run `ezyfox-server` or your application with a specific profile

# How to deploy?

## Deploy mapping

Modules which are deployed to ezyfox-server will be mapped as follows::

1. free-game-server-app-api => `ezyfox-server/apps/common/free-game-server-app-api-0.13.0.jar`
2. free-game-server-app-entry => `ezyfox-server/apps/entries/free-game-server-app`
3. free-game-server-common => `ezyfox-server/common/ free-game-server-common-0.13.0.jar`
4. free-game-server-plugin => `ezyfox-server/plugins/free-game-server-plugin`

## Deploy with tools

You can use bellow tools to copy jar files (follow by above mapping)

1. [filezilla](https://filezilla-project.org/)
2. [transmit](https://panic.com/transmit/)

## Deploy with scp

We've already prepared for you `deploy.sh` file, you just need:

1. Open `deploy.sh` file
2. Set `ezyfoxServerLocal` by your `ezyfox-server` folder path on local
3. Set `ezyfoxServerRemote` by your `ezyfox-server` folder path on remote
4. Set `sshCredential` by your ssh credential, i.e `root@your_host.com`
5. Run `bash deploy.sh` command
6. After the deployment is done, you need to open `settings/ezy-settings.xml` file in `ezyfox-server` on remote and
   add (if you have already done this step in the past, please skip it):

```xml
<zone>
	<name>free-game-server</name>
	<config-file>free-game-server-zone-settings.xml</config-file>
	<active>true</active>
</zone>
```

## Deploy without ezyfox-server

You just need use tool or `scp` to copy `free-game-server-startup/deploy` to your server

# How to test?

On your IDE, you need:

1. Move to `free-game-server-startup` module
2. Run `ApplicationStartup` in `src/main/java`
3. Run `ClientTest` in `src/test/java`

# How to deploy?

You can take a look this guide: [Deploy EzyFox Server](https://youngmonkeys.org/deploy-ezyfox-server/)

# How to export external libraries to ezyfox server?

1. Move to `free-game-server-startup` module 
2. Run `mvn clean install -Denv.EZYFOX_SERVER_HOME=deploy -Pezyfox-deploy`
3. Run class `com.assambra.account.tools.ExternalLibrariesExporter` in `free-game-server-startup/src/test/java`

# Documentation

You can find a lot of documents on [youngmonkeys.org](https://youngmonkeys.org/ezyfox-sever/)

# Contact us

- Touch us on [Facebook](https://www.facebook.com/youngmonkeys.org)
- Ask us on [stackask.com](https://stackask.com)

# Help us by donation

Currently, our operating budget is depending on our salary, every effort still based on voluntary contributions from a
few members of the organization. But with a low budget like that, it causes many difficulties for us. With big plans and
results being intellectual products for the community, we hope to receive your support to take further steps. Thank you
very much.

[https://youngmonkeys.org/donate/](https://youngmonkeys.org/donate/)

# License

- Apache License, Version 2.0
