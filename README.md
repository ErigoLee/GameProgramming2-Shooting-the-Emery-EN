# GameProgramming2 - Shooting the Emery - EN
An undergraduate project game developed for the Game Programming II course.  
This project was built with Unity (C#) and basic asset pipelines.

## 1. Game Concept
A lone soldier is deployed behind enemy lines, dodging fire and eliminating foes.

### Core Loop
- Move → Dodge bullets → Shoot Emeny → Repeat

### Objectives
- Clear all enemies within 10 minutes.

### Win / Lose Conditions
- **Win:** All enemies defeated within 10 minutes
- **Lose:** Health reaches 0 or time limit expires

## 2. Key features
- **10-Minute Mission** — Clear all enemies before the timer runs out or the mission fails.
- **Responsive Controls** — Move with **W/A/S/D**, click to **shoot**; auto-rotate toward the firing direction.
- **Firing Constraints** — While shooting, forward/back input (W/S) is disabled to stabilize aim.
- **Aim-Gated Shots** — Bullets spawn **only when the gun is aligned with the target**, at a fixed interval.
- **Enemy AI FSM** — Patrol, Chase, Flee, Attack, Dead; **Chase/Flee** probabilities (0.8/0.2) are tunable in the Inspector.
- **Obstacle Avoidance** — Raycast steering with recovery from unexpected collisions to keep AIs moving.
- **Clean Damage Model** — Only **player-tagged bullets** damage enemies; distinct tags prevent friendly fire.
- **Clear UX Flow** — Title ↔ Manual ↔ Main Game; arrow keys to navigate, **Space** to confirm; end screen shows **elapsed time** and **result**.
- **Audio** — sound effects for shooting and explosions.

## 3. Code description
(1) AIController.cs
- Defines the enemy finite-state machine:
 ```csharp
  public enum FSMState {
      None, Patrol, Chase, Flee, // runaway
      Attack, Dead
  }
```

- State transitions by player distance. Example from UpdatePatrolState():
 ```csharp
if (Distance(patrolDestination, transform.position) < 50.0f) {
    FindNextPoint();
} else if (Distance(playerTransform.position, transform.position) < 200.0f) {
    // Switch to Chase or Flee
    int probability = Random.Range(0, chaseProbability + fleeProbability);
    curState = (probability < chaseProbability) ? FSMState.Chase : FSMState.Flee;
}
```

- Obstacle avoidance with Raycast. Look ahead and steer away when an obstacle is detected (layer mask 1 << 9):
Raycasts are used to detect obstacles in front and prevent collisions by adjusting heading.
```csharp
int layerMask = 1 << 9;
if (Physics.Raycast (transform.position, transform.forward, out hit, minumDistToAvoid, layerMask)) {
	//Vector3 hitNormal = hit.normal;
	//hitNormal.y = 0;
	//targetRotation = Quaternion.LookRotation(transform.forward+hitNormal*50.0f);
	int a = Random.Range (0,2);
	if(a==0)
		transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y-60.0f,transform.rotation.z));
	else
		transform.Rotate(new Vector3(transform.rotation.x,transform.rotation.y+60.0f,transform.rotation.z));

	transform.Rotate (new Vector3 (transform.rotation.x, transform.rotation.y - 60.0f, transform.rotation.z));
	}
```

(2) PlayerController.cs
- Fire on left mouse release. Play SFX/animation and stop forward/back motion during the shot:
```csharp
if (Input.GetMouseButtonUp(0)) {
    GetComponent<AudioSource>().PlayOneShot(shootBefore);
    GetComponent<Animation>().Play("Shooting");
    shootingAni = true;
    targetSpeed = 0.0f;
}
```

- Movement & animation blending. W/S moves forward/back and cross-fades to walking; no input cross-fades to Idle
```csharp
if (!shootingAni) {
    if (Input.GetKey(KeyCode.W)) {
        gameObject.GetComponent<Animation>().CrossFade("walking", 0.1f);
        targetSpeed = maxForwardSpeed;
    } else if (Input.GetKey(KeyCode.S)) {
        gameObject.GetComponent<Animation>().CrossFade("walking", 0.1f);
        targetSpeed = maxBackwardSpeed;
    } else {
        gameObject.GetComponent<Animation>().CrossFade("Idle", 0.1f);
        targetSpeed = 0.0f;
    }
}
```

- Turning & smoothing. A/D rotates left/right; speed transitions are smoothed with Mathf.Lerp to avoid abrupt changes:
```csharp
if (Input.GetKey(KeyCode.A))      rotSpeed = maxLeftAngle;
else if (Input.GetKey(KeyCode.D)) rotSpeed = maxRightAngle;

curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);
```

## 4. How to play?
### Title Screen
- **↑ / ↓**: Select **Manual** or **Start Game**
- **Space**: Confirm selection

### Manual Screen
- **← / →**: Browse instruction pages
- **Space**: Return to the Title screen after reading

### Main Game
- **W / S**: Move forward / backward
- **A / D**: Move left / right
- **Mouse Click**: Fire toward the clicked point

### End of Game
- When you **clear** or **fail** the mission, the **elapsed time** and **result** are displayed.
- Press **Space** to return to the Title screen.

### In-Game screens
(1) Manual Screen (Screenshot)<br/>
<img width="406" height="225" alt="image" src="https://github.com/user-attachments/assets/6d804a42-41e3-4f62-a71a-4b76a297a108" /><br/>
(2) Main Game (Screenshot)<br/>
<img width="426" height="225" alt="image" src="https://github.com/user-attachments/assets/0e43229a-b1fb-4a82-9c98-57ddd565f39b" /><br/>

## 5. Implementation Notes & Problems Solved
### (1) AI Controllers (`Aicontroller.cs` — instances 2, 3, 4, 5 share the same logic)
- Based on the **simple FSM** template learned in the GameProgramming2 lectures (used as the base skeleton).
- On detecting the player, the AI randomly chooses to **flee (20%)** or **chase (80%)**.
  - These probabilities are exposed as **public** fields so they can be tuned easily.

### (2) Obstacle & Collider Avoidance
- Initially attempted **Navigation Mesh**, but synchronizing reliable **shooting** and **walking** animations proved difficult, so NavMesh was dropped for this project.
- Implemented steering by referencing **`VehicleAvoidance.cs`**, enabling the agent to avoid obstacles appropriately.

### (3) Edge-Case Fix: Getting Stuck on Obstacles
- With only the `VehicleAvoidance.cs` approach, AIs sometimes collided with unexpected geometry and could not move forward.
- To resolve this, key obstacles were given **Rigidbody** components, and **`OnCollisionEnter()`** was used to steer/nudge away so movement could resume.

### (4) Shooting: Timing, Aiming & Firing Constraints
- To ensure bullets fire **only when the gun is pointing at the target**, an **accumulator** in **`FixedUpdate()`** uses **`Time.fixedDeltaTime`** to spawn bullets at a fixed interval.
- While firing: set movement speed to **0** and **disable `W`/`S`** input.
- Automatically rotate the player toward the firing direction during the shot.

### (5) Damage Filtering (Player vs. AI Bullets)
- In **`OnCollisionEnter()`**, only **player-fired** bullets reduce AI health.
- Achieved by assigning distinct **tags** to bullets (e.g., `PlayerBullet` vs. `AIBullet`) and checking these tags in code.

### (6) Player Controller**
- `PlayerController.cs` was implemented based on the **Introduction to Game Programming** content covered in the *Game Programming Basics* course.

### (7) **Soldier Object**
- Merged the **soldier** and **gun** meshes; authored **Walking / Idle / Shooting** animations in **3ds Max** on the rigged model. (Full third-party sources are listed under **Asset References**.)

### (8) **UI & Animation Components**
- **EagleAnimation.cs**
  - Adds logic to play the Eagle object’s `Armature|Fly` animation.

- **GUISkin**
  - Created using the project’s **GameSkin**.


## 6. Play link
https://erigolee.github.io/GameProgramming2-Shooting-the-Emery-EN/WebBuilds/shootingGame/



## License 
- All **source code** in this repository is licensed under the [MIT License](./LICENSE).
- Some code and assets are adapted from **GameProgramming2 course materials** (undergraduate coursework).
  Details are listed below.
- Third-party **assets** (models, textures, sounds, fonts, etc.) remain under their original licenses.
  They may **not be licensed for redistribution or in-game use** in this repository.  
  Please check the original source pages for specific license terms.

### Course References
The following prefabs, scripts, and images are **adapted from the GameProgramming2 (undergraduate) lectures**:
-  **Prefabs**
  - Bullet 1, Bullet 2
  - ParticleExplosion (including related images)
-  **Scripts**
  - Bullet.cs *(with additional audio-related code added by me)*
  - AutoDestruct.cs

### Assets References
> Note: Third-party assets remain under their original licenses.
> They may **not be licensed for redistribution or in-game use** in this repository.  
> Please review each source page for license terms before redistribution or in-game use.

- **Gun object & textures**
  - Source: https://www.turbosquid.com/ko/3d-models/free-m1911a1-pistol-3d-model/362695

- **Soldier (object & textures(Swath_texture))**
  - Source: https://www.turbosquid.com/ko/3d-models/free-fbx-model-soldier-military-character-rigged/516949
    
- **Button image**
  - Note: In this project, the term **"button"** is used to refer to a **floor object**, not a UI element.
  - Source: https://pixabay.com/ko/photos/%EC%8A%A4%EC%BD%94%ED%8B%80%EB%9E%9C%EB%93%9C-%EC%9D%B8%EC%9D%98-%EB%B3%84%EB%AA%85-%ED%95%B4%EB%B3%80-214564/
  - 
- **Wall image**
  - Source: https://pixabay.com/ko/photos/%EC%98%B7%EA%B0%90-%EA%B5%AC%EC%A1%B0-%EB%B9%A8%EA%B0%84%EC%83%89-%EC%A7%81%EB%AC%BC-68946/
  
- **Building1 object & textures**
  - Textures: `tex_besi`, `texture-sumur`, `wall`, `wall_2`
  - Source: https://free3d.com/ko/3d-model/buildings-in-the-city-471489.html

- **Button2 image**
  - Note: In this project, the term **"button"** is used to refer to a **floor object**, not a UI element.
  - Source: https://pixabay.com/ko/photos/%EC%BD%98%ED%81%AC%EB%A6%AC%ED%8A%B8-%EB%B2%BD-%EA%B5%AC%EC%A1%B0-%EB%8F%84%EC%8B%9C-1840731/

- **Building2 object & textures**
  - Textures: `TexturesCom_BarreIsC`, `TexturesCom_Concret`, `TexturesCom_concret`, etc. (see `Images/Building2`)
  - Source: https://free3d.com/ko/3d-model/ruin-wall-3d-model--16179.html

- **Pyramid (object)**
  - Source: https://free3d.com/ko/3d-model/pyramid-832548.html

- **Pyramid_image (image)**
  - Source: https://pixabay.com/ko/photos/roter-%EB%AA%A8%EB%9E%98-%EC%95%84%ED%94%84%EB%A6%AC%EC%B9%B4-2042738/

- **Building3 object & textures**
  - Textures: `city_house_2_Col`, `city_house_2_Nor`, etc.
  - Source: https://free3d.com/ko/3d-model/small-city-building-2-65522.html

- **Building4 object & textures**
  - Textures: `windmill_001_base_COL`, `windmill_001_lopatky_COL`
  - Source: https://www.turbosquid.com/ko/3d-models/3d-farm-house-1596847

- **City object & textures**
  - Textures: `Palette`
  - Source: https://free3d.com/ko/3d-model/sci-fi-tropical-city-25746.html

- **City_Wall object**
  - Source: https://www.turbosquid.com/ko/AssetManager/Index.cfm?stgAction=getFiles&subAction=Download&intID=586045&intType=3

- **Building5 object & textures**
  - Textures: `MAF_Buildings_Roof_detail`
  - Source: https://www.turbosquid.com/ko/3d-models/apartment-house-building-3d-model-1650425

- **Building6 object & textures**
  - Textures: `free_building_d`
  - Source: https://www.cgtrader.com/free-3d-models/architectural/street/free-nyc-building

- **Building7 object & textures**
  - Textures: `1383`, `1383-v3`, etc. (see `images/building7`)
  - Source: https://www.cgtrader.com/free-3d-models/exterior/office/indusrtial-building

- **Building8 object & textures**
  - Textures: `Modern Brick House_1`, etc. (see `FBX/Building8/Building8.fbm`)
  - Source: https://www.turbosquid.com/ko/3d-models/3d-modern-brick-house-1342083

- **Building9 object & textures**
  - Textures: `Blech_01`, `Blech_01_Bump`, etc. (see `FBX/Building9/Building9.fbm`)
  - Source: https://www.turbosquid.com/ko/3d-models/berlin-juedenstrasse-residence-dxf-free/897214

- **Building10 object & textures**
  - Textures: `Col Klein Haus_C`, `Col Klein Haus_N`, `Col Klein Haus_S`
  - Source: https://free3d.com/ko/3d-model/small-building-1-22802.html

- **Building11 object & textures**
  - Textures: `Build10`
  - Source: https://www.turbosquid.com/ko/3d-models/free-3ds-mode-building-games/689819

- **Building12 object & textures**
  - Textures: `DSC_5871`
  - Source: https://www.turbosquid.com/ko/3d-models/free-realistic-old-building-3d-model/837997

- **Building13 object & textures**
  - Textures: `trade clothesdiffusemap`
  - Source: https://free3d.com/ko/3d-model/trade-clothes-73486.html

- **Cactus object & textures**
  - Textures: `CacutsTexture` *(file name as used in project)*
  - Source: https://www.turbosquid.com/ko/3d-models/3d-model-low-poly-cactus-1434125

- **Farm wall object & textures**
  - Textures: `tijoloTexture`
  - Source: https://free3d.com/ko/3d-model/wall-with-grid-32677.html

- **Buttom2 image**
  - File/Material: `buttom2 <Material>`
  - Source: https://pixabay.com/ko/photos/%EC%9E%94%EB%94%94-%EB%B0%B0%EA%B2%BD-%EB%B6%84%EC%95%BC-%EC%8B%A0%EC%84%A0%ED%95%9C-84622/

- **Tree object & textures**
  - Textures: `BranchesDiffuse`, `BranchesNormal`, etc. (see `FBX/Tree/Textures`)
  - Source: https://www.cgtrader.com/items/848599/download-page

- **Tree2 object & textures**
  - Textures: `Tree_1`, `Tree_2`
  - Source: https://www.cgtrader.com/items/882950/download-page

- **Tree3 object & textures**
  - Textures: `branch_dif`, `truck_bump`, `truck_dif`, etc. (see `FBX/Tree3/textures`)
  - Source: https://www.cgtrader.com/items/738892/download-page

- **Bridge object & textures**
  - Textures: `Diffuse Map`
  - Source: https://www.turbosquid.com/ko/3d-models/free-bridge-games-3d-model/666679

- **Bath object & textures**
  - Textures: `map_bath1_AO`, `map_bath1_Noraml`, etc. (see `FBX/Bath`)
  - Source: https://3dexport.com/free-3dmodel-old-bath-1-322636.htm

- **Basin object & textures**
  - Textures: `Taz1-low_defaultMath_Height`, `Taz1-low_defaultMath_Normal` (see `FBX/Basin`)
  - Source: https://3dexport.com/free-3dmodel-old-basin-307243.htm

- **oldCar object & textures**
  - Textures: `beige`, `normal` (see `FBX/Oldcar`)
  - Source: https://www.turbosquid.com/ko/3d-models/moskvitch-402-max-free/1024740

- **Eagle object**
  - Source: https://www.turbosquid.com/ko/3d-models/eagle-rigged-륱-free/1045001

- **Title_image**
  - Source: https://pixabay.com/ko/illustrations/%EC%A0%84%EC%9F%81-%ED%8C%8C%EA%B4%B4-%EC%B6%A9%EB%8F%8C-%ED%95%B5-2930360/

- **KeyBoard image**
  - Source: https://pixabay.com/ko/vectors/%ED%82%A4%EB%B3%B4%EB%93%9C-%ED%82%A4-%EB%B2%84%ED%8A%BC-%EA%B2%B0%ED%95%A9-37762/

- **Paper image**
  - Usage: background image behind *manual scene*, *success scene*, *failed scene*
  - Source: https://pixabay.com/ko/illustrations/%EC%9A%A9%EC%A7%80-%ED%8E%B8%EC%A7%80%EC%A7%80-%EC%96%91%ED%94%BC%EC%A7%80-%EB%8A%99%EC%9D%80-68829/

- **Mouse image**
  - Source: https://pixabay.com/ko/vectors/%EB%A7%88%EC%9A%B0%EC%8A%A4-%EC%BB%B4%ED%93%A8%ED%84%B0-%ED%95%98%EB%93%9C%EC%9B%A8%EC%96%B4-%EA%B4%91%ED%95%99-159568/

- **Solider / Soldier image**
  - Source: https://pixabay.com/ko/vectors/%EC%9C%A1%EA%B5%B0-%EC%A0%84%ED%88%AC-%EC%9C%84%EC%9E%A5-%EB%B3%B4%EB%B3%91-%EC%B4%9D-1299938/
  - Note: File name may appear as `Solider` in the project.

- **Lambert object & textures**
  - Textures: `lambert1_Base_Color`, `llambert1_Normal` (see `FBX/Lambert`)
  - Source: https://www.turbosquid.com/ko/3d-models/free-obj-mode-Victorian-entertainment-center/1078494

- **stonewall image — used as Wall2 texture**
  - Source: https://pixabay.com/ko/photos/%EB%85%B9%EC%8A%A8-%EB%B0%B0%EA%B2%BD-%EB%85%B9-%EC%B2%A0-%EA%B8%88%EC%86%8D-4926112/

- **Win image**
  - Source: https://pixabay.com/ko/vectors/%EC%A3%BC%EB%A8%B9-%ED%95%B8%EB%93%9C-%ED%9D%9D%EB%AF%B8%EB%A1%9C%EC%9A%B4-%EA%B2%83%EB%93%A4-1294353/

- **Dead image**
  - Source: https://pixabay.com/ko/illustrations/%EC%B4%9D-%ED%8A%B8%EB%A0%81%ED%81%AC-%EB%AC%B4%EA%B8%B0-%EB%A6%AC%EB%B3%BC%EB%B2%84-3294590/

- **Success image**
  - Source: https://pixabay.com/ko/vectors/%EC%8B%A4%EB%A3%A8%EC%97%A3-%EC%8B%9D-%ED%96%89%EB%B3%B5%ED%95%9C-%EC%84%B1%EA%B3%B5-3127948/

- **235968__tommccann__explosion-01.wav (audio)**
  - Source: https://freesound.org/people/tommccann/sounds/235968/

- **Wall2 (object)**
  - Source: https://www.turbosquid.com/ko/3d-models/medieval-wall-3d-model-1469745

### Audio
- `Voices - Patrick Patrikios.mp3`  
  - Source: YouTube Audio Library — https://studio.youtube.com/channel/UCN2EGtmnBlsJlEh2JF4hQhw/music
- `Awful - josh pan.mp3`  
  - Source: YouTube Audio Library — https://studio.youtube.com/channel/UCN2EGtmnBlsJlEh2JF4hQhw/music
- `Brooklyn and the Bridge - Nico Staf.mp3`  
  - Source: YouTube Audio Library — https://studio.youtube.com/channel/UCN2EGtmnBlsJlEh2JF4hQhw/music
- `Melancholia - Godmode.mp3`  
  - Source: YouTube Audio Library — https://studio.youtube.com/channel/UCN2EGtmnBlsJlEh2JF4hQhw/music
- `Broken - Patrick Patrikios.mp3`  
  - Source: YouTube Audio Library — https://studio.youtube.com/channel/UCN2EGtmnBlsJlEh2JF4hQhw/music
- `handling 50cal gun-[AudioTrimmer.com].mp3`  
  - Source: YouTube Audio Library — https://studio.youtube.com/channel/UCN2EGtmnBlsJlEh2JF4hQhw/music  
  - Note: Edited/trimmed via AudioTrimmer.com

### Fonts
- `HOONMAKDAEYUNPILR`  
  - Source: https://www.gytni.com/new_gytni/license.php?document_srl=23703&mode=font&mode2=view&mode3=&utm_source=chatgpt.com
- `SCDREAM8`  
  - Source: https://s-core.co.kr/company/font/


 
