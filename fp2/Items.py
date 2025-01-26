from itertools import groupby
from typing import Dict, Set, NamedTuple
from BaseClasses import ItemClassification
#All the imports, as usual.

class FP2ItemData(NamedTuple):
    classification: ItemClassification
    quantity_in_item_pool: int
    item_id_offset: int #Not sure how FP2 offsets are treated.
    item_group: str = ""


item_base_id = 9132022000 #Need to determine this. Gotta be in the Unity code somewhere.

#I am not sure if EVERY Star Card, and EVERY Time Capsule are thier own value in FP2.
#I am going to treat it here like Star Cards and Capsules like lump sums.
#May have to rename some of these later to reflect with the patcher. Format seems to be: 
#"Name_of_Item": FP2ItemData(ItemClassification.Classification, quantity_in_world, order, "Item_Group" ),
item_table: Dict[str, FP2ItemData] = {
    "Star Card": FP2ItemData(ItemClassification.progression, 64, 0,"Star Cards"),
    "Time Capsule": FP2ItemData(ItemClassification.progression, 12, 1, "Time Capsules"),
    "Potion - Extra Stock": FP2ItemData(ItemClassification.useful, 1, 2, "Potions"),
    "Potion - Strong Revivals": FP2ItemData(ItemClassification.useful, 1, 3,  "Potions"),
    "Potion - Cheaper Stocks": FP2ItemData(ItemClassification.useful, 1, 4,  "Potions"),
    "Potion - Healing Strike": FP2ItemData(ItemClassification.useful, 1, 5, "Potions"),
    "Potion - Attack Up": FP2ItemData(ItemClassification.useful, 1, 6, "Potions"),
    "Potion - Strong Shields": FP2ItemData(ItemClassification.useful, 1, 7, "Potions"),
    "Potion - Accelerator": FP2ItemData(ItemClassification.useful, 1, 8, "Potions"),
    "Potion - Super Feather": FP2ItemData(ItemClassification.useful, 1, 9, "Potions"),
    "Element Burst": FP2ItemData(ItemClassification.useful, 1, 10, "Brave Stones"),
    "Max Life Up": FP2ItemData(ItemClassification.useful, 1, 11, "Brave Stones"),
    "Crystals to Petals": FP2ItemData(ItemClassification.useful, 1, 12, "Brave Stones"),
    "Powerup Start": FP2ItemData(ItemClassification.useful, 1, 13, "Brave Stones"),
    "Shadow Guard": FP2ItemData(ItemClassification.useful, 1, 14, "Brave Stones"),
    "Payback Ring": FP2ItemData(ItemClassification.useful, 1, 15, "Brave Stones"),
    "Wood Charm": FP2ItemData(ItemClassification.useful, 1, 16, "Brave Stones"),
    "Earth Charm": FP2ItemData(ItemClassification.useful, 1, 17, "Brave Stones"),
    "Water Charm": FP2ItemData(ItemClassification.useful, 1, 18, "Brave Stones"),
    "Fire Charm": FP2ItemData(ItemClassification.useful, 1, 19, "Brave Stones"),
    "Metal Charm": FP2ItemData(ItemClassification.useful, 1, 20, "Brave Stones"),
    "No Stocks": FP2ItemData(ItemClassification.useful, 1, 21, "Brave Stones"),
    "Expensive Stocks": FP2ItemData(ItemClassification.useful, 1, 22, "Brave Stones"),
    "Double Damage": FP2ItemData(ItemClassification.useful, 1, 23, "Brave Stones"),
    "No Revivals": FP2ItemData(ItemClassification.useful, 1, 24, "Brave Stones"),
    "No Nuarding": FP2ItemData(ItemClassification.useful, 1, 25, "Brave Stones"),
    "No Petals": FP2ItemData(ItemClassification.useful, 1, 26, "Brave Stones"),
    "Time Limit": FP2ItemData(ItemClassification.useful, 1, 27, "Brave Stones"),
    "Items to Bombs": FP2ItemData(ItemClassification.useful, 1, 28, "Brave Stones"),
    "Life Oscillation": FP2ItemData(ItemClassification.useful, 1, 29, "Brave Stones"),
    "One Hit KO": FP2ItemData(ItemClassification.useful, 1, 30, "Brave Stones"),
    "Petal Armor": FP2ItemData(ItemClassification.useful, 1, 31, "Brave Stones"),
    "Rainbow Charm": FP2ItemData(ItemClassification.useful, 1, 32, "Brave Stones"),
    "Gold Gem x1": FP2ItemData(ItemClassification.filler, 1, 33, "Currency"),
    "Crystals x1000": FP2ItemData(ItemClassification.filler, 1, 34, "Currency"),
    #"Robot Cores x 20": FP2ItemData(ItemClassification.filler, 1, 35, "Currency"),
    #I'm putting these here because I wasn't sure where to put/if this could be done.
    "Extra Item Slot": FP2ItemData(ItemClassification.useful, 2, 35, "Item Slots"),
    "Extra Potion Slots": FP2ItemData(ItemClassification.useful, 1, 36, "Item Slots"),
    #Chapter Unlocks: I suppose that's how we'll "gate" progression in this game, besides Star Cards.
    "Sky Pirate Panic": FP2ItemData(ItemClassification.progression, 1, 37, "Chapter Unlocks"),
    "Enter the Battlesphere": FP2ItemData(ItemClassification.progression, 1, 38, "Chapter Unlocks"),
    "Mystery of the Frozen North": FP2ItemData(ItemClassification.progression, 1, 39, "Chapter Unlocks"),
    "Globe Opera": FP2ItemData(ItemClassification.progression, 1, 40, "Chapter Unlocks"),
    "Robot Wars! Snake VS Tarsier": FP2ItemData(ItemClassification.progression, 1, 41, "Chapter Unlocks"),
    "Echoes of the Dragon War": FP2ItemData(ItemClassification.progression, 1, 42, "Chapter Unlocks"),
    "Justice in the Sky Paradise": FP2ItemData(ItemClassification.progression, 1, 43, "Chapter Unlocks"),
    "Bakunawa": FP2ItemData(ItemClassification.progression, 1, 44, "Chapter Unlocks"),
    #"Progressive Chapter": FP2ItemData(ItemClassification.progression, 8, 45, "Progressive Chapter Unlocks"),
    "Mirror Trap": FP2ItemData(ItemClassification.trap, 1, 45, "Traps"),
    "Moon Gravity Trap": FP2ItemData(ItemClassification.trap, 1, 46, "Traps"),
    "Double Gravity Trap": FP2ItemData(ItemClassification.trap, 1, 47, "Traps"),
}    
    #"Pie Trap": FP2ItemData(ItemClassification.trap, 1, 48, "Traps"), #This would be a good one.
    #Vinyls
    #"Vinyl - Dragon Valley": FP2ItemData(ItemClassification.filler, 1, 49, "Vinyl"),
    #"Vinyl - Shenlin Park": FP2ItemData(ItemClassification.filler, 1, 50, "Vinyl"),
    #"Vinyl - Avian Museum": FP2ItemData(ItemClassification.filler, 1, 51, "Vinyl"),
    #"Vinyl - Airship Sigwada": FP2ItemData(ItemClassification.filler, 1, 52, "Vinyl"),
    #"Vinyl - Phoenix Highway": FP2ItemData(ItemClassification.filler, 1, 53, "Vinyl"),
    #"Vinyl - Zao Land": FP2ItemData(ItemClassification.filler, 1, 54, "Vinyl"),
    #"Vinyl - Tiger Falls": FP2ItemData(ItemClassification.filler, 1, 55, "Vinyl"),
    #"Vinyl - Robot Graveyard": FP2ItemData(ItemClassification.filler, 1, 56, "Vinyl"),
    #"Vinyl - Shade Armory": FP2ItemData(ItemClassification.filler, 1, 57, "Vinyl"),
    #"Vinyl - Globe Opera 1": FP2ItemData(ItemClassification.filler, 1, 58, "Vinyl"),
    #"Vinyl - Globe Opera 2A": FP2ItemData(ItemClassification.filler, 1, 59, "Vinyl"),
    #"Vinyl - Globe Opera 2B": FP2ItemData(ItemClassification.filler, 1, 60, "Vinyl"),
    #"Vinyl - Palace Courtyard": FP2ItemData(ItemClassification.filler, 1, 61, "Vinyl"),
    #"Vinyl - Tidal Gate": FP2ItemData(ItemClassification.filler, 1, 62, "Vinyl"),
    #"Vinyl - Zulon Jungle": FP2ItemData(ItemClassification.filler, 1, 63, "Vinyl"),
    #"Vinyl - Nalao Lake": FP2ItemData(ItemClassification.filler, 1, 64, "Vinyl"),
    #"Vinyl - Ancestral Forge": FP2ItemData(ItemClassification.filler, 1, 65, "Vinyl"),
    #"Vinyl - Magma Starscape": FP2ItemData(ItemClassification.filler, 1, 66, "Vinyl"),
    #"Vinyl - Sky Bridge": FP2ItemData(ItemClassification.filler, 1, 67, "Vinyl"),
    #"Vinyl - Lightning Tower": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Gravity Bubble": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Bakunawa Rush": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Clockwork Arboretum": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Inversion Dynamo": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Lunar Cannon": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Title Screen": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Main Menu": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Basic Tutorial": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Globe Opera 2A": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Weapon's Core": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Robot A": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Robot B": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Aaa": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Pheonix Highway": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Zao Land": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Arena": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Captian Kalaw": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Serpentine A": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Serpentine B": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Beast One/Two": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Beast Three": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - BFF2000": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Diamond Point": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Arboretum": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Merga": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Merga (Pinch)": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Boss - Weapon's Core": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Stage Clear": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Results - Lilac": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Results - Carol": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Results - Milla": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Results - Neera": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Bonus Stage": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Speed Gate": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Shopping": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Map - Shang Tu": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Map - Shang Mu": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Map - Shuigang": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Map - Opera": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Map - Parusa": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Map - Floating Island": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Map - Volcano": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Map - Bakunawa": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Singing Water Temple": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Royal Palace": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Battlesphere Commercial": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Battlesphere Lobby": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Battlesphere Course": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Captian Kalaw's Theme": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Gallery": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Shuigang": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - City Hall": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Adventure Square": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Paradise Prime": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Big Mood A": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Big Mood B": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Heroic": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Preparation": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Bakunawa": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Lilac's Theme": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Carol's Theme": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Milla's Theme": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Neera's Theme": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Corazon's Theme": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Serpentine's Theme": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Merga's Theme": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Audio Log A": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Audio Log B": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),
    #"Vinyl - Cutscene - Audio Log C": FP2ItemData(ItemClassification.filler, 1, "Vinyl"),

filler_items: List[str] = ["Gold Gem", "Crystals x1000"]

item_name_to_id: Dict[str, int] = {name: item_base_id + data.item_id_offset for name, data in item_table.items()}

def get_item_group(item_name: str):
    return item_table[item_name].item_group

item_name_groups: Dict[str, Set[str]] = {
    group: set(item_names) for group, item_names in groupby(sorted(item_table, key=get_item_group), get_item_group) if group != ""
}