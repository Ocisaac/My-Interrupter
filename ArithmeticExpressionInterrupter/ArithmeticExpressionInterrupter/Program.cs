using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ArithmeticExpressionInterrupter
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                ArithmeticEx ex1 = ArithmeticEx.Parse(Console.ReadLine());
                Console.WriteLine("Calculation Result: " + ex1.LoggerCalculate(""));
                Console.WriteLine("================================================================================");
            }
        }
    }

    enum Operation
    {
        Addtion,
        Subtrubtion,
        Multiplication,
        Division,
        Power,
        Value
    }

    class ArithmeticEx
    {
        public ArithmeticEx LeftSide { get; }
        public Operation Operation { get; }
        public ArithmeticEx RightSide { get; }
        public float? Value;

        ArithmeticEx(float v)
        {
            Value = v;
            Operation = Operation.Value;
        }

        ArithmeticEx(ArithmeticEx arExLeft, Operation op, ArithmeticEx arExRight)
        {
            LeftSide = arExLeft;
            Operation = op;
            RightSide = arExRight;
        }

        public float LoggerCalculate(string indetion)
        {
            switch (Operation)
            {
                case Operation.Value:
                    Console.WriteLine(indetion + "Looking at value " + (float)Value);
                    Thread.Sleep(2000);
                    return (float)Value;

                case Operation.Addtion:
                    Console.WriteLine(indetion + $"Looking at {LeftSide.LoggerCalculate(indetion + "|")} + {RightSide.LoggerCalculate(indetion + "|")}");
                    Console.WriteLine(indetion + (LeftSide.Calculate() + RightSide.Calculate()));
                    Thread.Sleep(2000);
                    return LeftSide.Calculate() + RightSide.Calculate();

                case Operation.Subtrubtion:
                    Console.WriteLine(indetion + $"Looking at {LeftSide.LoggerCalculate(indetion + "|")} - {RightSide.LoggerCalculate(indetion + "|")}");
                    Console.WriteLine(indetion + (LeftSide.Calculate() - RightSide.Calculate()));
                    Thread.Sleep(2000);
                    return LeftSide.Calculate() - RightSide.Calculate();

                case Operation.Multiplication:
                    Console.WriteLine(indetion + $"Looking at {LeftSide.LoggerCalculate(indetion + "|")} * {RightSide.LoggerCalculate(indetion + "|")}");
                    Console.WriteLine(indetion + (LeftSide.Calculate() * RightSide.Calculate()));
                    Thread.Sleep(2000);
                    return LeftSide.Calculate() * RightSide.Calculate();

                case Operation.Division:
                    Console.WriteLine(indetion + $"Looking at {LeftSide.LoggerCalculate(indetion + "|")} / {RightSide.LoggerCalculate(indetion + "|")}");
                    Console.WriteLine(indetion + (LeftSide.Calculate() / RightSide.Calculate()));
                    Thread.Sleep(2000);
                    return LeftSide.Calculate() / RightSide.Calculate();

                case Operation.Power:
                    Console.WriteLine(indetion + $"Looking at {LeftSide.LoggerCalculate(indetion + "|")} ^ {RightSide.LoggerCalculate(indetion + "|")}");
                    Console.WriteLine(indetion + Math.Pow(LeftSide.Calculate(),RightSide.Calculate()));
                    Thread.Sleep(2000);
                    return (float)Math.Pow(LeftSide.Calculate(), RightSide.Calculate());

                default:
                    throw new Exception();
            }
        }

        public float Calculate()
        {
            switch (Operation)
            {
                case Operation.Value:
                    return (float)Value;

                case Operation.Addtion:
                    return LeftSide.Calculate() + RightSide.Calculate();

                case Operation.Subtrubtion:
                    return LeftSide.Calculate() - RightSide.Calculate();

                case Operation.Multiplication:
                    return LeftSide.Calculate() * RightSide.Calculate();

                case Operation.Division:
                    return LeftSide.Calculate() / RightSide.Calculate();

                case Operation.Power:
                    return (float)Math.Pow(LeftSide.Calculate(), RightSide.Calculate());

                default:
                    throw new Exception();
            }
        }

        public static ArithmeticEx Parse(string s)
        {
            s = new string(s.Where(c => c != ' ').ToArray());
            float v = 0f;
            string InsideBrackets = "";
            int bracketCount = 0;
            if (s.StartsWith("(") && s.EndsWith(")"))
                return Parse(s.Substring(1, s.Length - 2));

            if (Single.TryParse(s, out v))
                return new ArithmeticEx(v);
            Regex arithExRegex = new Regex(@"^[\(\)\^\.\-0-9\+\*/-]+$");
            if (!arithExRegex.IsMatch(s))
                throw new Exception("invalid string");
            if (s.Count(c => c == '(') != s.Count(c => c == ')'))
        //        throw new Exception("parens must balance");

            if (s.StartsWith("-"))
                s = "0" + s;

            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == '(')
                    bracketCount++;
                if (s[i] == ')')
                    bracketCount--;
                if (bracketCount != 0)
                    InsideBrackets = s[i] + InsideBrackets;
                else
                {
                    if (s[i] == '+')
                        return new ArithmeticEx(
                            Parse(s.Substring(0, i)),
                            Operation.Addtion,
                            Parse(s.Substring(i + 1, s.Length - (i + 1))));

                    if (s[i] == '-')
                        return new ArithmeticEx(
                            Parse(s.Substring(0, i)),
                            Operation.Subtrubtion,
                            Parse(s.Substring(i + 1, s.Length - (i + 1))));
                }
            }

            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == '(')
                    bracketCount++;
                if (s[i] == ')')
                    bracketCount--;
                if (bracketCount != 0)
                    InsideBrackets = s[i] + InsideBrackets;
                else
                {
                    if (s[i] == '*')
                        return new ArithmeticEx(
                            Parse(s.Substring(0, i)),
                            Operation.Multiplication,
                            Parse(s.Substring(i + 1, s.Length - (i + 1))));

                    if (s[i] == '/')
                        return new ArithmeticEx(
                            Parse(s.Substring(0, i)),
                            Operation.Division,
                            Parse(s.Substring(i + 1, s.Length - (i + 1))));
                }
            }

            for (int i = s.Length - 1; i >= 0; i--)
            {
                if (s[i] == '(')
                    bracketCount++;
                if (s[i] == ')')
                    bracketCount--;
                if (bracketCount != 0)
                    InsideBrackets = s[i] + InsideBrackets;
                else
                {
                    if (s[i] == '^')
                        return new ArithmeticEx(
                            Parse(s.Substring(0, i)),
                            Operation.Power,
                            Parse(s.Substring(i + 1, s.Length - (i + 1))));
                }
            }
            
            throw new Exception("");
        }
        
    }
}
