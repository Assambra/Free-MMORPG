[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]

<div align="center">
<h1>Free-MMORPG</h1>
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

<h2>Table of Contents</h2>
<ul>
    <li><a href="#foreword">Foreword</a></li>
    <li><a href="#key-features">Key Features</a></li>
    <li><a href="#playable-demo">Playable Demo</a></li>
    <li><a href="#master-server">Master Server</a></li>
        <ul>
            <li><a href="#introduction-transitioning-to-a-hybrid-backend">Introduction: Transitioning to a Hybrid Backend</a></li>
            <li><a href="#project-structure-overview">Project Structure Overview</a></li>
            <li><a href="#communication-flow">Communication Flow</a></li>
            <li><a href="#master-server-centralized-management">Master-Server: Centralized Management</a></li>Request Flow
            <li><a href="#request-flow">Request Flow</a>
            <li><a href="#notes">Notes</a>
        </ul>
    <li><a href="#client">Client</a></li>
        <ul>
            <li><a href="#clone-from-github">Clone Free-MMORPG from GitHub</a></li>
            <li><a href="#needed-unity-packages">Needed Unity packages</a></li>
            <ul>
                <li><a href="#addressables">Addressables</a></li>
                <li><a href="#burst-compiler">Burst compiler</a></li>
                <li><a href="#collections">Collections</a></li>
                <li><a href="#color-picker">HSV-Color-Picker-Unity</a></li>
                <li><a href="#newtonsoft-json">Newtonsoft Json</a></li>
            </ul>
            <li><a href="#needed-projects-from-github">Needed projects from GitHub</a></li>
            <ul>
            <li><a href="#crest-ocean-system">Crest Ocean System</a></li>
                <ul>
                    <li><a href="#get-crest-ocean-system">Get Crest Ocean System</a></li>
                    <li><a href="#install-crest-ocean-system">Install Crest Ocean System</a></li>
                </ul>
            </ul>
            <li><a href="#needed-unity-packages-from-the-unity-asset-store">Needed Unity packages from the Unity asset store</a></li>
            <ul>
                <li><a href="#mapmagic-2">MapMagic 2</a></li>
                <li><a href="#uma-2">UMA 2</a></li>
            </ul>
            <li><a href="#play-the-client">Play the client</a></li>
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
    <li><a href="#resources-section">Resources Section</a></li>
    <li><a href="#credits">Credits</a></li>
    <li><a href="#thank-you">Thank you</a></li>
    <li><a href="#contact">Contact</a></li>
</ul>

---

## Foreword

This project utilizes free resources for both server and client technology. Without these, the project likely wouldn't have been possible, so I extend my special thanks to the team at [Young Monkeys](https://youngmonkeys.org/) for their excellent foundational work, particularly for the [EzyFox Server](https://youngmonkeys.org/projects/ezyfox-server) framework, and for sharing it with the community. Also, special thanks to all the contributors to the [UMA](https://github.com/umasteeringgroup/UMA) project and their outstanding Unity Multipurpose Avatar System. For the ocean, we use the [Crest Ocean System](https://github.com/wave-harmonic/crest), which, in my personal opinion, is the best available ocean system for Unity. Please check out our Resources Section to see what technologies we use and consider supporting their projects in some way, such as through donations or code contributions, to help these talented individuals continue doing what they love: writing great code and developing fantastic frameworks.

