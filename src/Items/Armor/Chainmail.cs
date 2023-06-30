using System;

class Chainmail : Armor
{
    public Chainmail ()
    {
        this.ArmorType = new string[] {"heavy armor"};
        this.StealthDisadvantage = false;
        this.DexModifier = false;
        this.ArmorClass = 16;
    }
}