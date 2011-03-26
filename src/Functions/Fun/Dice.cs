using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using CwIRC;

namespace MoronBot.Functions.Fun
{
    class Dice : Function
    {
        public Dice(MoronBot moronBot)
        {
            Name = GetName();
            Help = "roll <NdN[+N]...[v]>\t- Rolls the specified dice. Eg: 1d8+2d6+5v";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override void GetResponse(BotMessage message, MoronBot moronBot)
        {
            if (Regex.IsMatch(message.Command, "^(roll)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count > 0)
                {
                    string diceString = message.Parameters.Replace(" ", "");
                    Match diceMatch = Regex.Match(diceString, "(\\+|\\-)?[0-9]+d[0-9]+");
                    if (diceMatch.Success)
                    {
                        Match modMatch = Regex.Match(diceString, "(\\+|\\-)[0-9]+(?!(d|[0-9]))");
                        int modTotal = 0;
                        while (modMatch.Success)
                        {
                            string modString = modMatch.Value;
                            if (modString.Length < 10)
                            {
                                modTotal += Convert.ToInt32(modString);
                            }
                            else
                            {
                                moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "A modifier is too large, cannot roll.", message.ReplyTo));
                                return;
                            }

                            modMatch = modMatch.NextMatch();
                        }

                        List<int> rolls = new List<int>();
                        Random rand = new Random();

                        while (diceMatch.Success)
                        {
                            string[] dice = diceMatch.Value.Split('d');
                            Match signMatch = Regex.Match(dice[0], "(\\+|\\-)");
                            int sign = 1;
                            if (signMatch.Success)
                            {
                                if (signMatch.Value == "-")
                                {
                                    sign = -1;
                                }
                                dice[0] = dice[0].Substring(1, dice[0].Length - 1);
                            }

                            int numDice = 0;
                            if (dice[0].Length < 3)
                            {
                                numDice = Convert.ToInt32(dice[0]);
                                if (numDice > 50)
                                {
                                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "You can't roll more than 50 dice in one go.", message.ReplyTo));
                                    return;
                                }
                            }
                            else
                            {
                                moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "You can't roll more than 50 dice in one go.", message.ReplyTo));
                                return;
                            }
                            int diceSides = 0;
                            if (dice[1].Length < 4)
                            {
                                diceSides = Convert.ToInt32(dice[1]);
                                if (diceSides > 100)
                                {
                                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "No dice can have more than 100 sides.", message.ReplyTo));
                                    return;
                                }
                            }
                            else
                            {
                                moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "No dice can have more than 100 sides.", message.ReplyTo));
                                return;
                            }

                            for (int i = 0; i < numDice; i++)
                            {
                                rolls.Add(sign * rand.Next(1, diceSides + 1));
                            }

                            diceMatch = diceMatch.NextMatch();
                        }

                        string rollString = " [";
                        int rollTotal = 0;
                        for (int i = 0; i < rolls.Count; i++)
                        {
                            rollTotal += rolls[i];
                            rollString += rolls[i].ToString();
                            if (i < rolls.Count - 1)
                            {
                                rollString += ", ";
                            }
                            else
                            {
                                rollString += "]";
                            }
                        }

                        if (modTotal != 0)
                        {
                            rollString += " ";
                            rollTotal += modTotal;
                            if (modTotal > 0)
                            {
                                rollString += "+";
                            }
                            rollString += modTotal.ToString();
                        }

                        string rollMessage = message.User.Name + " rolled: " + rollTotal.ToString();
                        if (diceString.EndsWith("v"))
                        {
                            rollMessage += rollString;
                        }
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, rollMessage, message.ReplyTo));
                        return;
                    }
                    else
                    {
                        moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "No valid roll detected.", message.ReplyTo));
                        return;
                    }
                }
                else
                {
                    moronBot.MessageQueue.Add(new IRCResponse(ResponseType.Say, "Roll what?", message.ReplyTo));
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
