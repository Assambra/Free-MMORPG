[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<!-- HEADER -->
<div align="center">
<h1>Free-MMORPG</h1>
    <!--
    <a href="https://github.com/Assambra">
        <img src="Github/Images/Assambra-Logo-512x512.png" alt="Logo" width="80" height="80">
    </a>
    -->
</div>

<!-- LINKS -->
<div align="center">
    <p align="center">
        <a href="https://github.com/Assambra/Free-MMORPG/wiki"><strong>Explore the docs »</strong></a>
    </p>
    <p align="center">
        <a href="https://github.com/Assambra/Free-MMORPG/releases">Get Latest</a>
        ·
        <a href="https://github.com/Assambra/Free-MMORPG/issues">Request Feature</a>
        ·
        <a href="https://github.com/Assambra/Free-MMORPG/issues">Report Bug</a>
    </p>
</div>

<!-- DEMO IMAGE -->
![Our Demo Scene][product-screenshot]

<a name="readme-top"></a>

<!-- TABLE OF CONTENTS -->
<h2>Table of Contents</h2>
<ul>
    <li><a href="#foreword">Foreword</a></li>
    <li><a href="#key-features">Key Features</a></li>
    <li><a href="#playable-demo">Playable Demo</a></li>
    <li><a href="#client">Client</a></li>
        <ul>
            <li><a href="#clone-from-github">Clone Free-MMORPG from Github</a></li>
            <li><a href="#clone-assets-and-projects-from-github">Clone Unity Assets from Github</a></li>
            <ul>
            <li><a href="#uma">UMA</a></li>
                <ul>
                    <li><a href="#get-uma">Get UMA</a></li>
                    <li><a href="#install-uma-package">Install UMA Package</a></li>
                </ul>
            <li><a href="#crest-ocean-system">Crest Ocean System</a></li>
                <ul>
                    <li><a href="#get-crest-ocean-system">Get Crest Ocean System</a></li>
                    <li><a href="#install-crest-ocean-system">Install Crest Ocean System</a></li>
                </ul>
            </ul>
            <li><a href="#needed-unity-packages">Needed Unity packages</a></li>
            <ul>
                <li><a href="#newtonsoft-json">Newtonsoft Json</a></li>
                <li><a href="#color-picker">HSV-Color-Picker-Unity</a></li>
            </ul>
            <li><a href="#unity-asset-store">Assets from the Unity asset store</a></li>
            <ul>
                <li><a href="#mapmagic-2">MapMagic 2</a></li>
            </ul>
        </ul>
    <li><a href="#server">Server</a></li>
        <ul>
            <li><a href="#setup-database">Setup database</a></li>
            <li><a href="#setup-mail">Setup e-mail</a></li>
            <li><a href="#deploy-server">Deploy the server</a></li>
            <ul>
                <li><a href="#download-server">Download the Server</a></li>
                <li><a href="#server-preparations">Server preparations</a></li>
                <li><a href="#export-external-libraries">Export external libraries</a></li>
                <li><a href="#build-the-server">Build the server</a></li>
                <li><a href="#run-the-server">Run the server</a></li>
            </ul>
        </ul>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#credits">Credits</a></li>
    <li><a href="#thank-you">Thank you</a></li>
    <li><a href="#resources-section">Resources Section</a></li>
    <li><a href="#contact">Contact</a></li>
</ul>

<!-- FOREWORD-->
## Foreword

This project utilizes free resources for both server and client technology. Without these, the project likely wouldn't have been possible, so I extend my special thanks to the team at [Young Monkeys](https://youngmonkeys.org/) for their excellent foundational work, particularly for the [EzyFox Server](https://youngmonkeys.org/projects/ezyfox-server) framework, and for sharing it with the community. Also, special thanks to all the contributors to the [UMA](https://github.com/umasteeringgroup/UMA) project and their outstanding Unity Multipurpose Avatar System. Please check out our Resources Section to see what technologies we use and consider supporting their projects in some way, such as through donations or code contributions, to help these talented individuals continue doing what they love: writing great code and developing fantastic frameworks.



For the game client we are using [Unity](https://unity.com "Unity") as game engine.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- KEY FEATURES -->
## Key Features

<ul>
    <li>Account Management</li>
        <ul>
            <li>Create Account</li>
            <li>Login</li>
            <li>Forgot password function, sending new password to a given username / e-mail address</li>
            <li>Forgot username function, sending the username to a given e-mail address</li>
            <li>SSL encryption for creating accounts this is feature from the EzyFox Server framework <a href="https://youngmonkeys.org/ezyfox-server-ssl/">EzyFox Server SSL</a></li>
        </ul>
    <li>Email</li>
        <ul>
            <li>Sending e-mails via smtp, with different authentication protocols SSL, TLS or no authentication</li>
            <li>Easy to create a new custom mails with our interface MailBody</li>
            <li>HTML based email templates, you can also use your own variables</li>
        </ul>
    <li>Character</li>
    <ul>
        <li>Modifiable character model thanks to UMA</li>
        <li>Create a Character and modifying the look with sliders and save on the server</li>
        <li>Colorize character, skin hairs.. (In Progress: change hairs/clothes of the model)</li>
        <li>Character selection with all available characters</li>
    </ul>
    <li>Scene and UI Management, from our <a href="https://github.com/Assambra/Module-GameManager">Module-GameManager</a></li>
    <ul>
        <li>We use one persistent Scene all other scene will be load additive/async the last one will unloaded async.</li>
        <li>For each scene we can create a scene asset (ScriptableObject) that holds a List of SceneUISet also a asset (ScriptableObject)</li>
        <li>A SceneUISet (ScriptableObject) containing all the UIElement prefabs as a Set in an Array that we want to instantiate in the Scene. You can also add multiple sets per scene.</li>
        <li>All Scene UIs will automatically be instantiated for the Scene and last Scene UIs will be destroyed. A check if the next scene have the same SceneUISets, then it don't have to be destroyed or instantiated the UI`s.</li>
    </ul>
    <li>Mouse Handler, from our <a href="https://github.com/Assambra/Module-MouseHandler">Module-MouseHandler</a></li>
    <ul>
        <li>Gives the user a visual feedback what is currently under the mouse cursor and changes the cursor look (2D UI) with Graphic Raycaster</li>
        <li>Enhanced IsOverUIElement - This stores when you press the left mouse button whether you was over a UI element or not and saves the state until you release the mouse button.</li>
    </ul>
    <li>CameraController, from our <a href="https://github.com/Assambra/Module-CameraController">Module-CameraController</a></li>
    <ul>
        <li>At the moment only integrated in the create character process</li>
        <li>The camera can orbit around the character on LMB, same on RMB but turn the target object.</li>
        <li>A lot of options to fine tune the camera behaviour</li>
        <li>Planed: Camera collision, more smoothing of the camera motions, distance and orbit</li>
    </ul>
    <li>Popup System</li>
        <ul>
            <li>Create your own popups for different use-cases</li>
        </ul>
</ul>

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- PLAYABLE DEMO -->
## Playable Demo
We provide a playable Demo there you can play the latest release, and check first what Free-MMORPG can do, or to test and send bug reports.
You can find it here: <a href="https://github.com/Assambra/Free-MMORPG/releases">Get Latest</a>. Only the latest release have a playable demo, provided as rar file. This client will connect to our game server.

<!-- CLIENT -->
## Client
### Clone from Github
Clone the repo:

`git clone git@github.com:Assambra/Free-MMORPG.git`

Get submodules:

`cd Free-MMORPG`

`git submodule update --init --recursive`

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- NEEDED UNITY PACKAGES -->
### Needed Unity packages
#### Newtonsoft Json
The EzyFox Client SDK need Newtonsoft Json package to work.

Edit File: `Free-MMORPG\Free-Client\Packages\manifest.json`

Add a new dependency `"com.unity.nuget.newtonsoft-json": "3.2.1"`. Please read the hint below.

#### Color Picker
To get the HSV-Color-Picker-Unity
Edit File: `Free-MMORPG\Free-Client\Packages\manifest.json`

Add a new dependency `"com.judahperez.hsvcolorpicker": "3.3.0"`
and additionally add to the file.

````
"scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.judahperez.hsvcolorpicker",
        "com.openupm"
      ]
    }
  ]
 ````

