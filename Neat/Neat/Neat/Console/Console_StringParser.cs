using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using Neat.Mathematics;
#if LIVE
using Microsoft.Xna.Framework.GamerServices;
#endif
namespace Neat.Components
{
    public partial class Console : GameComponent
    {
        public string ParseParenthesis(string command)
        {
            command = command.Trim();
            if (command.Length < 1) return command;

            if (command.Contains(eval_open)) // there's a dvar needs to be evaluated
            {
                //bruteforce
                int start = command.IndexOf(eval_open);
                int end = start;
                for (int i = start; i < command.Length && i >= 0; i++)
                {
                    if (command[i] == eval_open)
                    {
                        start = i;
                    }
                    else if (command[i] == eval_close)
                    {
                        end = i;
                        string left = command.Substring(0, start);
                        string right = command.Substring(end + 1);
                        string mid = EvaluateString( command.Substring(start + 1, end - start - 1) );

                        command = left + Ram.GetValue(mid) + right;
                        start = command.IndexOf(eval_open);
                        i = start;
                    }
                }
            }
            return command;
        }

        List<Operator> operators, sortedOperatorsByLength, sortedOperatorsByOrder;
        Operator minusOperator;
        void InitExpressionEvaluation()
        {
            operators = new List<Operator>();
            operators.Add(new Operator("^", EvaluatePow,5));
            operators.Add(new Operator("%", EvaluateMod,4));
            operators.Add(new Operator("/", EvaluateDiv,4));
            operators.Add(new Operator("*", EvaluateMul,4));
            operators.Add(new Operator("+", EvaluateAdd,3));
            operators.Add(minusOperator = new Operator("-", EvaluateSub,3));
            operators.Add(new Operator("&", EvaluateAnd,2));
            operators.Add(new Operator("|", EvaluateOr,2));
            operators.Add(new Operator("#", EvaluateXor,2));
            operators.Add(new Operator("!", EvaluateNot,2,true));
            operators.Add(new Operator("==", EvaluateCmpEqual,1));
            operators.Add(new Operator("!=", EvaluateCmpNotEqual,1));
            operators.Add(new Operator(">", EvaluateCmpGreaterThan,1));
            operators.Add(new Operator(">=", EvaluateCmpGreaterEqualThan,1));
            operators.Add(new Operator("<", EvaluateCmpLessThan,1));
            operators.Add(new Operator("<=", EvaluateCmpLessEqualThan,1));

            sortedOperatorsByLength = operators.OrderBy(str => str.Sign.Length).ToList();
            sortedOperatorsByOrder = operators.OrderBy(str => str.Order).ToList();
            for (int i = sortedOperatorsByLength.Count - 1; i >= 0; i--)
            {
                sortedOperatorsByLength[i].Tag = ((char)(i + 1)).ToString();
            }
        }

        #region Operators
        delegate string OperatorFunction(string lvalue, string rvalue);
        class Operator
        {
            public Operator(string sign, OperatorFunction func, int ord, bool unary)
            {
                Sign = sign;
                Function = func;
                IsUnary = unary;
                Order = ord;
            }
            public Operator(string sign, OperatorFunction func, int ord)
            {
                Sign = sign;
                Function = func;
                Order = ord;
            }
            public string Sign;
            public OperatorFunction Function;
            public bool IsUnary = false;
            public string Tag = "";
            public int Order = 0;
            public string Evaluate(string lvalue, string rvalue)
            {
                return Function(lvalue, rvalue);
            }
        }

        string EvaluateAdd(string lvalue, string rvalue)
        {
            return (float.Parse(lvalue) + float.Parse(rvalue)).ToString();
        }

        string EvaluateSub(string lvalue, string rvalue)
        {
            if (lvalue == null || lvalue.Trim().Length == 0)
                return (-float.Parse(rvalue)).ToString();
            else
                return (float.Parse(lvalue) - float.Parse(rvalue)).ToString();
        }

        string EvaluateMul(string lvalue, string rvalue)
        {
            return (float.Parse(lvalue) * float.Parse(rvalue)).ToString();
        }

        string EvaluateDiv(string lvalue, string rvalue)
        {
            return (float.Parse(lvalue) / float.Parse(rvalue)).ToString();
        }

        string EvaluatePow(string lvalue, string rvalue)
        {
            return (Math.Pow(double.Parse(lvalue), double.Parse(rvalue))).ToString();
        }

