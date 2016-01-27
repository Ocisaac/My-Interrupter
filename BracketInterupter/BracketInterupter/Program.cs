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

            try { var bfail = Bracket.Parse("(5(3)3)");}
            catch { Console.WriteLine("failed bf5"); }

            try { var bfail = Bracket.Parse("((6)())"); }
            catch { Console.WriteLine("failed bf6"); }

            var b4 = Bracket.ParseMany(Console.ReadLine()); 
            foreach (var b in b4)
                Console.WriteLine(b);                
            Console.ReadKey();
        }
    }
    class Bracket
    {
        public List<Bracket> inside = new List<Bracket>();
        public int? value= null;

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
        private static IEnumerable<Bracket> ParseAsIE(string s)
        {
            Regex bracketRegex = new Regex(@"^\(+[0-9\(\)]+\)+$");
            Regex invalidBracket = new Regex(@"[0-9]+\(");
            if (!bracketRegex.IsMatch(s))
                throw new InvalidOperationException();
            if (s.Where(c => c == '(').Count(c => true) != s.Where(c => c == ')').Count(c => true))
                throw new InvalidOperationException();
            if (invalidBracket.IsMatch(s))
                throw new InvalidOperationException("brackets with inner brackets cannot have values");


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
                    if (parseString == "")
                        throw new Exception("brackets must not be empty");

                    var b = new Bracket();
                    int v = 0;
                    bool suc = int.TryParse(parseString, out v); //TO DO TO DO
                    if (suc)
                    {
                        if (b.value == null)
                            b.value = v;
                        else
                            throw new Exception("cannot value twice");
                        //b.inside.Add(new Bracket(v));
                    }
                    else
                    {
                        b.inside = ParseAsIE(parseString).ToList();
                    }
                    yield return b;
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

            return ParseAsIE(s).ToArray();
        }
        /// <summary>
        /// not the best, but it will parse brackets when they are right
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Bracket Parse(string s)
        {
            Regex bracketRegex = new Regex(@"^\(+[0-9\(\)]+\)+$");
            Regex invalidBracket = new Regex(@"[0-9]+\(");
            if (!bracketRegex.IsMatch(s))
                throw new InvalidOperationException();
            if (s.Where(c => c == '(').Count(c => true) != s.Where(c => c == ')').Count(c => true))
                throw new InvalidOperationException();
            if (invalidBracket.IsMatch(s))
                throw new InvalidOperationException("brackets with inner brackets cannot have values");

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
                    if (parseString == "")
                        throw new Exception("brackets must not be empty");


                    int v = 0;
                    bool suc = int.TryParse(parseString, out v); //TO DO TO DO
                    if (suc)
                    {
                        if (b.value == null)
                            b.value = v;
                        else 
                            throw new Exception("A bracket cannot have more then one value");
                        //b.inside.Add(new Bracket(v));
                    }
                    else
                        b.inside.InsertRange(b.inside.Count, ParseAsIE(parseString));
                    startingpos = ++loc;
                    parseString = "";
                }
            }
            return b;
        }
        
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