`Hint: Don't forget commas after "dependencies" and before "scopedRegistries" -> '},' and line before the new dependency ends with also with comma too.`
<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CLONE ASSETS AND PROJECTS FROM GITHUB -->
## Clone Assets and projects from github

<!-- UMA -->
### UMA
#### Get UMA
Download the latest Pre-release unity asset pack file<a href="https://github.com/umasteeringgroup/UMA/releases/download/v2.13a4/UMA213a4.unitypackage"> UMA213a4.unitypackage</a>

#### Install UMA Package
Add the UMA213a4.unity package to your Free-Client project. To do this, simply move the package to your opened Free-Client project in Unity. Select everything and click Import.

<!-- CREST OCEAN SYSTEM -->
### Crest Ocean System
#### Get Crest Ocean System
Download the latest source code with tag 4.19 as zip file <a href="https://github.com/wave-harmonic/crest/releases/tag/4.19">4.19</a>

#### Install Crest Ocean System
Unzip and drag the second crest folder into unity ./crest-4.19/crest-4.19/crest/Assets/Crest/Crest

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- Unity asset store -->
## Unity Asset Store
### MapMagic 2
Go to the Unity asset store and buy the free asset <a href="https://assetstore.unity.com/packages/tools/terrain/mapmagic-2-165180"> MapMagic2<a/> 
after that in the Unity Editor open the Package Manager Window -> PackageManager (Select Packages: My Assets) download and import MapMagic2 to the project.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- SERVER -->
## Server
We use two servers free account server and free game server.
You only can start and test one of it from your IDE at same time since they both use the same port. 
To test both at the same time, you need to deploy the server. More information can be found in the section <a href="#deploy-server">deploy the server</a>.

