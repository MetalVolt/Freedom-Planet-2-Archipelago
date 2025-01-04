# Freedom Planet 2 Archipelago

A (rather hacky) implementation of Freedom Planet 2 into [Archipelago](https://archipelago.gg/), using the [Manual](https://github.com/ManualForArchipelago/Manual) system as I cannot for the life of me wrap my head around writing a proper apworld.

## Building

First off, ensure that your system has [Visual Studio 2022](https://visualstudio.microsoft.com/) installed alongside the `.NET Framework 3.5 development tools`, as well as [Unity 5.6.3](https://unity.com/releases/editor/whats-new/5.6.3#installs).

Open the solution file in VS2022 then go to `Tools > Options` and select `Package Sources` under the `NuGet Package Manager` category. Then add a package source called `BepInEx` with the source url set to `https://nuget.bepinex.dev/v3/index.json`.

Next, go to the `Assemblies` category in the `Dependencies` for the project, then delete the `Assembly-CSharp` and `Rewired_Core` references. Right click on the Assemblies category and click `Add Assembly Reference...`, then click `Browse...` and navigate to Freedom Planet 2's install directory. Open the `FP2_Data` directory, then the `Managed` directory and select the `Assembly-CSharp.dll` and `Rewired_Core.dll` files. Click Add, then OK.

You should now be able to right click the solution and choose `Rebuild` to build the mod. Though it is recommended to change the build configuration from `Debug` to `Release`, as the debug build prints a lot more console messages, some of which spoil what items are at a location.

## Installing

First off, install [BepInEx 5](https://github.com/BepInEx/BepInEx/releases/latest) into Freedom Planet 2 (make sure to use the x86 version, as the game is a 32-bit application rather than a 64-bit one). Then create an `Archipelago` folder in the `BepInEx/plugins` directory. Copy the `Archipelago.MultiClient.Net`, `Freedom_Planet_2_Archipelago` and `Newtonsoft.Json` DLLs from the build (`bin/Debug/net35` or `bin/Release/net35`) into this folder. Then grab the `clang32` version of [c-wspp-websocket-sharp](https://github.com/black-sliver/c-wspp-websocket-sharp/releases/latest) and extract the `c-wspp` and `websocket-sharp` DLLs to this folder as well.

It is also HEAVILY recommended to enable the BepInEx console. To do this, go to the `BepInEx/config` and open `BepInEx.cfg` in a text editor. There, find the `Enabled = false` line under `[Logging.Console]` and change it to `Enabled = true`.

## Connecting

Upon running the game, it should load to a basic connection screen consisting of three textboxes. Change the contents of the first textbox to the address of the Archipelago server that is running a multiworld with a Freedom Planet 2 game. Change the second textbox to your Slot Name, then put the server password in the third box (if the server has no password, then leave it blank.)

Upon pressing Connect, the game will freeze for a short while as it gets the starting information from the server. Checking the console will show multiple lines reading `[Info   :Freedom_Planet_2_Archipelago] Getting information for location [locationName] with an index of [locationIndex]`. Once this is done, the game will go to the Classic Mode map screen.

## Custom Content

### Music

By default, an option for custom music is enabled (this can be disabled by changing the `Enable Music Randomiser = true` line in `FP2_AP.cfg` (found in the same directory as BepInEx's own configuration file) to `Enable Music Randomiser = false`). To use this, place any music you want into the `mod_overrides\Archipelago\Audio\music` directory as an OGG Vorbis file. To control when the music loops, create a file in the same directory called `loop_points.txt` and open it in a text editor. Add a line to it with the file name, the start loop point (in samples) and end loop point (also in samples).

**Example:**

`S4EP1_15_Boss1_Mixed4816_wav|53261|1546750`

![](https://raw.githubusercontent.com/Knuxfan24/Freedom-Planet-2-Archipelago/refs/heads/master/readme_imgs/custom_music.png)
*An example of a populated custom music directory and approriate loop_points.txt file.*

Custom 1UP, Stage Clear and Invincibility Jingles can also be added in a similar manner, although these do not use loop points and go in different directories within the `mod_overrides\Archipelago\Audio\jingles` directory.

![](https://raw.githubusercontent.com/Knuxfan24/Freedom-Planet-2-Archipelago/refs/heads/master/readme_imgs/custom_jingles.png)
*An example of populated custom jingle directories.*

This custom audio is loaded when the connect screen is and will also print the total amount of custom music to the console.

### Item Sprites and Descriptions

By default, items for other games will appear as the generic Archipelago logo, and shop items will simply say who the item is for. To get sprites and descriptions for other games, create a folder in `mod_overrides\Archipelago\Sprites` with the game's name. Then place PNG files with the item names into the new folder. For descriptions, create a file called `descriptions.txt` and add a line with the item name, followed by a comma, then the desired description.

**Example:**

`Power Star,Stars that unlock doors and can dispell magic stairs.`

![](https://raw.githubusercontent.com/Knuxfan24/Freedom-Planet-2-Archipelago/refs/heads/master/readme_imgs/custom_sm64items.png)
*An example of a populated directory for Super Mario 64, showing sprites for various items, as well a descriptions.txt file for some of them.*

Custom sprites should ideally max out at 32x32 pixels.