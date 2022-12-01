using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class Level2Monster : Creature {

    public int XP { get; protected set; }
    public Level2Monster()
    {
        this.GoldPouch = Dice.Roll(2, 6);
        this.Inventory = new Container<Item>(3);
        this.XP = 150;
    }
}