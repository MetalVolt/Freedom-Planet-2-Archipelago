#Sample Archipelago World _init_.py for Freedom Planet 2
"""
Archipelago World definition for Freedom Planet 2
"""
#Standard imports, may or may not need some of these.
#Can decide that later.
import copy
import os
from worlds.AutoWorld import World, WebWorld
from worlds.generic.Rules import set_rule, forbid_item
from typing import Dict, List, Any, Tuple, TypedDict, ClassVar, Union
from BaseClasses import Tutorial, MultiWorld, Location, Item, ItemClassification
from .Items import item_name_to_id, item_name_groups, item_table, filler_items  #put here as needed.
from .Locations import location_name_to_id, location_name_groups, location_table, FP2Location
from .Regions import FP2_Regions
from .Options import FP2Options
#from Fill import fill_restrictive, FillError

#The WebWorld, need this for AP website things... eventually. Still have to have this now though.
#class FreedomPlanet2(WebWorld): 
#I feel like calling the WebWorld "WebbingACarol(WebWorld)" for fun, AHiT does something like this.    
    #"""
    #Webhost info for Freedom Planet 2
    #"""
    #theme = "ocean"
    #setup_en = Tutorial(
     #   "Multiworld Setup Guide",
     #   "A guide to Playing Freedom Planet 2 with Archipelago.",
     #   "English",
     #   "setup_en.md",
     #   "setup/en",
     #   #I, (echo) I, Your Name,  #On a serious note, whoever will be doing the guides goes here.
     #   ["YourName"]
    #)

    #tutorials = [setup_en]

class FP2Item(Item):
    game = "Freedom Planet 2"

class FP2Location(Location):
    game = "Freedom Planet 2"

class FreedomPlanet2World(World):
    """
    Freedom Planet 2 is a sequel to the 2014 indie platformer Freedom Planet.
    Four playable characters with unique movesets aim to find the Star Cards.
    These and other helpful items will allow you to face Avalice's greatest threat yet, Merga.
    """

    game = "Freedom Planet 2"
    #Might be a few ways to do the next lines, likely depends on "items.py".
    item_name_to_id = item_name_to_id #This depends on how FP2 has item data structured.
    location_name_to_id = location_name_to_id #Largely same as above, if we need it.
    options_dataclass = FP2Options
    options: FP2Options
    item_name_groups = item_name_groups  #May need to be set in game's "items.py", also if we even need it. For now just listing them here.
    location_name_groups = location_name_groups  #if we need, can be uncommented.

    def create_regions(self) -> None:
        for region_name in FP2_Regions.keys():
            region = FP2_Regions(region_name, self.player, self.multiworld)
            self.multiworld.regions.append(region)

        for region_name, region_data in FP2_Regions.items():
            region = self.multiworld.get_region(region_name, self.player)
            region.add_locations({})

        for location_name, location_id in self.location_name_to_id.items():
            region = self.multiworld.get_region(location_table[location_name].region, self.player)
            location = FP2Location(self.player, location_name, location_id, region)
            region.locations.append(location)

        victory_region = self.multiworld.get_region("Weapon's Core", self.player)
        victory_location = location_table(self.player, "Weapon's Core - Clear", None, victory_region)
        victory_location.place_locked_item(FP2Item("Goal", ItemClassification.progression, None, self.player))
        self.multiworld.completion_condition[self.player] = lambda state: state.has("Goal", self.player)
        victory_region.locations.append(victory_location)

    def create_item(self, name: str, classification: ItemClassification = None) -> FP2Item:
        item_data = item_table[name]
        return FP2Item(name, classification or item_data.classification, self.item_name_to_id[name], self.player)
        #if Progressive Chapter Unlocks:
        #if self.options.progressive_chapter_unlocks:
        #    return FP2Item(name, classification, item_table[name].code, self.player)
        #else:
        #    return FP2Item(name, item_table[name].type, item_table[name].code, self.player)

    def create_items(self) -> None:

        self.fp2_items: List[FP2Item] = []
        self.slot_data_items = []

        self.items_to_create: Dict[str, int] = {item: data.quantity_in_item_pool for item, data in item_table.items()}

        def remove_filler(amount: int) -> None:
            for _ in range(amount):
                self.items_to_create[fill] -= 1
                if self.items_to_create[fill] == 0:
                    filler_items.remove(fill)

    def set_rules(self) -> None:
        set_rule(self.multiworld.get_entrance("Avian Museum", self.player),
            lambda state: state.has("Sky Pirate Panic", self.player))
        set_rule(self.multiworld.get_entrance("Airship Sigwada", self.player),
            lambda state: state.has("Sky Pirate Panic", self.player))
        set_rule(self.multiworld.get_entrance("Phoenix Highway", self.player),
            lambda state: state.has("Enter the Battlesphere", self.player))
        set_rule(self.multiworld.get_entrance("Zao Land", self.player),
            lambda state: state.has("Enter the Battlesphere", self.player))
        set_rule(self.multiworld.get_entrance("The Battlesphere - 1st Pass"),
            lambda state: state.has("Enter the Battlesphere", self.player))
        set_rule(self.multiworld.get_entrance("Tiger Falls", self.player),
            lambda state: state.has("Mystery of the Frozen North", self.player))
        set_rule(self.multiworld.get_entrance("Robot Graveyard", self.player),
            lambda state: state.has("Mystery of the Frozen North", self.player)) 
        set_rule(self.multiworld.get_entrance("Shade Armory", self.player),
            lambda state: state.has("Mystery of the Frozen North", self.player))
        set_rule(self.multiworld.get_entrance("Globe Opera 1", self.player),
            lambda state: state.has("Globe Opera", self.player) and
                          state.has("Sky Pirate Panic", self.player) and
                          state.has("Enter the Battlesphere", self.player) and
                          state.had("Mystery of the Frozen North", self.player) and
                          state.has("Star Card", self.player, 11))
        set_rule(self.multiworld.get_entrance("Globe Opera 2", self.player),
            lambda state: state.has("Globe Opera", self.player) and
                          state.has("Sky Pirate Panic", self.player) and
                          state.has("Enter the Battlesphere", self.player) and
                          state.had("Mystery of the Frozen North", self.player) and
                          state.has("Star Card", self.player, 11))
        set_rule(self.multiworld.get_entrance("Auditorium", self.player),
            lambda state: state.has("Globe Opera", self.player) and
                          state.has("Sky Pirate Panic", self.player) and
                          state.has("Enter the Battlesphere", self.player) and
                          state.had("Mystery of the Frozen North", self.player) and
                          state.has("Star Card", self.player, 11))
        set_rule(self.multiworld.get_entrance("Palace Courtyard", self.player),
            lambda state: state.has("Globe Opera", self.player) and
                          state.has("Sky Pirate Panic", self.player) and
                          state.has("Enter the Battlesphere", self.player) and
                          state.had("Mystery of the Frozen North", self.player) and
                          state.has("Star Card", self.player, 11))
        set_rule(self.multiworld.get_entrance("Tidal Gate", self.player),
            lambda state: state.has("Globe Opera", self.player) and
                          state.has("Sky Pirate Panic", self.player) and
                          state.has("Enter the Battlesphere", self.player) and
                          state.had("Mystery of the Frozen North", self.player) and
                          state.has("Star Card", self.player, 11))    
               
    def fill_slot_data(self):
        return {
            "start_inventory_from_pool": self.options.start_inventory_from_pool,
            "deathlink": self.options.deathlink.value,
        }