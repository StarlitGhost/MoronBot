using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities.Calc;

namespace Fun
{
    public class Roll : Function
    {
        List<Operator> Operators { get; set; }
        static Random rand = new Random();
        static List<string> verboseRolls = new List<string>();

        public Roll()
        {
            Help = "roll <NdN[+N]...[v]> - Rolls the specified dice. Eg: 1d8+2d6+5v";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            Operators = PopulateOperators();
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(roll)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count == 0)
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Roll what?", message.ReplyTo) };

                AllOperators.Operators = Operators;

                verboseRolls.Clear();

                string stringExpr = message.Parameters.Replace(" ", "");
                bool verbose = stringExpr.EndsWith("v");
                if (verbose)
                    stringExpr = stringExpr.TrimEnd('v');

                Expression expr = Tokenizer.Split(stringExpr);

                List<Token> postfixTokens = new List<Token>();
                bool isError = false;
                int errorTokenIndex = -1;
                string errorMessage = string.Empty;

                Evaluator.Validate(expr, ref postfixTokens, out isError, out errorTokenIndex, out errorMessage);

                string output = message.User.Name + " rolled: ";

                if (CalcUtilities.IsArithmeticExpression(expr))
                {
                    if (!isError)
                    {
                        double result = Evaluator.EvaluateBasic(postfixTokens);
                        if (verbose)
                        {
                            string verboseOutput = "[" + string.Join(" | ", verboseRolls) + "] ";
                            if (verboseOutput.Length < 400)
                                output += verboseOutput;
                            else
                                output += "[LOTS O' DICE] ";
                        }
                        output += result.ToString();
                    }
                    else
                    {
                        output = "Error at Token " + (errorTokenIndex + 1) + " ['" + expr.Tokens[errorTokenIndex].LinearToken + "']. " + errorMessage;
                    }
                }
                else
                {
                    output = "Not a recognized dice expression.";
                }

                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, output, message.ReplyTo) };
            }
            
            return null;
        }

        static List<Operator> PopulateOperators()
        {
            List<Operator> opList = new List<Operator>();

            opList.Add(new Operator()
            {
                SymbolText = "+",
                OperandCount = 2,
                PrecedenceLevel = 9,
                Associativity = OperatorAssociativity.LeftToRight,
                Execute = (x, y) => x + y
            });

            opList.Add(new Operator()
            {
                SymbolText = "-",
                OperandCount = 2,
                PrecedenceLevel = 9,
                Associativity = OperatorAssociativity.LeftToRight,
                Execute = (x, y) => x - y
            });

            opList.Add(new Operator()
            {
                SymbolText = "*",
                OperandCount = 2,
                PrecedenceLevel = 8,
                Associativity = OperatorAssociativity.LeftToRight,
                Execute = (x, y) => x * y
            });

            opList.Add(new Operator()
            {
                SymbolText = "/",
                OperandCount = 2,
                PrecedenceLevel = 8,
                Associativity = OperatorAssociativity.LeftToRight,
                Execute = (x, y) => x / y
            });

            opList.Add(new Operator()
            {
                SymbolText = "d",
                OperandCount = 2,
                PrecedenceLevel = 4,
                Associativity = OperatorAssociativity.LeftToRight,
                Execute = (x, y) =>
                {
                    int total = 0;
                    string rollString = "";
                    for (int dice = 0; dice < x; dice++)
                    {
                        int diceRoll = rand.Next(1, (int)y + 1);
                        rollString += (dice == 0 ? x.ToString() + "d" + y.ToString() + ": " : ", ") + diceRoll.ToString();
                        total += diceRoll;
                    }
                    verboseRolls.Add(rollString + " (" + total.ToString() + ")");
                    return total;
                }
            });

            opList.Add(new Operator()
            {
                SymbolText = "$",
                OperandCount = 1,
                PrecedenceLevel = 3,
                Associativity = OperatorAssociativity.RightToLeft,
                Execute = (x, y) =>
                {
                    int diceRoll = rand.Next(1, (int)x + 1);
                    verboseRolls.Add("d" + x.ToString() + ": " + diceRoll);
                    return diceRoll;
                }
            });

            opList.Add(new Operator()
            {
                SymbolText = "_",
                OperandCount = 1,
                PrecedenceLevel = 2,
                Associativity = OperatorAssociativity.RightToLeft,
                Execute = (x, y) => -x
            });

            opList.Add(new Operator()
            {
                SymbolText = "(",
                OperandCount = 0,
                PrecedenceLevel = 1,
                Associativity = OperatorAssociativity.LeftToRight
            });

            opList.Add(new Operator()
            {
                SymbolText = ")",
                OperandCount = 0,
                PrecedenceLevel = 1,
                Associativity = OperatorAssociativity.LeftToRight
            });

            opList.Add(new Operator()
            {
                SymbolText = "X",
                OperandCount = 0,
                PrecedenceLevel = 0,
                Associativity = OperatorAssociativity.LeftToRight
            });

            return opList;
        }
    }
}