        string EvaluateMod(string lvalue, string rvalue)
        {
            return (float.Parse(lvalue) % float.Parse(rvalue)).ToString();
        }
        string EvaluateNot(string lvalue, string rvalue)
        {
            return (!bool.Parse(rvalue)).ToString();
        }

        string EvaluateAnd(string lvalue, string rvalue)
        {
            return (bool.Parse(lvalue) & bool.Parse(rvalue)).ToString();
        }

        string EvaluateOr(string lvalue, string rvalue)
        {
            return (bool.Parse(lvalue) | bool.Parse(rvalue)).ToString();
        }

        string EvaluateXor(string lvalue, string rvalue)
        {
            return (bool.Parse(lvalue) ^ bool.Parse(rvalue)).ToString();
        }

        string EvaluateCmpEqual(string lvalue, string rvalue)
        {
            return (lvalue.Trim().ToLower() == rvalue.Trim().ToLower()).ToString();
        }

        string EvaluateCmpNotEqual(string lvalue, string rvalue)
        {
            return (lvalue.Trim().ToLower() != rvalue.Trim().ToLower()).ToString();
        }

        string EvaluateCmpGreaterThan(string lvalue, string rvalue)
        {
            return (float.Parse(lvalue) > float.Parse(rvalue)).ToString();
        }

        string EvaluateCmpGreaterEqualThan(string lvalue, string rvalue)
        {
            return (float.Parse(lvalue) >= float.Parse(rvalue)).ToString();
        }

        string EvaluateCmpLessThan(string lvalue, string rvalue)
        {
            return (float.Parse(lvalue) < float.Parse(rvalue)).ToString();
        }

        string EvaluateCmpLessEqualThan(string lvalue, string rvalue)
        {
            return (float.Parse(lvalue) <= float.Parse(rvalue)).ToString();
        }

        #endregion 
        