<!-- SETUP DATABASE -->
### Setup Database

1. install mongoDB
2. open your mongosh
3. Create your Database 

``use free``

4. Create a new user and password and give it access to the created database 
     
`` db.createUser({user: "root", pwd: "123456",roles: [{role: "readWrite", db:"free" }] })``
 
5. Create two new collections: 

```` 
db.createCollection("account", { collation: { locale: 'en_US', strength: 2 } } ) 
db.account.createIndex( { username: 1 } )
````
````
db.createCollection("character", { collation: { locale: 'en_US', strength: 2 } } )
db.character.createIndex( { name: 1 } )
````
6. Add/Create this files to both of your Server projects:

`Location: Free-Account-Server/free-account-server-common/src/main/resources/free-account-server-common-config.properties`

`Free-Game-Server/free-game-server-common/src/main/resources/free-game-server-common-config.properties`

7. Insert the following values for your database and change it to your needs. 

````
database.mongo.uri=mongodb://root:123456@127.0.0.1:27017/free
database.mongo.database=free
database.mongo.collection.naming.case=UNDERSCORE
database.mongo.collection.naming.ignored_suffix=Entity
````

In this example file we use:

user: root

password 123456

database: free
<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- SETUP MAIL -->
# Setup Mail
Enable or disable server sending mail via SMTP.

## Basic Configuration
If you don't have a mail server or a mail provider that supports sending emails via SMTP, or if you do not wish to send emails (e.g., for local development), locate the following file:

`Location: Free-MMORPG\Free-Server\free-server-app-api\src\main\java\com\assambra\app\constant\ServerVariables`

