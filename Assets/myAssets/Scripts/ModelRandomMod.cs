using System.Collections.Generic;

namespace TwdCustomMod
{
    public class ModelRandomMod
    {
        public int State { get; set; }

        public int CallCount { get; set; }

        public int InitialSeed { get; set; }

        public ModelRandomMod()
        {
        }

        public ModelRandomMod(ModelRandomMod random)
        {
            State = random.State;
            CallCount = random.CallCount;
            InitialSeed = random.InitialSeed;
        }

        public ModelRandomMod(int seed)
        {
            InitialSeed = seed;
            State = seed;
            Next();
        }

        public float Next()
        {
            CallCount++;
            State = (State * 1103515245 + 12345) & 0x7FFFFFFF;
            return (float)((State >> 4) & 0xFFFFF) / 1048576f;
        }

        public int Next(int n)
        {
            CallCount++;
            State = (State * 1103515245 + 12345) & 0x7FFFFFFF;
            return (State >> 4) % n;
        }

        public int GetRandomInRange(int min, int max)
        {
            return Next(max - min + 1) + min;
        }

        public T GetRandomElement<T>(List<T> list, bool remove)
        {
            int index = Next(list.Count);
            T result = list[index];
            if (remove)
            {
                list.RemoveAt(index);
            }

            return result;
        }
    }
}