        string EvaluateString(string expr)
        {
            if (expr.Length == 0) return expr;

            if (expr.Contains(eval_open)) expr = ParseParenthesis(expr);

            for (int i = sortedOperatorsByLength.Count - 1; i >= 0; i--)
            {
                expr = expr.Replace(sortedOperatorsByLength[i].Sign, ((char)(i + 1)).ToString());
            }

            int idx = -1;

            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i] == minusOperator.Tag[0])
                {
                    if (i == 0 || expr[i-1] <= (char)operators.Count)
                    {
                        expr = expr.Remove(i, 1);
                        expr = expr.Insert(i, minusOperator.Sign);
                    }
                }
            }

            int currentOrder = 0;
            Operator selectedOperator = operators[0];
            int minIdx = -1;
            for (int i = 0; i < sortedOperatorsByOrder.Count; i++)
            {
                idx = expr.LastIndexOf(sortedOperatorsByOrder[i].Tag);
                if (idx >= 0 && idx > minIdx)
                {
                    minIdx = idx;
                    selectedOperator = sortedOperatorsByOrder[i];
                }
                if (sortedOperatorsByOrder[i].Order != currentOrder)
                {
                    currentOrder = sortedOperatorsByOrder[i].Order;
                    if (minIdx >= 0 && minIdx < expr.Length)
                    {
                        var left = selectedOperator.IsUnary ? "" : EvaluateString(expr.Substring(0, minIdx));
                        var right = EvaluateString(expr.Substring(minIdx + 1, expr.Length - minIdx - 1));
                        expr = selectedOperator.Function(left, right);
                        currentOrder = 0;
                        i = 0;
                    }
                    minIdx = -1;
                }
            }
            return expr;
        }

        public void RunCommand()
        {
            RunCommand(command);
        }

        protected void RunCommand(string rawcommand)
        {
            if (Echo) WriteLine("  > " + rawcommand);
            try
            {
                var commands =
                    rawcommand.Replace("$_", "\n").
                    Replace("$S", " ").
                    Split('\n');

                foreach (var command in commands)
                {
                    if (!string.IsNullOrWhiteSpace(command))
                        Run(ParseParenthesis(command.Trim()).Split(' ').ToList());
                }
            }
            catch (Exception e)
            {
                WriteLine("Error while parsing command: " + e.Message);
            }
        }

        Dictionary<string, Color> knownColors = new Dictionary<string, Color>
        {
            {"aliceblue", Color.AliceBlue},
            {"antiquewhite", Color.AntiqueWhite},
            {"aqua", Color.Aqua},
            {"aquamarine", Color.Aquamarine},
            {"azure", Color.Azure},
            {"beige", Color.Beige},
            {"bisque", Color.Bisque},
            {"black", Color.Black},
            {"blanchedalmond", Color.BlanchedAlmond},
            {"blue", Color.Blue},
            {"blueviolet", Color.BlueViolet},
            {"brown", Color.Brown},
            {"burlywood", Color.BurlyWood},
            {"cadetblue", Color.CadetBlue},
            {"chartreuse", Color.Chartreuse},
            {"chocolate", Color.Chocolate},
            {"coral", Color.Coral},
            {"cornflowerblue", Color.CornflowerBlue},
            {"cornsilk", Color.Cornsilk},
            {"crimson", Color.Crimson},
            {"cyan", Color.Cyan},
            {"darkblue", Color.DarkBlue},
            {"darkcyan", Color.DarkCyan},
            {"darkgoldenrod", Color.DarkGoldenrod},
            {"darkgray", Color.DarkGray},
            {"darkgreen", Color.DarkGreen},
            {"darkkhaki", Color.DarkKhaki},
            {"darkmagenta", Color.DarkMagenta},
            {"darkolivegreen", Color.DarkOliveGreen},
            {"darkorange", Color.DarkOrange},
            {"darkorchid", Color.DarkOrchid},
            {"darkred", Color.DarkRed},
            {"darksalmon", Color.DarkSalmon},
            {"darkseagreen", Color.DarkSeaGreen},
            {"darkslateblue", Color.DarkSlateBlue},
            {"darkslategray", Color.DarkSlateGray},
            {"darkturquoise", Color.DarkTurquoise},
            {"darkviolet", Color.DarkViolet},
            {"deeppink", Color.DeepPink},
            {"deepskyblue", Color.DeepSkyBlue},
            {"dimgray", Color.DimGray},
            {"dodgerblue", Color.DodgerBlue},
            {"firebrick", Color.Firebrick},
            {"floralwhite", Color.FloralWhite},
            {"forestgreen", Color.ForestGreen},
            {"fuchsia", Color.Fuchsia},
            {"gainsboro", Color.Gainsboro},
            {"ghostwhite", Color.GhostWhite},
            {"gold", Color.Gold},
            {"goldenrod", Color.Goldenrod},
            {"gray", Color.Gray},
            {"green", Color.Green},
            {"greenyellow", Color.GreenYellow},
            {"honeydew", Color.Honeydew},
            {"hotpink", Color.HotPink},
            {"indianred", Color.IndianRed},
            {"indigo", Color.Indigo},
            {"ivory", Color.Ivory},
            {"khaki", Color.Khaki},
            {"lavender", Color.Lavender},
            {"lavenderblush", Color.LavenderBlush},
            {"lawngreen", Color.LawnGreen},
            {"lemonchiffon", Color.LemonChiffon},
            {"lightblue", Color.LightBlue},
            {"lightcoral", Color.LightCoral},
            {"lightcyan", Color.LightCyan},
            {"lightgoldenrodyellow", Color.LightGoldenrodYellow},
            {"lightgray", Color.LightGray},
            {"lightgreen", Color.LightGreen},
            {"lightpink", Color.LightPink},
            {"lightsalmon", Color.LightSalmon},
            {"lightseagreen", Color.LightSeaGreen},
            {"lightskyblue", Color.LightSkyBlue},
            {"lightslategray", Color.LightSlateGray},
            {"lightsteelblue", Color.LightSteelBlue},
            {"lightyellow", Color.LightYellow},
            {"lime", Color.Lime},
            {"limegreen", Color.LimeGreen},
            {"linen", Color.Linen},
            {"magenta", Color.Magenta},
            {"maroon", Color.Maroon},
            {"mediumaquamarine", Color.MediumAquamarine},
            {"mediumblue", Color.MediumBlue},
            {"mediumorchid", Color.MediumOrchid},
            {"mediumpurple", Color.MediumPurple},
            {"mediumseagreen", Color.MediumSeaGreen},
            {"mediumslateblue", Color.MediumSlateBlue},
            {"mediumspringgreen", Color.MediumSpringGreen},
            {"mediumturquoise", Color.MediumTurquoise},
            {"mediumvioletred", Color.MediumVioletRed},
            {"midnightblue", Color.MidnightBlue},
            {"mintcream", Color.MintCream},
            {"mistyrose", Color.MistyRose},
            {"moccasin", Color.Moccasin},
            {"navajowhite", Color.NavajoWhite},
            {"navy", Color.Navy},
            {"oldlace", Color.OldLace},
            {"olive", Color.Olive},
            {"olivedrab", Color.OliveDrab},
            {"orange", Color.Orange},
            {"orangered", Color.OrangeRed},
            {"orchid", Color.Orchid},
            {"palegoldenrod", Color.PaleGoldenrod},
            {"palegreen", Color.PaleGreen},
            {"paleturquoise", Color.PaleTurquoise},
            {"palevioletred", Color.PaleVioletRed},
            {"papayawhip", Color.PapayaWhip},
            {"peachpuff", Color.PeachPuff},
            {"peru", Color.Peru},
            {"pink", Color.Pink},
            {"plum", Color.Plum},
            {"powderblue", Color.PowderBlue},
            {"purple", Color.Purple},
            {"red", Color.Red},
            {"rosybrown", Color.RosyBrown},
            {"royalblue", Color.RoyalBlue},
            {"saddlebrown", Color.SaddleBrown},
            {"salmon", Color.Salmon},
            {"sandybrown", Color.SandyBrown},
            {"seagreen", Color.SeaGreen},
            {"seashell", Color.SeaShell},
            {"sienna", Color.Sienna},
            {"silver", Color.Silver},
            {"skyblue", Color.SkyBlue},
            {"slateblue", Color.SlateBlue},
            {"slategray", Color.SlateGray},
            {"snow", Color.Snow},
            {"springgreen", Color.SpringGreen},
            {"steelblue", Color.SteelBlue},
            {"tan", Color.Tan},
            {"teal", Color.Teal},
            {"thistle", Color.Thistle},
            {"tomato", Color.Tomato},
            {"transparent", Color.Transparent},
            {"turquoise", Color.Turquoise},
            {"violet", Color.Violet},
            {"wheat", Color.Wheat},
            {"white", Color.White},
            {"whitesmoke", Color.WhiteSmoke},
            {"yellow", Color.Yellow},
            {"yellowgreen", Color.YellowGreen},
        };

        //ParseColor(string) -> color 
        //returns new Color() on error
        //input methods:              
        //r,g,b                       
        //r,g,b,a 0f-1f               
        public Color ParseColor(string p)
        {
            Color cl = new Color();

            List<string> args = new List<string>();
            args.Add("");

            string k = "";
            string cmd = p;
            for (int i = 0; i < cmd.Length; i++)
            {
                if (cmd[i] == ' ')
                {
                    cmd = (cmd.Substring(i)).Trim();
                    i = -1;
                    args.Add(k);
                    k = "";
                }
                else k += cmd[i];
            }
            args.Add(k);

            try
            {
                if (knownColors.ContainsKey(args[1].ToLower())) cl = knownColors[args[1].ToLower()];
                else if (args[1].Count(new Func<char, bool>(o => o == ',')) == 3) cl = new Color(GeometryHelper.String2Vector4(args[1]));
                else if (args[1].Count(new Func<char, bool>(o => o == ',')) == 2) cl = new Color(GeometryHelper.String2Vector3(args[1]));
                else if (args.Count > 2) // custom color - float float float [ float(alpha) ]
                {
                    try
                    {
                        if (args.Count == 4)
                            cl = new Color(
                                new Vector3(float.Parse(args[1]),  //R
                                    float.Parse(args[2]), //G
                                    float.Parse(args[3])));
                        else if (args.Count == 5)
                            cl = new Color(
                                new Vector4(float.Parse(args[1]),  //R
                                    float.Parse(args[2]), //G
                                    float.Parse(args[3]), //B
                                    float.Parse(args[4]))); //A
                        else
                            WriteLine("Error in parameters");
                    }
                    catch { WriteLine("Error: Cannot create color"); }
                }
                else { WriteLine("Bad Color"); }
            }
            catch { WriteLine("Error in " + Args2Str(args, 0)); }
            return cl;
        }
        public string Args2Str(IList<string> args, int startIndex)
        {
            string result = "";
            for (int i = startIndex; i < args.Count; i++)
                result += args[i] + " ";
            return result.Trim();
        }
    }
}
