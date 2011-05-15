using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using MoronBot.Utilities;
using MoronBot.Utilities.Calc;
using CwIRC;

namespace MoronBot.Functions.Utility
{
    class Calc : Function
    {
        public Calc(MoronBot moronBot)
        {
            Name = GetName();
            Help = "calc <expr>\t\t- Calculates the result of the given expression. Supported operations, in decreasing order of precedence, are: ( ), ^, % / *, - +";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, @"^(calc(ulate)?)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0) // Expression given
                {
                    Expression expr = Tokenizer.Split(message.Parameters);

                    List<Token> postfixTokens = new List<Token>();
                    bool isError = false;
                    int errorTokenIndex = -1;
                    string errorMessage = string.Empty;

                    Evaluator.Validate(expr, ref postfixTokens, out isError, out errorTokenIndex, out errorMessage);

                    string output = "";
                    if (!isError)
                    {
                        double result = Evaluator.EvaluateBasic(postfixTokens);
                        output = "Result: " + result.ToString();
                    }
                    else
                    {
                        output = "Error at Token " + (errorTokenIndex + 1) + " ['" + expr.Tokens[errorTokenIndex].LinearToken + "']. " + errorMessage;
                    }

                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, output, message.ReplyTo));
                    return;
                }
                else // No expr given
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "You didn't give an expression to calculate!", message.ReplyTo));
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}
