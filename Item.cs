using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class Item: IUsable, ILocatable, IComparable
{
    /* ILocatable Properties */

    public int X {get; set;}
    public int Y {get; set;}
    public Direction Facing {get; set;}
    public char RepresentWith { get; set; }

    public string Name {get; set;}

    /*  IUsable Properties */
    public int UsesLeft { get; set; }
    public float UseChance { get; set; }   // range of 0.0 to 1.0


    /* Item Properties */

    public bool IsInInventory { get; set; }
    public int Weight { get; protected set; }


    public Item () 
    {

    }


    /*  IUsable Methods */

    public abstract string SuccessMessage();
    public abstract string FailureMessage();

    public virtual string Use(Creature c, int Slot)
    {
        this.UsesLeft -= 1;
        if (this.UseChance * 10 >= Dice.D10())
        {
            return this.SuccessMessage();
        }
        else
        {
            return this.FailureMessage();
        }

    }

    /* IComparable Method */

    public int CompareTo(object obj)
    {
        if (obj == null){return 1;}

        Item otherItem = obj as Item;
        if (otherItem != null)
            return this.Name.CompareTo(otherItem.Name); //Sort alphabetically
        else
            throw new ArgumentException("Not an Item");
    }

    public override string ToString()
    {
        return this.Name;
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

}