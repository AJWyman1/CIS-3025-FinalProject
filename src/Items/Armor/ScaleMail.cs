using System;

class ScaleMail : Armor
{
    public ScaleMail ()
    {
        this.ArmorType = new string[] {"medium armor"};
        this.StealthDisadvantage = true;
        this.DexModifier = true;
        this.MaxMod = 2;
        this.ArmorClass = 14;
    }
}