namespace CardPile.CardData;

[Flags]
public enum Color : int
{
    None      = 0b000000,
    White     = 0b000001,
    Blue      = 0b000010,
    Black     = 0b000100,
    Red       = 0b001000,
    Green     = 0b010000,
    
    WU   = Color.White | Color.Blue,
    WB   = Color.White | Color.Black,
    WR   = Color.White | Color.Red,
    WG   = Color.White | Color.Green,
    UB   = Color.Blue  | Color.Black,
    UR   = Color.Blue  | Color.Red,
    UG   = Color.Blue  | Color.Green,
    BR   = Color.Black | Color.Red,
    BG   = Color.Black | Color.Green,
    RG   = Color.Red   | Color.Green,    
}
