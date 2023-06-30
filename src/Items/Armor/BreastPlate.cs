using System;

class BreastPlate : Armor
{
    public BreastPlate ()
    {
        this.ArmorType = new string[] {"medium armor"};
        this.StealthDisadvantage = false;
        this.DexModifier = true;
        this.MaxMod = 2;
        this.ArmorClass = 14;
    }
}