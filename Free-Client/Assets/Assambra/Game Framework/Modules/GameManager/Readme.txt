README GameManager

Demo:
Open your DemoPersistent scene and press play, select the GameObject SceneHandler and change 
the CurrentScene to another Scene asset. The selected scene asset will load the scene additive / async
and the required UI elements will be instantiated. 
The LastScene is unloaded async and the instantiated UIs are destroyed.


How to use the GameManager Asset:

1.) Create a scene, this will be your persistent scene there will be the GameManager, SceneHandler, UIHandler lives. 
All other scenes are loaded additive.

2.) Add the GameManager, ScenHandler and UIHandler prefab into the hierarchy. 
UIHandler: Add to the Canvas field the child object canvas and to the SceneHandler field the SceneHandler.

3.) Create your needed scenes like GameMenu, World, Dungeon... Adding all scenes to 
Build Settings -> Scenes in Built. 

4.) Create for each scene a Scene asset (except the peristent scene). 
Menu: Create/Assambra/Scene. Create it into a folder e.g. Data/Scenes 
and name it to your scene name. One of this need to have checked IsFirstScene e.g GameMenu.
Add into the Scene field the Scene. Add the Scene asset to the SceneHandler scenes list field.

5.) Create for each UI Set you need a SceneUISet asset.
Menu: Create/Assambra/SceneUISet. Create it into a folder e.g Data/SceneUISets 
and name it like Game, GameMenu.

6.) Use the prefab YourScenUI and create for each UI Element you need a new prefab 
as example Login, and child UILogin, Inventory, SkillBar and so on and place it into a /Prefab folder.

7.) Select the named SceneUISet asset. (lock it  with the lock symbol). 
Add for each SceneUISet the UI Element prefabs that have to be in this set into the array "UIElementPrefab", 
(the eralier created UI prefabs).

8.) Select the named Scene asset (lock it with the lock symbol). 
Add the eralier created named SceneUISet asset to your field list "SceneUISet",
that you want to use in this scene.

Additional and recommended: 
Add to each Scene Main Camera and Directional Light the two scripts MainCamera and DirectionalLight
found in /Scripts/Helper.
Enable on the Persistent Scene IsPersistentMainCamera and IsPersistentDirectionalLight. 
This will disable all other Directional Light and Main Cameras in play mode or in Game builts in the other scenes. 

In this way, for example, post-processing can be installed and set once in the camera in the Persistent Scene,
Or one time a global Light setting for all scenes.
In the other scenes you can leave the light and camera enabled in the editor when creating the level. 
This will also fix the Error that two audio listener in scene, this are attached on the Main Camera, 
if a aditional scene are loaded.

