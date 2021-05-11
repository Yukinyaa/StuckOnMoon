using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Lehmer_random_number_generator implimentation 
public class SerializableDeterministicRNG
{
    public uint State { get; private set; }    /* must not be zero */
    public uint Next()
    {
        return State = (State * 48271) % 0x7fffffff;
    }

    /// <summary>
    /// Return ranged random. recommened to stay at small number.
    /// </summary>
    /// <param name="min">min value, inclusive</param>
    /// <param name="max">max value, exclusive</param>
    /// <returns></returns>
    public int NextRange(int min, int max)
    {
        return (int)(Next() % (max - min)) + min;
    }
}
