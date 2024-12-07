[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

# Free-MMORPG

<div align="center">
    <a href="https://github.com/Assambra">
        <img src="Github/Images/Assambra-Logo-512x512.png" alt="Logo" width="80" height="80">
    </a>
</div>

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

![Free-MMORPG screenshot][product-screenshot]

<a name="readme-top"></a>

---

<details><summary>Table of Contents</summary>
    <ul>
        <li><a href="#foreword">Foreword</a></li>
        <li><a href="#key-features">Key Features</a></li>
        <li><a href="#playable-demo">Playable Demo</a></li>
        <li><a href="#getting-started">Getting Started</a></li>
            <ul>
                <li><a href="#get-the-project-from-github">Get the project from github</li>
                <li><a href="#download-and-import-ezyfox-server-csharp-client-unity">Download and Import EzyFox Server CSharp Client Unity</a></li>
                <li><a href="#needed-projects-from-github">Needed projects from GitHub</a></li>
                    <ul>
                        <li><a href="#crest-ocean-system">Crest Ocean System</a></li>
                    </ul>
                <li><a href="#needed-unity-packages-from-the-unity-asset-store">Needed Unity packages from the Unity asset store</a></li>
                    <ul>
                        <li><a href="#mapmagic-2">MapMagic 2</a></li>
                        <li><a href="#uma-2">UMA 2</a></li>
                    </ul>
                <li><a href="#free-server">Free-Server</a></li>
                    <ul>
                        <li><a href="#build-the-free-server">Build the Free-Server</a></li>
                        <li><a href="#setup-server-executables-path">Setup server executables path</a></li>
                    </ul>
                <li><a href="#master-server">Master-Server</a></li>
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
            </ul>
        <li><a href="#play-the-client">Play the client</a></li>
        <li><a href="#contributing">Contributing</a></li>
        <li><a href="#resources-section">Resources Section</a></li>
        <li><a href="#credits">Credits</a></li>
        <li><a href="#thank-you">Thank you</a></li>
        <li><a href="#contact">Contact</a></li>
    </ul>
</details>

---

## Foreword

This project utilizes free resources for both server and client technology. Without these, the project likely wouldn't have been possible, so I extend my special thanks to the team at [Young Monkeys](https://youngmonkeys.org/) for their excellent foundational work, particularly for the [EzyFox Server](https://youngmonkeys.org/projects/ezyfox-server) framework, and for sharing it with the community. Also, special thanks to all the contributors to the [UMA](https://github.com/umasteeringgroup/UMA) project and their outstanding Unity Multipurpose Avatar System. For the ocean, we use the [Crest Ocean System](https://github.com/wave-harmonic/crest), which, in my personal opinion, is the best available ocean system for Unity. Please check out our Resources Section to see what technologies we use and consider supporting their projects in some way, such as through donations or code contributions, to help these talented individuals continue doing what they love: writing great code and developing fantastic frameworks.

For the game client we are using [Unity](https://unity.com "Unity") as game engine.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Key Features

<ul>
    <li>Area of Interest System on the Unity-Server</li>
    <ul>
        <li>Capability to send spawn or despawn commands to the Unity Client player based on range</li>
        <li>Each entity has a NearbyPlayer list and sends position updates only to players in this list</li>
    </ul>
    <li>Entity-based System</li>
    <ul>
        <li>An entity is the base and can represent a player, effect, or building (currently, only Player is implemented)</li>
        <li>Each entity on the Unity Client has a NetworkTransform component to receive position updates</li>
        <li>If the Unity-Server sends a spawn or despawn command to the player (Client), the entity will be automatically added to or removed from the client entity list and instantiated or destroyed accordingly</li>
    </ul>
    <li>Master Server</li>
        <ul>
            <li>Extended EzyFox Server capabilities to spawn Unity-based server instances for each room</li>
            <li>Unified Unity-Server project for managing all room-specific game logic</li>
            <li>Consistent use of Unity Engine for both client and server to streamline development</li>
            <li>Centralized user management, database handling, and request forwarding via EzyFox Server</li>
            <li>Seamless communication flow between Unity-Client, Master-Server, and Unity-Servers</li>
        </ul>
    <li>Account Management</li>
        <ul>
            <li>Create a new account</li>
            <li>Account activation, sending activation code via email, login checks if account activated if not needed to insert the code, resend code possible.</li>
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
    <li>Character Creation</li>
    <ul>
        <li>Creating a new character</li>
        <li>Modifiable character model thanks to UMA, modifying the look of the character with sliders</li>
        <li>Colorize character, skin, hairs underwear.</li>
        <li>Change Hair, Beard Eyebrows, Underwear</li>
        <li>Save the character server/database</li>
    </ul>
    <li>Character Selection</li>
    <ul>
        <li>Character selection with all available characters</li>
        <li>(Todo) Delete a Character</li>
    </ul>
    <li>Character Movement</li>
    <ul>
        <li>Server Authoritative, movement (no client side prediction, server reconciliation or entity interpolation, simple send input compute position on the server send back to client)</li>
        <li>Simple but working animation based by the position the player getting from the server</li>
        <li>Server: Read from unity exported heightmaps format: .raw file, 16-bit, byte order: windows to compute the players Y position</li>
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
        <li>The camera can orbit around the character on LMB, same on RMB but turn the target object.</li>
        <li>A lot of options to fine tune the camera behaviour</li>
        <li>Planed: Camera collision, more smoothing of the camera motions, distance and orbit</li>
    </ul>
    <li>Popup System</li>
        <ul>
            <li>Create your own popups for different use-cases</li>
        </ul>
    <li>World Scene</li>
    <ul>
        <li>Nice looking Island surrounded by an ocean</li>
    </ul>
</ul>

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Playable Demo

~~We provide a playable demo where you can play the latest release, check what Free-MMORPG can do, or test and send bug reports.
You can find it here: <a href="https://github.com/Assambra/Free-MMORPG/releases">Get Latest</a>. Only the latest release has a playable demo, provided as a `.rar` file. This client will connect to our game server.~~

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Getting Started
### Get the project from github

Clone the repository:

```bash
git clone git@github.com:Assambra/Free-MMORPG.git
```

---

### Download and import EzyFox Server CSharp Client Unity

Download and extract the following file: [ezyfox-server-csharp-client-1.1.6-unity.zip](https://github.com/youngmonkeys/ezyfox-server-csharp-client/archive/refs/tags/v1.1.6-unity.zip).

Perform the following steps for both Unity projects: **Free-Client** and **Free-Server**:
1. Open the project in the Unity Editor.
2. Select all folders and files from the extracted folder `ezyfox-server-csharp-client-1.1.6-unity/ezyfox-server-csharp-client-1.1.6-unity` and drag them into the folder `/Assets/Plugins/ezyfox-server-csharp-client-1.1.6-unity/` of your opened Unity project.

---

### Needed projects from GitHub

#### Crest Ocean System

1. Get Crest Ocean System
Download and extract the following file: <a href="https://github.com/wave-harmonic/crest/archive/refs/tags/4.21.3.zip">crest-4.21.3.zip</a>.

1. Install Crest Ocean System
3. Open the Unity-Client project in the Unity Editor.
Unzip and select all folders and filesthe second crest folder into Unity: `crest-4.21.3/crest-4.21.3/crest/Assets/Crest/Crest`.
4. Select all folders and files from the extracted folder `crest-4.21.3/crest-4.21.3/crest/Assets/Crest/Crest/` and drag them into the folder `/Assets/Crest/` of your opened Unity project.

---

### Needed Unity packages from the Unity Asset Store

#### MapMagic 2
Go to the Unity Asset Store and get the free asset: <a href="https://assetstore.unity.com/packages/tools/terrain/mapmagic-2-165180">MapMagic 2</a>.

After acquiring the asset:
1. Open both projects (**Free-Client** and **Free-Server**) in the Unity Editor.
2. Open the **Package Manager** (`Window -> Package Manager`) and select `Packages: My Assets`.
3. Download and import **MapMagic 2** into both projects.

#### UMA 2
Please use our modified version of UMA: <a href="https://drive.google.com/drive/folders/1bZsXNPgpA5pMtIq1rkJhNSJeAs05XDxg?usp=sharing">UMA 2.13 Beta 1-modified.rar</a>.

Steps to use it:
1. Download and extract the `.rar` file.
2. Drag the folder `UMA` directly into `Free-Client/Assets` in the Unity Editor.

~~Alternatively, get the free asset from the Unity Asset Store: <a href="https://assetstore.unity.com/packages/3d/characters/uma-2-35611">UMA 2</a>.~~

~~After importing UMA 2 into the project, you must add Assembly Definition References:~~
1. ~~Navigate to `Assets/UMA/Core/UMA_Core`.~~
2. ~~In the Inspector, add references for `Unity.Mathematics`, `Unity.Burst`, and `Unity.Collections`.~~

---

### Free-Server

#### Build the Free-Server

Open the Unity-Server project in Unity. Open the Build Settings: (`File -> Build Settings`). The master server can handle the following three types of builds:

- Windows, Mac, Linux
- Dedicated Server - Windows
- Dedicated Server - Linux

Select `Windows, Mac, Linux` or `Dedicated Server` as the Platform. Then, under Target Platform, choose your platform (Windows or Linux) and build the project.

#### Setup Server Executables Path

Open the master-server project in your IDE. Edit the following file:  
`master-server/master-server-app-api/src/main/resources/application.properties`.

Add the path to your server executables from the previous step where you saved them. For example:  
`D:/Game Builds/Free-Server/Free-Server.exe`.


---

### Master-Server

#### Setup Database

1. Install MongoDB.
2. Open your Mongo shell (`mongosh`).
3. Create your database:
```bash
use free
```
4. Create a new user and password, and give it access to the created database:
```bash
db.createUser({
  user: "root", 
  pwd: "123456", 
  roles: [{role: "readWrite", db: "master-server"}]
})
```
5. Create the following collections:
```bash
db.createCollection("account", { collation: { locale: 'en_US', strength: 2 } })
```
```bash
db.account.createIndex({ username: 1 })
```
```bash
db.createCollection("character", { collation: { locale: 'en_US', strength: 2 } })
```
```bash
db.character.createIndex({ accountId: 1 })
```
```bash
db.createCollection("character_location", { collation: { locale: 'en_US', strength: 2 } })
```
```bash
db.character_location.createIndex({ characterId: 1 })
```
6. Add to the following configuration file:

**File location:** `master.server/master-server-common/src/main/resources/master-server-common-config.properties`

7. Insert the following values into your configuration file and modify them as needed:

```
database.mongo.uri=mongodb://root:123456@127.0.0.1:27017/free
database.mongo.database=free
database.mongo.collection.naming.case=UNDERSCORE
database.mongo.collection.naming.ignored_suffix=Entity
```

Example credentials:
- **User:** root
- **Password:** 123456
- **Database:** free

---

#### Setup Mail

Enable or disable server-side email sending via SMTP.

If you do not have a mail server or mail provider that supports sending emails via SMTP, or if you do not wish to send emails (e.g., for local development), locate the following file:

**File location:**  
`Free-MMORPG/master-server/master-server-common/src/main/resources/master-server-common-config.properties`

Set the value to `false`:
```java
server.can_send_mail=false;
```

**Important:** When `server.can_send_mail` is set to `false`, only log messages are generated on the server side. This is primarily intended for local development. For production environments, email functionality should be properly configured to enable features like "Forgot Password," "Forgot Username," and "Account Activation."


**Setup Mail Values**

For production environments, configure email functionality in the same configuration file used for the database settings:

**File location:**  
`Free-MMORPG/master-server/master-server-common/src/main/resources/master-server-common-config.properties`

Modify the following settings:

```
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
```

**Hint:**  
Do not set both `mail.tls` and `mail.ssl` to `true` simultaneously. Only one should be set to `true` to avoid conflicts.

---

### Deploy Master-Server

#### Download Server
Download the EzyFox Server: <a href="https://resources.tvd12.com/ezyfox-server-full-1.2.9.zip">ezyfox-server-full-1.2.9.zip</a>

#### Server Preparations

In following examples, we use allways the location: `D:\ezyfox-server`.

1. Extract `ezyfox-server-full-1.2.9.zip` and copy the extracted files to the target location.
2. Open the file `D:\ezyfox-server\settings\ezy-settings.xml` in a text editor.
3. Add the following zones inside the `<zones>` tag:
```
<zone>
    <name>free-master-server</name>
    <config-file>free-master-server-zone-settings.xml</config-file>
    <active>true</active>
</zone>
```

---

#### Export External Libraries

1. Open Git Bash and navigate to the folder `free-account-server-startup`.
2. Run the following command:
   ```bash
   mvn clean install -Denv.EZYFOX_SERVER_HOME=deploy -Pezyfox-deploy
   ```
3. Run the `ExternalLibrariesExporter` tool located in `free-account-server-startup/src/test/java/com/assambra/account/tools` in your IDE.
4. When prompted, enter the path: `D:/ezyfox-server`.

---

#### Build the Server

1. Open the file `Free-MMORPG/master-server/build.sh` in the editor
2. Remove the `#` from the line `#export EZYFOX_SERVER_HOME=`:
3. Insert after `export EZYFOX_SERVER_HOME=`, the path `D:/ezyfox-server`.
4. Run the following command in some command client e.g. gitbash from the root of the project (./Free-MMORPG/master-server/):
   ```bash
   bash build.sh
   ```
**Hint:** If you encounter an error like:
```
cp: cannot create regular file 'D:ezyfox-server/settings/zones/': No such file or directory
```
Replace backslashes (`\`) with forward slashes (`/`) in your paths, depending on your operating system.

---

#### Run the Server

Navigate to your server location: `D:/ezyfox-server`.

- **Windows:** Execute `console.bat`.
- **Linux:** Run:
  ```bash
  sh ./console.sh
  ```

For Linux as a service:
- Start the service:
  ```bash
  sh ./start-service.sh
  ```
- Stop the service:
  ```bash
  sh ./stop-service.sh
  ```
- Restart the service:
  ```bash
  sh ./restart-service.sh
  ```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Play the client

In your Unity project (**Free-Client**), double-click the **Persistent scene** in the folder location `Assets/Free-Client/Scenes/`. This is the persistent scene with all Managers/Handlers. All other scenes will automatically load additively/asynchronously when needed, along with the User Interfaces for the active scene.

To learn more about the **Module-GameManager**, visit the [Module-GameManager wiki page](https://github.com/Assambra/Free-MMORPG/wiki/Module-GameManager).

Once everything is set up, press "Play" in the Unity Editor to start the game, provided the server setup is complete (see below).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Contributing

Thank you for your interest in contributing to our project! Contributions are welcome and greatly appreciated.

### How to Contribute

1. **Code Contributions**
    - Fork the repository on GitHub.
    - Create a new branch for your changes.
    - Implement your changes and thoroughly test them.
    - Submit a pull request to our repository.

2. **Report Bugs**  
   Use our GitHub issue tracking system to report bugs. Provide as much detail as possible to help us troubleshoot effectively.

3. **Feature Requests**  
   If you have a great idea for a new feature, create an issue on GitHub and describe your idea in detail. We'll review and discuss it.

4. **Documentation Contributions**  
   If you find errors or want to add to our documentation, feel free to contribute to our Wiki or submit pull requests for README updates.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Resources Section

- For the server/client, we use the [EzyFox Server Framework](https://youngmonkeys.org/projects/ezyfox-server) from [Young Monkeys](https://youngmonkeys.org/). You can support their great project. For more information, visit [Donate to Young Monkeys](https://youngmonkeys.org/donate/).
- For the characters, we use [UMA](https://github.com/umasteeringgroup/UMA) (Unity Multipurpose Avatar System). There is currently a campaign to take UMA to a new level so that it will receive new textures and models in the future. We would be very happy if you supported this. [Let’s fund some new artwork for UMA!](https://www.gofundme.com/f/lets-fund-some-new-artwork-for-uma?utm_campaign=m_pd+share-sheet&utm_content=undefined&utm_medium=copy_link_all&utm_source=customer&utm_term=undefined).
- For the ocean, we use the [Crest Ocean System](https://github.com/wave-harmonic/crest), which, in our opinion, is the best available ocean system for Unity. You can become a sponsor for this project on [GitHub Sponsors](https://github.com/sponsors/wave-harmonic?o=esb).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Credits

- We use textures created by João Paulo, available at [3DTextures.me](https://3dtextures.me). He published these under the CC0 license. You can support him by buying him a [Ko-fi](https://ko-fi.com/katsukagi) or becoming a patron on [Patreon](https://www.patreon.com/gendo).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Thank You

- A special thanks to **tvd12** for fixing various bugs and providing great help with other issues.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Contact

Join us on [Discord](https://discord.gg/vjPWk5FSYj).

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
[product-screenshot]: Github/Images/Free-MMORPG-Demo-Image.v0.43.6.jpg
[master-server-concept]: Github/Images/Master-Server-Concept.jpg
[Unity-url]: https://www.unity.com
[Unity.com]: https://img.shields.io/badge/Unity-000000.svg?style=for-the-badge&logo=unity&logoColor=white