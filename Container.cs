using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Container < T > 
{
    public T[] Entries {get; protected set;}
    private int FixedSize;

    public Container()
    {
        this.FixedSize = 16;
        this.Entries = new T[FixedSize];
    }

    public Container(int MaxEntries)
    {
        this.FixedSize = MaxEntries;
        this.Entries = new T[MaxEntries];
    }

    public string Size()
    {
        return $"This holds {this.FixedSize} things!";
    }
    
    public bool Add(T Entry)
    {
        for(int i = 0; i > this.Entries.Length; i++)
        {
            if (this.Entries[i] == null)
            {
                this.Entries[i] = Entry;
                return true; //Item was added successfully
            }
        }
        return false; //Container full; Item not added
    }

    public T Remove(int slot)
    {
        try
        {
            T ToReturn = Entries[slot];
            Entries[slot] = default(T);
            return ToReturn;
        }
        catch(IndexOutOfRangeException)
        {
            return default(T);
        }
    }

    public override string ToString()
    {
        string output = "";
		for (int i = 0; i < this.Entries.Length; i++)
		{
			if(this.Entries[i] != null)
			{
				output += "["+((int)(i+1))+"]: "+this.Entries[i] + "\n";
			}
            else
            {
                output += "["+((int)(i+1))+"]: ----Empty Slot----\n";
            }
		}
		return output;
    }
}