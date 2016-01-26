using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BraketPrasing
{
    class Program
    {
        static void Main(string[] args)
        {
            //testing
            var b1 = Bracket.Parse("(4)"); //works
            Console.WriteLine(b1.ToString());

            var b2 = Bracket.Parse("((7))"); // works
            Console.WriteLine(b2.ToString());

            var b3 = Bracket.Parse("((6)(3))"); //works
            Console.WriteLine(b3.ToString());


            try { var bfail = Bracket.Parse("(("); }
            catch { Console.WriteLine("failed bf1"); }

            try { var bfail = Bracket.Parse("(a)"); }
            catch { Console.WriteLine("failed bf2"); }

            try { var bfail = Bracket.Parse("hello"); }
            catch { Console.WriteLine("failed bf3"); }

            try { var bfail = Bracket.Parse("(()"); }
            catch { Console.WriteLine("failed bf4"); }

            var b4 = Bracket.ParseMany(Console.ReadLine()); 
            foreach (var b in b4)
            {
                Console.WriteLine(b);
            }
            Console.ReadKey();
        }
    }



    class Bracket
    {
        public List<Bracket> inside = new List<Bracket>();
        public int? value;

        public Bracket()
        {

        }

        public Bracket(int val)
        {
            value = val;
        }

        public override string ToString()
        {
            string rtn = value == null ? "(" : "([" + value + "]";
            for (int i = 0; i < this.inside.Count - 1; i++)
                rtn += this.inside[i].ToString() + "|";
            if (this.inside.Count != 0)
                rtn += this.inside[this.inside.Count - 1].ToString();
            rtn += ")";
            return rtn;
        }

        #region parsing
        /// <summary>
        /// not the best, but it will parse brackets when they are right
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static IEnumerable<Bracket> ParseAsArr(string s)
        {
            int bracketCount = 0;
            int loc = 0;
            string parseString = "";
            int startingpos = 0;
            foreach (var c in s)
            {
                if (c == '(')
                    bracketCount++;
                else if (c == ')')
                    bracketCount--;
                if (loc != startingpos && bracketCount != 0)
                    parseString += c;
                loc++;
                if (bracketCount == 0)
                {
                    int v = 0;
                    bool suc = int.TryParse(parseString, out v); //TO DO TO DO
                    if (suc)
                    {
                        yield return new Bracket(v);
                        //b.inside.Add(new Bracket(v));
                    }
                    else
                    {
                        var b = new Bracket();
                        b.inside = ParseAsArr(parseString).ToList();
                        yield return b;
                    }
                    startingpos = ++loc;
                    parseString = "";
                }
            }
        }
        /// <summary>
        /// not the best, but it will parse brackets when they are right
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Bracket[] ParseMany(string s)
        {
            Regex bracketRegex = new Regex(@"^\(+[0-9\(\)]+\)+$");
            if (!bracketRegex.IsMatch(s))
                throw new InvalidOperationException();
            if (s.Where(c => c == '(').Count(c => true) != s.Where(c => c == ')').Count(c => true))
                throw new InvalidOperationException();

            return ParseAsArr(s).ToArray();
        }
        /// <summary>
        /// not the best, but it will parse brackets when they are right
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Bracket Parse(string s)
        {
            Regex bracketRegex = new Regex(@"^\(+[0-9\(\)]+\)+$");

            if (!bracketRegex.IsMatch(s))
                throw new InvalidOperationException();
            if (s.Where(c => c == '(').Count(c => true) != s.Where(c => c == ')').Count(c => true))
                throw new InvalidOperationException();

            Bracket b = new Bracket();
            int bracketCount = 0;
            int loc = 0;
            string parseString = "";
            int startingpos = 0;
            foreach (var c in s)
            {
                if (c == '(')
                    bracketCount++;
                else if (c == ')')
                    bracketCount--;
                if (loc != startingpos && bracketCount != 0)
                    parseString += c;
                loc++;
                if (bracketCount == 0)
                {
                    int v = 0;
                    bool suc = int.TryParse(parseString, out v); //TO DO TO DO
                    if (suc)
                    {
                        b.value = v;
                        //b.inside.Add(new Bracket(v));
                    }
                    else
                        b.inside.InsertRange(b.inside.Count, ParseAsArr(parseString));
                    startingpos = ++loc;
                    parseString = "";
                }
            }
            return b;
        }
        /// <summary>
        /// not the best, but it will parse brackets when they are right
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool TryParse(string s, out Bracket result)
        {
            result = null;
            try
            {
                result = Parse(s);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// not the best, but it will parse brackets when they are right
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool TryParseMany(string s, out Bracket[] result)
        {
            result = null;
            try
            {
                result = ParseMany(s);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
