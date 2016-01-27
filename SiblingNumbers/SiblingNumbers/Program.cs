using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiblingNumbers
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(FindMaxSibling(43864));
        }

        static int FindMaxSibling(int i)
        {
            var s = i.ToString();
            s = new string(s.OrderByDescending(c => (int)c).ToArray());           
            return Int32.Parse(s);
        }
    }
}
