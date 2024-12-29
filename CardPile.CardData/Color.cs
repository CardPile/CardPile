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
    
    WUB = Color.White | Color.Blue | Color.Black,
    WUR = Color.White | Color.Blue | Color.Red,
    WUG = Color.White | Color.Blue | Color.Green,
    WBR = Color.White | Color.Black | Color.Red,
    WBG = Color.White | Color.Black | Color.Green,
    WRG = Color.White | Color.Red | Color.Green,
    UBR = Color.Blue | Color.Black | Color.Red,
    UBG = Color.Blue | Color.Black | Color.Green,
    URG = Color.Blue | Color.Red | Color.Green,
    BRG = Color.Black | Color.Red | Color.Green,
    
    WUBR = Color.White | Color.Blue | Color.Black | Color.Red,
    WUBG = Color.White | Color.Blue | Color.Black | Color.Green,
    WURG = Color.White | Color.Blue | Color.Red | Color.Green,
    WBRG = Color.White | Color.Black | Color.Red | Color.Green,
    UBRG = Color.Blue | Color.Black | Color.Red | Color.Green,
    
    WUBRG = Color.White | Color.Blue | Color.Black | Color.Red | Color.Green,
}
