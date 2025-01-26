from dataclasses import dataclass
from Options import (Toggle, StartInventoryPool, DeathLink, PerGameCommonOptions)

#class ChapterUnlocks(Toggle):
#    """
#    Set whether Chapter Unlocks are progressive or not.
#    """
#    internal_name = "progressive_chapter_unlocks"
#    display_name =  "Progressive Chapter Unlocks"

@dataclass
class FP2Options(PerGameCommonOptions):
    start_inventory_from_pool: StartInventoryPool
    deathlink: DeathLink 
    #progressive_chapter_unlocks: ChapterUnlocks