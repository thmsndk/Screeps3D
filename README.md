# Screeps3D
A 3D client for the MMORTS Screeps.com
![roomview](readme-images/roomview.png?raw=true "A base on shard2 showing some models")

## Goal 
To build a 3D client for Screeps.
![mapoverview](readme-images/mapoverview.png?raw=true "A base on shard2 showing some models")

## Progress
It has a fairly solid foundation so that if anyone wants to contribute to it, it is at a good point. There is a websocket/http client that could be fleshed out more but has all the basic functionality. There is a solid system for rendering rooms/objects. At the moment the project is organized into two systems:

* ScreepsAPI - HTTP/Websocket client for communicating with the server
* Screeps3D - Login, WorldView, RoomView, etc.

It would be ideal to keep these two separate, so that the ScreepsAPI can be exported as a package for use in other screeps/unity3D projects. 

Scroll to the bottom for a feature comparison with official clients

Here are the major areas that we would like to tackle next: 
* ~Get it working with private servers~
* Respawn / Placing spawn
* Placing construction sites
* Gameplay options like hotkeys, camera controls, console colors, etc.
* ~Finish model set for room objects~
* ~Rendering roads~
* ~Rendering creeps (new model with player icon downloaded and assigned as texture)~
* Rendering creep, tower, link, etc. actions (particle systems are an excellent way to make these visually appealing)
* ~Creep Say (I'm imagining a floating text that appears above their heads and drifts up to eventually disappear)~
* ~Figure out the best way to subscribe/unsubscribe from rooms, fog of war, etc.~
* ~Visualize nukes flying from source to target room (#58)~
  

# Installation
Want to try the client?  
Download a release from the [release page](https://github.com/thmsndk/Screeps3D/releases)

## Contributing
The Project is built using Unity and C# so the following software is required to run it.

### Software Requirements
* Visual Studio 2019 Community Edition
* [Unity 2019.3.12f1](https://unity3d.com/get-unity/download/archive)
* [Blender 2.8](https://download.blender.org/release/)
  * If no models are showing up, you need to install blender, .blend files are used in unity
  * Models might take a while before showing up after installing blender, it has to "reimport" the .blend files before they render.

After installation you can open the project in unity, and in the asset menu you can "Open C# Project" to open visual studio
* The C# Solution is constructed every time you open the project with Unity.

* Merges in git
    * https://www.youtube.com/watch?v=yQvbaBgxA34
    * https://docs.unity3d.com/Manual/SmartMerge.html
	* open cli run `git mergetool`

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change. If there is an existing issue you would like to tackle, please mention that in the issue to allow others to collab with you :)

## Feature Comparison with official client
This is an attempt at a feature comparison table.

| Feature | Official | 3D |
|---------|----------|----|
| Connect to official server | :heavy_check_mark: | :heavy_check_mark: |
| Connect to private server | :heavy_check_mark: | :heavy_check_mark: |
| Simulation | :heavy_check_mark: |  |
| Visualization of nuke arcs |  | :heavy_check_mark: |
| Nuke list |  | :heavy_check_mark: |
| Market | :heavy_check_mark: |  |
| Overview | :heavy_check_mark: |  |
| Stats | :heavy_check_mark: |  |
| Room View | :heavy_check_mark: | :heavy_check_mark: |
| Seamless transition to rooms|  | :heavy_check_mark: |
| Map View | :heavy_check_mark: | :heavy_check_mark: |
| Spawn invaders | :heavy_check_mark: |  |
| Place spawn | :heavy_check_mark: |  |
| Place construction sites | :heavy_check_mark: |  |
| Place flags | :heavy_check_mark: | :heavy_check_mark: |
| Room Visuals | :heavy_check_mark: |  |

### Rendering of Room Objects
| Object | Official | 3D |
|---------|----------|----|
| construction site | :heavy_check_mark: | :heavy_check_mark: |
| creep | :heavy_check_mark: | :heavy_check_mark: |
| deposit | :heavy_check_mark: | :heavy_check_mark: |
| flag | :heavy_check_mark: | :heavy_check_mark: |
| mineral | :heavy_check_mark: | :heavy_check_mark: |
| Nuke | :heavy_check_mark: | :white_check_mark: |
| PowerCreep | :heavy_check_mark: | :heavy_check_mark: |
| Resource | :heavy_check_mark: | :heavy_check_mark: |
| Ruin | :heavy_check_mark: | :heavy_check_mark: |
| Source | :heavy_check_mark: | :heavy_check_mark: |
| Container | :heavy_check_mark: | :heavy_check_mark: |
| Controller | :heavy_check_mark: | :heavy_check_mark: |
| Extension | :heavy_check_mark: | :heavy_check_mark: |
| Extractor | :heavy_check_mark: | :heavy_check_mark: |
| Factory | :heavy_check_mark: | :heavy_check_mark: |
| InvaderCore | :heavy_check_mark: | :heavy_check_mark: |
| Keeper Lair | :heavy_check_mark: | :heavy_check_mark: |
| Lab | :heavy_check_mark: | :heavy_check_mark: |
| Link | :heavy_check_mark: | :heavy_check_mark: |
| Nuker | :heavy_check_mark: | :heavy_check_mark: |
| Observer | :heavy_check_mark: | :heavy_check_mark: |
| Power Bank | :heavy_check_mark: | :heavy_check_mark: |
| Power Spawn | :heavy_check_mark: | :heavy_check_mark: |
| Portal | :heavy_check_mark: | :heavy_check_mark: |
| Rampart | :heavy_check_mark: | :heavy_check_mark: |
| Road | :heavy_check_mark: | :heavy_check_mark: |
| Spawn | :heavy_check_mark: | :heavy_check_mark: |
| Storage | :heavy_check_mark: | :heavy_check_mark: |
| Terminal | :heavy_check_mark: | :heavy_check_mark: |
| Tower | :heavy_check_mark: | :heavy_check_mark: |
| Wall | :heavy_check_mark: | :heavy_check_mark: |
| Tombstone | :heavy_check_mark: | :heavy_check_mark: |
| Warp Container| :white_check_mark: |  |
