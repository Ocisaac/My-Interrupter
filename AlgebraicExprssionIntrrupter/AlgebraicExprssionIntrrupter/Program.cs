using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AlgebraicExprssionIntrrupter
{
    class Program
    {
        /*problem with paren of sort (x + 1)/(x + 2)*/
        static void Main(string[] args)
        {
            var treeEx = AlgebExpression.Parse("1=2x - 3x^2 + 1 - 4x");
            Console.WriteLine(treeEx);  
            Console.WriteLine(treeEx.SimplifyEquality().SubtructionToNegAddition());
            Console.WriteLine(treeEx.SimplifyEquality().SubtructionToNegAddition().LeftAlignAddition());
            Console.WriteLine(treeEx.SimplifyDivision().SimplifyEquality());
        }
    }

    enum Operation
    {
        Equality,
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Value
    }

    struct AlgebVal
    {
        public float Coaf;
        public int Pow;

        public static AlgebVal Zero = new AlgebVal(0, 0);

        public AlgebVal(float coaf, int pow)
        {
            Coaf = coaf;
            Pow = coaf == 0 ? 0 : pow;
        }

        /// <summary>
        /// of the shape (float)x^(int)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static AlgebVal Parse(string s)
        {
            var sa = s.Split('x', '^');
            float f; int i;
            var b = Single.TryParse(sa[0], out f);
            b &= Int32.TryParse(sa[2], out i);
            if (b)
                return new AlgebVal(f, i);
            else
                throw new FormatException("must be of shape (float)x^(int)");
        }

        public static bool TryParse(string s, out AlgebVal av)
        {
            Regex avreg = new Regex(@"^\-?[0-9\.]*x?\^?[0-9]*$");
            if (avreg.IsMatch(s))
            {
                if (!s.Contains('x'))
                    s = s + "x^0";
                if (s.EndsWith("x"))
                    s = s + "^1";
                if (s.StartsWith("x"))
                    s = "1" + s;
                av = Parse(s);
                return true;
            }
            av = new AlgebVal(0,0);
            return false;
        }

        public static AlgebVal operator +(AlgebVal left, AlgebVal right)
        {
            if (left.Pow != right.Pow)
                throw new InvalidOperationException("must have same power");
            return new AlgebVal(left.Coaf + right.Coaf, left.Pow);
        }

        public static AlgebVal operator *(AlgebVal left, AlgebVal right)
        {
            return new AlgebVal(left.Coaf * right.Coaf, left.Pow + right.Pow);
        }

        public static AlgebVal operator /(AlgebVal left, AlgebVal right)
        {
            if (left.Pow >= right.Pow)
                return new AlgebVal(left.Coaf / right.Coaf, left.Pow - right.Pow);
            else throw new ArgumentException("can't m8");
        }

        public static AlgebVal operator -(AlgebVal left, AlgebVal right)
        {
            if (left.Pow != right.Pow && left.Coaf != 0 && right.Coaf != 0)
                throw new InvalidOperationException("must have same power");
            return new AlgebVal(left.Coaf - right.Coaf, left.Pow);
        }

        public static bool operator ==(AlgebVal left, AlgebVal right)
        {
            return 
                (left.Coaf == 0 && right.Coaf == 0) 
                    ? true 
                    : left.Coaf == right.Coaf && left.Pow == right.Pow;
        }

        public static bool operator !=(AlgebVal left, AlgebVal right)
        {
            return !(left == right);
        }

        public static implicit operator AlgebVal(float i) =>
            new AlgebVal(i, 0);
        

        public override string ToString() =>
            this == Zero ? "0" : (Coaf == 0 ? "" : Coaf.ToString()) + ((Pow > 1) ? "x^" + Pow.ToString() : Pow == 1 ? "x" : "");
    }

    class AlgebExpression
    {
        public AlgebExpression left;
        public Operation op;
        public AlgebExpression right;
        public AlgebVal? value;

        public AlgebExpression SimplyfyZeroAddition()
        {
            if (this.op == Operation.Value)
                return this;
            var temp = this.LeftAlignAddition();

            if (temp.right.value.Value.Coaf == 0)
                return temp.left.SimplyfyZeroAddition();
            else
                return temp.left.SimplyfyZeroAddition() + temp.right;
        }

        public AlgebExpression LeftAlignAddition()
        {
            if (this.op == Operation.Equality)
            {
                var leftleft = left.LeftAlignAddition();
                return new AlgebExpression(this.left.LeftAlignAddition(), Operation.Equality, right.LeftAlignAddition());
            }
            if (this.op == Operation.Value)
                return this;
            if (this.op != Operation.Addition)
                throw new InvalidOperationException("to align addition all ops must be addition");
            if (this.right.op == Operation.Value)
                return left.LeftAlignAddition() + right;
            if (this.left.op == Operation.Value)
                return right.LeftAlignAddition() + left;
            if (left.op == Operation.Addition && right.op == Operation.Addition)
            {
                left = left.LeftAlignAddition();
                right = right.LeftAlignAddition();
                return new AlgebExpression
                    (((left.left + right.right)
                        + left.right)
                        + right.left)
                        .LeftAlignAddition();
            }

            throw new Exception("sumthing went wrong");
        }

        public AlgebExpression SubtructionToNegAddition()
        {
            if (this.op == Operation.Value)
                return value;
            this.left = left.SubtructionToNegAddition();
            this.right = right.SubtructionToNegAddition();
            if (this.op == Operation.Subtraction)
                return new AlgebExpression(left, Operation.Addition, right * -1);
            return this;
        }

        public AlgebExpression SimplifyEquality()
        {
            if (op == Operation.Equality)
                if (right != AlgebVal.Zero)                
                    return this - right;
            return this;
        }

        public AlgebExpression SimplifyDivision()
        {
            var myThis = this;
            return SimplifyDivsion(this, ref myThis, true);
        }
        private static AlgebExpression SimplifyDivsion(AlgebExpression currTree, ref AlgebExpression bigTree, bool rtnBig)
        {
            if (currTree.op == Operation.Value)
                return currTree;
            
            currTree.left = SimplifyDivsion(currTree.left, ref bigTree, false);
            currTree.right = SimplifyDivsion(currTree.right, ref bigTree, false);
            
            if (currTree.op == Operation.Division)
            {
                bigTree = bigTree * currTree.right;
            }
            return rtnBig ? bigTree : currTree;
        }

        public static implicit operator AlgebExpression(AlgebVal av) =>
            new AlgebExpression(av);

        public static implicit operator AlgebExpression(float i) =>
            new AlgebExpression(i);

        public AlgebExpression(AlgebExpression l, Operation o, AlgebExpression r)
        {
            left = l; op = o; right = r;
        }

        public AlgebExpression(AlgebVal val)
        {
            op = Operation.Value;
            value = val;
        }

        public AlgebExpression(AlgebExpression tree)
        {
            left = tree.left; op = tree.op; right = tree.right; value = tree.value;
        }

        public static AlgebExpression Parse(string s)
        {
            s = new string(s.Where(c => c != ' ').ToArray());
            Regex alexreg = new Regex(@"^[\(\)0-9\.\-\+/\*x^]+=?[\(\)0-9\.\-\+/\*x^]*$");
            if (!alexreg.IsMatch(s))
                throw new FormatException("bad format");

            AlgebVal v = AlgebVal.Zero;
            string InsideBrackets = "";
            int bracketCount = 0;

            if (s.Count(c => c == '(') != s.Count(c => c == ')'))
                throw new FormatException("parens must balance");
            if (s.StartsWith("(") && s.EndsWith(")"))             
            {
                int leftParen = s.Substring(1, s.Length - 2).IndexOf("(");
                int rightParen = s.Substring(1, s.Length - 2).IndexOf(")");
                if (leftParen <= rightParen)
                    return Parse(s.Substring(1, s.Length - 2));
            }                
            
            if (AlgebVal.TryParse(s, out v))
                return new AlgebExpression(v);

            if (s.StartsWith("-"))
                s = "0x^0" + s;
            #region =
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
                    if (s[i] == '=')
                        return new AlgebExpression(
                            Parse(s.Substring(0, i)),
                            Operation.Equality,
                            Parse(s.Substring(i + 1, s.Length - (i + 1))));
                }  
            }
            #endregion
            #region +-
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
                        return new AlgebExpression(
                            Parse(s.Substring(0, i))
                            +
                            Parse(s.Substring(i + 1, s.Length - (i + 1))));

                    if (s[i] == '-')
                        return new AlgebExpression(
                            Parse(s.Substring(0, i)) 
                            -
                            Parse(s.Substring(i + 1, s.Length - (i + 1))));
                }
            }
            #endregion
            #region */
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
                        return new AlgebExpression(
                            Parse(s.Substring(0, i))
                            *
                            Parse(s.Substring(i + 1, s.Length - (i + 1))));

                    if (s[i] == '/')
                        return new AlgebExpression(
                            Parse(s.Substring(0, i))
                            /
                            Parse(s.Substring(i + 1, s.Length - (i + 1))));
                }
            }
            #endregion

            throw new Exception();
        }

        public static bool operator ==(AlgebExpression left, AlgebExpression right)
        {
            if (left.op == Operation.Value && right.op == Operation.Value)
                return left.value == right.value;
            if (left.op != right.op)
                return false;

            else
                return
                    left.op == right.op
                    && left.left == right.left
                    && right.right == left.right;
        }

        public static bool operator !=(AlgebExpression left, AlgebExpression right) =>
            !(left == right);

        public static AlgebExpression operator +(AlgebExpression left, AlgebExpression right)
        {
            if (left.op == Operation.Value
                && right.op == Operation.Value
                && 
                    (left.value.Value.Pow == right.value.Value.Pow 
                    || left.value.Value.Coaf == 0 
                    || right.value.Value.Coaf == 0))
                return new AlgebExpression(left.value.Value + right.value.Value);

            if (left.op == Operation.Equality && right.op == Operation.Equality)
                throw new InvalidOperationException("can't be both =");

            if (left.op == Operation.Equality)
                return new AlgebExpression(left.left + right, Operation.Equality, left.right + right);
            if (right.op == Operation.Equality)
                return new AlgebExpression(right.left + left, Operation.Equality, right.right + left);
            
            return new AlgebExpression(left, Operation.Addition, right);
        }

        public static AlgebExpression operator -(AlgebExpression left, AlgebExpression right)
        {
            if (left.op == Operation.Value
                && right.op == Operation.Value
                && 
                    (left.value.Value.Pow == right.value.Value.Pow 
                    || left.value.Value.Coaf == 0 
                    || right.value.Value.Coaf == 0))
                return new AlgebExpression(left.value.Value - right.value.Value);

            if (left.op == Operation.Equality && right.op == Operation.Equality)
                throw new InvalidOperationException("can't be both =");

            if (left.op == Operation.Equality)
                return new AlgebExpression(left.left - right, Operation.Equality, left.right - right);
            if (right.op == Operation.Equality)
                return new AlgebExpression(right.left - left, Operation.Equality, right.right - left);

            if (right == left)
                return new AlgebExpression(AlgebVal.Zero);

            return new AlgebExpression(left, Operation.Subtraction, right);
        }

        public static AlgebExpression operator *(AlgebExpression left, AlgebExpression right)
        {
            if (left.op == Operation.Value
                && right.op == Operation.Value)
                return new AlgebExpression(left.value.Value * right.value.Value);

           
            if (left.op == Operation.Equality && right.op == Operation.Equality)
                throw new InvalidOperationException("can't be both =");

            if (left.op == Operation.Equality)
                return new AlgebExpression(left.left * right, Operation.Equality, left.right * right);
            if (right.op == Operation.Equality)
                return new AlgebExpression(right.left * left, Operation.Equality, right.right * left);

            if (left.op == Operation.Division && left.right == right)
                return left.left;

            if (right.op == Operation.Division && right.right == left)
                return right;


            if (left.op == Operation.Addition || left.op == Operation.Subtraction)
                return new AlgebExpression(left.left * right, left.op, left.right * right);

            if (right.op == Operation.Addition || right.op == Operation.Subtraction)
                return new AlgebExpression(right.left * left, right.op, right.right * left);
            if (left.op == Operation.Multiplication)
                return new AlgebExpression(left.left * right, Operation.Multiplication, left.right * right);
            if (right.op == Operation.Multiplication)
                return new AlgebExpression(right.left * left, Operation.Multiplication, right.right * left);

            if (left.op == Operation.Value)
                return new AlgebExpression(left.value.Value * right.left);

            if (right.op == Operation.Value)
                return new AlgebExpression(right.value.Value * left.left);

            return new AlgebExpression(left, Operation.Multiplication, right);
        }

        public static AlgebExpression operator /(AlgebExpression left, AlgebExpression right)
        {
            if (left.op == Operation.Value
                && right.op == Operation.Value 
                && left.value.Value.Pow >= right.value.Value.Pow)
                return new AlgebExpression(left.value.Value / right.value.Value);

            if (left.op == Operation.Equality && right.op == Operation.Equality)
                throw new InvalidOperationException("can't be both =");

            if (left.op == Operation.Equality)
                return new AlgebExpression(left.left / right, Operation.Equality, left.right / right);
            
            if (right.op == Operation.Equality)
                return new AlgebExpression(right.left / left, Operation.Equality, right.right / left);

        //   if (left.op == Operation.Addition || left.op == Operation.Subtraction)
        //       return new AlgebExpression(left.left / right, left.op, left.right / right);
           
            if (left == right)
                return new AlgebVal(1, 0);

            return new AlgebExpression(left, Operation.Division, right);
        }

        public override string ToString()
        {
            switch (op)
            {
                case Operation.Value:
                    return value.Value.ToString();
                case Operation.Equality:
                    return left.ToString() + "=" + right.ToString();
                case Operation.Addition:
                    return "(" + left.ToString() + "+" + right.ToString() + ")";
                case Operation.Subtraction:
                    return "(" + left.ToString() + "-" + right.ToString() + ")";
                case Operation.Multiplication:
                    return "(" + left.ToString() + "*" + right.ToString() + ")";
                case Operation.Division:
                    return "(" + left.ToString() + "/" + right.ToString() + ")";
            }
            return "";
        }
    }
}
