# Screeps3D
A 3D client for the MMORTS Screeps.com

## Goal 
To build a 3D client for Screeps.

## Progress
I think it has a fairly solid foundation so that if anyone wants to contribute to it, it is at a good point. There is a websocket/http client that could be fleshed out more but has all the basic functionality. There is a solid system for rendering rooms/objects. At the moment the project is organized into two systems:

* ScreepsAPI - HTTP/Websocket client for communicating with the server
* Screeps3D - Login, WorldView, RoomView, etc.

It would be ideal to keep these two separate, so that the ScreepsAPI can be exported as a package for use in other screeps/unity3D projects. 

Here are the major areas that we would like to tackle next: 
* ~Get it working with private servers~
* Gameplay options like hotkeys, camera controls, console colors, etc.
* Finish model set for room objects (PowerBanks #23, Portals #19 ... Others?)
* ~Rendering roads~
* ~Rendering creeps (new model with player icon downloaded and assigned as texture)~
* Rendering creep, tower, link, etc. actions (particle systems are an excellent way to make these visually appealing)
* Creep Say (I'm imagining a floating text that appears above their heads and drifts up to eventually disappear)
* ~Figure out the best way to subscribe/unsubscribe from rooms, fog of war, etc.~
  

## Installation

The Project is built using Unity and C# so the following software is required to run it.

### Software Requirements
* Visual Studio 2019 Community Edition
* Unity 2019.1.8f1
* Blender 2.79
  * If no models are showing up, you need to install blender, .blend files are used in unity
  * Models might take a while before showing up after installing blender, it has to "reimport" the .blend files before they render.

After installation you can open the project in unity, and in the asset menu you can "Open C# Project" to open visual studio
* The C# Solution is constructed every time you open the project with Unity.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change. If there is an existing issue you would like to tackle, please mention that in the issue to allow others to collab with you :)


---
Things thmsn came up with initially while streaming botarena

### ideas
* Crumble walls based on hitpoints
* Render alliances
  * Render your "friends" in another color than red
* Make resource/material nodes larger / smaller based on resource amount available
* ~color code material nodes based on the type of material~
* minimap

### stuff found when rendering BotArena
 * can we implement a spectator mode? #39
   * yes xD, currently shuffles random rooms with pvp if you toffle PVP Spectate on
   * ideas
     * follow a creep? perhaps random?
     * different camera angels
 * render safemode
 * prevent certain objects from being selected, 
   * selecting creeps and having roads selected sucks
 
 #### bugs
* creeps not despawning
* tombstones not showing
* ~dropped energy not rendered #36~ 
* attacks are rendered too far (blue beam)
* room update / connection is lost, and rooms looses all data
  * Possibly because of this
```
["err@room:E3S3","subscribe limit reached"]
 
(Filename: C:\buildslave\unity\build\Runtime/Export/Debug.bindings.h Line: 45)

recieved null data from server
 
(Filename: C:\buildslave\unity\build\Runtime/Export/Debug.bindings.h Line: 45)
```
* http error stays center of screen (Locked)
* Creeps remain dorment after their TTL becomes negative
* CreepView _body.material.mainTexture = _creep.Owner.Badge; Owner is null