Change the value to `false`: `SERVER_CAN_SEND_MAIL = true`.

**Important:** When `SERVER_CAN_SEND_MAIL = false` is set, only a log message in your IDE is generated on the server side. This setting is mainly intended for local development. However, it is strongly recommended to complete the email setup for production environments to ensure full functionality. Without email setup, clients will not be able to use features like "Forgot Password", "Forgot Username", and "Account Activation".

## Setup Mail Values
For configuring the email functionality, use the same file you employed for your database settings in the Free-Account-Server. Add or modify the following values as required:

`Location: Free-Account-Server/free-account-server-common/src/main/resources/free-account-server-common-config.properties`

````
mail.host=mail.example.com
mail.port.tls=587
mail.port.ssl=465
mail.authentication=true
mail.username=account@example.com
mail.password=123456
mail.tls=true
mail.ssl=false
mail.mail.address=account@example.com
mail.mail.sender.name=YourCompany Account Management
mail.use.reply.address=false
mail.mail.reply.address=account@example.com
mail.charset=UTF-8
mail.content.type.primary=text/HTML
mail.content.type.sub=charset=UTF-8
mail.format=flowed
mail.content.transfer.encoding=8bit
````

**Hint:** Do not use SSL and TLS simultaneously with both set to `true`. There is no check in place, and only one should be set to `true`.

Most settings should be self-explanatory. If not, refer to the `SMTP_EMail` class and review the comments for each variable.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- DEPLOY SERVER -->
## Deploy Server
### Download Server
- Download the EzyFox Server <a href="https://resources.tvd12.com/ezyfox-server-full-1.2.8.1.zip">ezyfox-server-full-1.2.8.1.zip</a>

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Server preparations
In this example we use the location D:\ezyfox-server
- extract ezyfox-server-full-1.2.8.1.zip and copy the extracted files in the location.
- open the file D:\ezyfox-server\settings\ezy-settings.xml in the editor and add between ```<zones></zones>``` two new zones.
````
<zone>
    <name>free-account-server</name>
    <config-file>free-account-server-zone-settings.xml</config-file>
    <active>true</active>
</zone>
````
````
<zone>
    <name>free-game-server</name>
    <config-file>free-game-server-zone-settings.xml</config-file>
    <active>true</active>
</zone>
````

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Export external Libraries
Steps for Account Server
- Open gitbash and go to the folder free-account-server-startup
- Insert: ``mvn clean install -Denv.EZYFOX_SERVER_HOME=deploy -Pezyfox-deploy``
- Run: ExternalLibrariesExporter in free-account-server-startup/src/test/java/com.assambra.account/tools your IDE
- Insert: D:/ezyfox-server hit enter

Repeat the steps for the Game Server
- Open gitbash and go to the folder free-game-server-startup
- Insert: ``mvn clean install -Denv.EZYFOX_SERVER_HOME=deploy -Pezyfox-deploy``
- Run: ExternalLibrariesExporter in free-game-server-startup/src/test/java/com.assambra.game/tools in your IDE
- Insert: D:/ezyfox-server hit enter

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Build the Server

find line "#export EZYFOX_SERVER_HOME=" remove # and insert after = D:/ezyfox-server in the two following files (please read the hint below)
- Free-MMORPG\Free-Account-Server\build.sh 
- Free-MMORPG\Free-Game-Server\build.sh

In this example i use windows and Git Bash command client go to the project root of:
- Free-MMORPG\Free-Account-Server\
- Free-MMORPG\Free-Game-Server\
- and run command bash build.sh for both of the project.

Hint: If is there some error: 

``cp: cannot create regular file 'D:ezyfox-server/settings/zones/': No such file or directory``

there is some problem with forward slash and backslash depends on your operating system or command client try to use instead of D:\ezyfox-server this D:/ezyfox-server.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Run the server
Go to your server location D:\ezyfox-server

Windows: execute console.bat 

