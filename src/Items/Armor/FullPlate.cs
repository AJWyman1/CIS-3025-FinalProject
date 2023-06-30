using System;

class FullPlate : Armor
{
    public FullPlate ()
    {
        this.ArmorType = new string[] {"heavy armor"};
        this.StealthDisadvantage = false;
        this.DexModifier = false;
        this.ArmorClass = 18;
    }
}