For the game client we are using [Unity](https://unity.com "Unity") as game engine.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Key Features

<ul>
    <li>Master Server</li>
        <ul>
            <li>Extending EzyFox Server's room management to launch dedicated Unity servers as room</li>
            <li>One Unity-Server project for all room-servers</li>
            <li>Unity Engine for Client and Server</li>
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

## Master-Server

![Master-Server Concept][master-server-concept]

### Introduction: Transitioning to a Hybrid Backend

This project initially began with a backend built entirely on **EzyFox Server**. However, over time, I realized that developing both the client and server in **C#** and **Unity** was not only more intuitive but also significantly more efficient. While EzyFox Server provides robust networking capabilities, it posed significant challenges when implementing complex features such as physics-based collision and other advanced mechanics.

For example, I initially calculated player height using a heightmap to ensure that the server's height calculations aligned with those on the client. While functional, this approach was labor-intensive and required significant effort to maintain state consistency between the server and Unity client. This challenge highlighted a recurring issue: each new feature or problem would require a similarly complex implementation to synchronize the server state with the client.

To overcome these limitations, I extended EzyFox Server's existing room structure to support spawning **Unity-based server instances**. This hybrid approach combines EzyFox's powerful networking features with Unity's physics and game logic, resulting in a seamless and efficient integration for server-client interactions.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

### Project Structure Overview

This project comprises three key components:

1. **Master-Server**: Based on the EzyFox Server, it acts as the central hub for communication and room management.
2. **Free-Server**: A Unity instance that handles the game logic, physics, and state management for individual rooms.
3. **Free-Client**: The client application for players, built using Unity and communicating with the Master-Server.

### Communication Flow
The **Unity-Servers** act as clients for the **Master-Server**, ensuring seamless interaction between:

- **Free-Client ↔ Master-Server ↔ Free-Server**

We rely on the **EzyFox Client SDK** for all communications across these components.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Master-Server: Centralized Management

The **Master-Server** is built on EzyFox Server and performs several critical tasks:

- **User Management**: Authentication and session tracking.
- **Database Operations**: Handling requests to and from the database.
- **Unity Server Management**: Spawning, monitoring, and managing a Unity-Server instance for each room.

### Request Flow

1. **Client Request**: A Unity-Client sends a request to the Master-Server.
2. **Forwarding**: The Master-Server forwards the request to the appropriate Unity-Server.
3. **Processing**: The Unity-Server processes the request (e.g., physics simulation or game logic updates).
4. **Response Handling**: The processed response is sent back to the Master-Server, which forwards it to the appropriate client(s).

<p align="right">(<a href="#readme-top">back to top</a>)</p>



### Notes

This structure ensures that the Master-Server remains lightweight while leveraging Unity’s strengths for game-specific logic and physics calculations. Future sections will detail implementation specifics and provide example workflows for game scenarios.

---

## Client

### Clone from GitHub
Clone the repository:

```bash
git clone git@github.com:Assambra/Free-MMORPG.git
```

Download and extract the file: [ezyfox-server-csharp-client-1.1.6-unity.zip](https://github.com/youngmonkeys/ezyfox-server-csharp-client/archive/refs/tags/v1.1.6-unity.zip).

Perform the following steps for both Unity projects: **Free-Client** and **Free-Server**:
1. Insert the extracted folder `ezyfox-server-csharp-client-1.1.6-unity` into the root folder `/Assets` of your opened Unity project.
2. Open the Unity Editor and navigate to the folder `Assets/ezyfox-server-csharp-client-1.1.6-unity`.
3. Right-click inside the folder -> **Create** -> **Assembly Definition**, and rename it to `com.tvd12.ezyfoxserver.client`.

#### Free-Server Setup
In the Unity Editor:
1. Navigate to `Assets/Free-Server/Scripts/Assambra.Server`.
2. In the Inspector, locate the **Assembly Definition Reference** section.
3. Remove the missing reference starting with `GUID:` (if present).
4. Add a new reference by clicking the plus sign (+) and select the earlier created `com.tvd12.ezyfoxserver.client` assembly definition.
5. Click **Apply**.

#### Free-Client Setup
Repeat the same steps for the **Free-Client** project:
1. Navigate to `Assets/Free-Client/Scripts/Assambra.Client`.
2. Follow the same process for updating the **Assembly Definition Reference**.

#### Crest Ocean System
Follow the same procedure for Crest Ocean System if it is already installed. If not, you can find the installation instructions under the section [Crest Ocean System](#crest-ocean-system).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Needed Unity packages

To manually add these packages is only required if you create a new Unity project—for example, if you create a Unity package from Free-MMORPG and insert it into a brand-new, empty Unity project. These are already included in this project.

#### Addressables
Install Addressables via the Unity Package Manager. Navigate to `Windows -> Package Manager`. Select `Packages: Unity Registry` from the dropdown menu. Use the search field to search for "Addressables." Click the "Install" button to install the package.

#### Burst Compiler
Install the Burst Compiler via the Unity Package Manager. Navigate to `Windows -> Package Manager`. Select `Packages: Unity Registry` from the dropdown menu. Use the search field to search for "Burst." Click the "Install" button to install the package.

#### Collections
Install Collections via the Unity Package Manager. Navigate to `Windows -> Package Manager`. Select `Packages: Unity Registry` from the dropdown menu. Use the search field to search for "Collections." Click the "Install" button to install the package.

#### Color Picker
To get the HSV-Color-Picker-Unity:

1. Edit the file: `Free-MMORPG\Free-Client\Packages\manifest.json`.
2. Add a new dependency: `"com.judahperez.hsvcolorpicker": "3.3.0"`.
3. Additionally, add the following section to the file:

```json
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
```

**Hint:** Don't forget commas after "dependencies" and before "scopedRegistries" (`'},'`), and ensure the line before the new dependency ends with a comma.

#### Newtonsoft Json
The EzyFox Client SDK requires the Newtonsoft Json package to work.

1. Edit the file: `Free-MMORPG\Free-Client\Packages\manifest.json`.
2. Add a new dependency: `"com.unity.nuget.newtonsoft-json": "3.2.1"`.

Please read the hint below.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Needed projects from GitHub

### Crest Ocean System

#### Get Crest Ocean System
Download the latest source code with tag `4.21.3` as a zip file: <a href="https://github.com/wave-harmonic/crest/releases/tag/4.21.3">4.21.3</a>.

#### Install Crest Ocean System
Unzip and drag the second crest folder into Unity: `./crest-4.21.3/crest-4.21.3/crest/Assets/Crest/Crest`.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Needed Unity packages from the Unity Asset Store

### MapMagic 2
Go to the Unity Asset Store and get the free asset: <a href="https://assetstore.unity.com/packages/tools/terrain/mapmagic-2-165180">MapMagic 2</a>.

After acquiring the asset:
1. Open both projects (**Free-Client** and **Free-Server**) in the Unity Editor.
2. Open the **Package Manager** (`Window -> Package Manager`) and select `Packages: My Assets`.
3. Download and import **MapMagic 2** into both projects.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### UMA 2
Please use our modified version of UMA: <a href="https://drive.google.com/drive/folders/1bZsXNPgpA5pMtIq1rkJhNSJeAs05XDxg?usp=sharing">UMA 2.13 Beta 1-modified.rar</a>.

Steps to use it:
1. Download and extract the `.rar` file.
2. Drag the folder `UMA` directly into `Free-Client/Assets` in the Unity Editor.

~~Alternatively, get the free asset from the Unity Asset Store: <a href="https://assetstore.unity.com/packages/3d/characters/uma-2-35611">UMA 2</a>.~~

~~After importing UMA 2 into the project, you must add Assembly Definition References:~~
- Navigate to `Assets/UMA/Core/UMA_Core`.
- In the Inspector, add references for `Addressables`, `Unity.Mathematics`, `Unity.Burst`, and `Unity.Collections`.

Additional required steps can be found in: `Free-Client/Assets/UMA/Addressables - Read Me Now.pdf`.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Play the client

In your Unity project (**Free-Client**), double-click the **Persistent scene** in the folder location `Assets/Free-Client/Scenes/`. This is the persistent scene with all Managers/Handlers. All other scenes will automatically load additively/asynchronously when needed, along with the User Interfaces for the active scene.

To learn more about the **Module-GameManager**, visit the [Module-GameManager wiki page](https://github.com/Assambra/Free-MMORPG/wiki/Module-GameManager).

Once everything is set up, press "Play" in the Unity Editor to start the game, provided the server setup is complete (see below).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Server

We use two servers: a **Free Account Server** and a **Free Game Server**.
You can only start and test one of them from your IDE at the same time since they both use the same port. To test both simultaneously, you need to deploy the server. More information can be found in the section [Deploy the Server](#deploy-the-server).

### Setup Database

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
db.createCollection("user", { collation: { locale: 'en_US', strength: 2 } })
```
```bash
db.user.createIndex({ username: 1 })
```
```bash
db.createCollection("character", { collation: { locale: 'en_US', strength: 2 } })
```
```bash
db.character.createIndex({ name: 1 })
```
```bash
db.createCollection("character_location", { collation: { locale: 'en_US', strength: 2 } })
```
6. Add or create the following configuration files for both server projects:

**File locations:**
- `Free-Account-Server/free-account-server-common/src/main/resources/free-account-server-common-config.properties`
- `Free-Game-Server/free-game-server-common/src/main/resources/free-game-server-common-config.properties`

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

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Setup Mail

Enable or disable server-side email sending via SMTP.

### Basic Configuration
If you do not have a mail server or mail provider that supports sending emails via SMTP, or if you do not wish to send emails (e.g., for local development), locate the following file:

**File location:**  
`Free-MMORPG/master-server/master-server-common/src/main/resources/master-server-common-config.properties`

Set the value to `false`:
```java
server.can_send_mail=false;
```

**Important:** When `server.can_send_mail` is set to `false`, only log messages are generated on the server side. This is primarily intended for local development. For production environments, email functionality should be properly configured to enable features like "Forgot Password," "Forgot Username," and "Account Activation."


### Setup Mail Values
For production environments, configure email functionality in the same configuration file used for the database settings:

**File location:**  
`Free-MMORPG/master-server/master-server-common/src/main/resources/master-server-common-config.properties`

Add or modify the following settings:

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

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Deploy Server

### Download Server
Download the EzyFox Server:  
<a href="https://resources.tvd12.com/ezyfox-server-full-1.2.9.zip">ezyfox-server-full-1.2.9.zip</a>

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Server Preparations
In this example, we use the location: `D:\ezyfox-server`.

1. Extract `ezyfox-server-full-1.2.9.zip` and copy the extracted files to the target location.
2. Open the file `D:\ezyfox-server\settings\ezy-settings.xml` in a text editor.
3. Add the following zones inside the `<zones>` tag:
```
<zone>
    <name>free-account-server</name>
    <config-file>free-account-server-zone-settings.xml</config-file>
    <active>true</active>
</zone>
```
```
<zone>
    <name>free-game-server</name>
    <config-file>free-game-server-zone-settings.xml</config-file>
    <active>true</active>
</zone>
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Export External Libraries

#### Steps for Account Server
1. Open Git Bash and navigate to the folder `free-account-server-startup`.
2. Run the following command:
   ```bash
   mvn clean install -Denv.EZYFOX_SERVER_HOME=deploy -Pezyfox-deploy
   ```
3. Run the `ExternalLibrariesExporter` tool located in `free-account-server-startup/src/test/java/com/assambra/account/tools` in your IDE.
4. When prompted, enter the path: `D:/ezyfox-server`.

#### Steps for Game Server
Repeat the above steps for the `free-game-server-startup` folder.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Build the Server

1. Open the following files and remove the `#` from the line `#export EZYFOX_SERVER_HOME=`:
    - `Free-MMORPG/Free-Account-Server/build.sh`
    - `Free-MMORPG/Free-Game-Server/build.sh`

2. After `export EZYFOX_SERVER_HOME=`, insert the path `D:/ezyfox-server`.

3. Run the following commands from the root of each project:
   ```bash
   bash build.sh
   ```

**Hint:** If you encounter an error like:
```
cp: cannot create regular file 'D:ezyfox-server/settings/zones/': No such file or directory
```
Replace backslashes (`\`) with forward slashes (`/`) in your paths, depending on your operating system.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Run the Server

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