Linux: ``sh ./console.sh`` for console server. 

Linux as Service: ``sh ./start-service.sh``. Stop the service: ``sh ./stop-service.sh``. Restart the service: ``sh ./restart-service.sh``

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!--Resources Section-->

<!--Contributing-->
## Contributing

Thank you for your interest in our project! We welcome contributions in various forms, whether it's through code contributions, reporting bugs, submitting feature requests, or spreading the word to your friends and colleagues. Your contribution is invaluable and helps us continuously improve our project.

### Code Contributions

If you have experience in programming and would like to contribute to the development of our project, pull requests are always welcome! Please follow these steps:

1. Fork the repository on GitHub.
2. Create a new branch for your changes.
3. Implement your changes and thoroughly test them.
4. Submit a pull request to our repository.

We will review your pull request and collaborate with you to ensure your changes are properly integrated.

### Reporting Bugs

If you come across a bug in our project, please don't hesitate to report it. Use our GitHub issue tracking system for this purpose. Be sure to provide as many details as possible to assist us in troubleshooting.

### Feature Requests

Do you have a great idea for a new feature? Share it with us! Simply create an issue on GitHub and describe your idea in detail. We'll review and discuss your request.

### Try the Playable Demo

Try out our <a href="https://github.com/Assambra/Free-MMORPG/releases/download/0.37.1/Free-Client-Live-0.37.1.rar">playable demo</a> to experience our project in action. We appreciate your feedback and impressions.

### Spread the Word

An easy yet effective way to support us is by sharing our project with your friends, colleagues, and on social media. The more people learn about our work, the better!

### Wiki and Documentation

Our Wiki page and documentation are places where you can share your knowledge and expertise. If you'd like to make improvements, please feel free to do so. Your contributions are valuable to the entire community.

Thank you for your support and interest in our project! Together, we can make it even better.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Resources Section
- For the server/client, we use the [EzyFox Server Framework](https://youngmonkeys.org/projects/ezyfox-server) from [Young Monkeys](https://youngmonkeys.org/) you can support there great project. For more information visit https://youngmonkeys.org/donate/

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- Credits -->
## Credits
We use Textures created by João Paulo https://3dtextures.me he published it under the CC0 license. You can buy him a <a href="https://ko-fi.com/katsukagi">Ko-fi</a> or support him as a <a href="https://www.patreon.com/gendo">patreon</a>.  

<!-- Thank you -->
## Thank you
- A special thanks to tvd12 for fixing a variety of bugs and providing great help with other issues.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- Contact -->
## Contact
Join us on <a href="https://discord.gg/qyCdkYSWVG">Discord</a>
<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- DOCUMENT VARIABLE-->
[contributors-shield]: https://img.shields.io/github/contributors/Assambra/Free-MMORPG.svg?style=for-the-badge
[contributors-url]: https://github.com/Assambra/Free-MMORPG/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Assambra/Free-MMORPG.svg?style=for-the-badge
[forks-url]: https://github.com/Assambra/Free-MMORPG/network/members
[stars-shield]: https://img.shields.io/github/stars/Assambra/Free-MMORPG.svg?style=for-the-badge
[stars-url]: https://github.com/Assambra/Free-MMORPG/stargazers
[issues-shield]: https://img.shields.io/github/issues/Assambra/Free-MMORPG.svg?style=for-the-badge
[issues-url]: https://github.com/Assambra/Free-MMORPG/issues
[license-shield]: https://img.shields.io/github/license/Assambra/Free-MMORPG.svg?style=for-the-badge
[license-url]: https://github.com/Assambra/Free-MMORPG/blob/main/LICENSE
[product-screenshot]: Github/Images/Free-MMORPG-Demo-Image.v0.19.0.jpg
[Unity-url]: https://www.unity.com
[Unity.com]: https://img.shields.io/badge/Unity-000000.svg?style=for-the-badge&logo=unity&logoColor=white