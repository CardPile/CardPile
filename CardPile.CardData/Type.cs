namespace CardPile.CardData;

[Flags]
public enum Type
{
    None         = 0b000000000000,
    Artifact     = 0b000000000001,
    Creature     = 0b000000000010,
    Enchantment  = 0b000000000100,
    Instant      = 0b000000001000,
    Land         = 0b000000010000,
    Planeswalker = 0b000000100000,
    Sorcery      = 0b000001000000,
    Kindred      = 0b000010000000,
    Dungeon      = 0b000100000000,
    Battle       = 0b001000000000,
}