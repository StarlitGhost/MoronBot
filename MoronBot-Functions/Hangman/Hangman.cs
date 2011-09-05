using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;
using MBUtilities.Channel;

namespace Fun
{
    public class Hangman : Function
    {
        Random rand = new Random();
        List<string> words = new List<string>();
        string word = "";
        List<char> guesses = new List<char>();
        bool playing = false;
        int badGuesses = 0;
        int maxBadGuesses = 5;

        public Hangman()
        {
            Help = "h(ang)m(an) <command> (<params>) - A game of hangman. Sub-commands are as follows: start, stop, max, <guess>";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;

            LoadWords();
        }

        ~Hangman()
        {
            SaveWords();
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (Regex.IsMatch(message.Command, "^(h(ang)?m(an)?)$", RegexOptions.IgnoreCase))
            {
                if (message.ParameterList.Count == 0)
                    return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "Guessing is done with '|hm <letter>', to start a game do '|hm start'", message.ReplyTo) };

                List<IRCResponse> responses = new List<IRCResponse>();
                switch (message.ParameterList[0].ToLowerInvariant())
                {
                    case "start":
                        if (!playing)
                        {
                            responses.Add(new IRCResponse(ResponseType.Say, Start(), message.ReplyTo));
                            responses.Add(new IRCResponse(ResponseType.Say, VisibleWord(word, guesses), message.ReplyTo));
                        }
                        else
                            responses.Add(new IRCResponse(ResponseType.Say, "There is already a game of Hangman in progress!", message.ReplyTo));
                        break;
                    case "stop":
                        if (ChannelList.UserIsAnyOp(message.User.Name, message.ReplyTo))
                            responses.Add(new IRCResponse(ResponseType.Say, Stop(), message.ReplyTo));
                        else
                            responses.Add(new IRCResponse(ResponseType.Say, "You need to be an operator to stop a game of Hangman!", message.ReplyTo));
                        break;
                    case "max":
                        if (!playing)
                        {
                            int temp;
                            if (message.ParameterList.Count > 1 && Int32.TryParse(message.ParameterList[1], out temp))
                                maxBadGuesses = (temp < 50 ? (temp >= 0 ? temp : 0) : 50);
                            else
                                responses.Add(new IRCResponse(ResponseType.Say, "You didn't give a number!", message.ReplyTo));

                            responses.Add(new IRCResponse(ResponseType.Say, "Maximum bad guesses set to: " + maxBadGuesses, message.ReplyTo));
                        }
                        else
                            responses.Add(new IRCResponse(ResponseType.Say, "Cannot change the maximum bad guesses while a game is in progress!", message.ReplyTo));
                        break;
                    default:
                        if (playing)
                        {
                            responses.Add(new IRCResponse(ResponseType.Say, Guess(message), message.ReplyTo));
                            responses.Add(new IRCResponse(ResponseType.Say, VisibleWord(word, guesses), message.ReplyTo));
                            if (badGuesses == maxBadGuesses)
                            {
                                responses.Add(new IRCResponse(ResponseType.Say, "You have failed! The " + WordOrPhrase(word) + " was: " + word, message.ReplyTo));
                                Stop();
                            }
                        }
                        else
                        {
                            responses.Add(new IRCResponse(ResponseType.Say, "There isn't a Hangman game running right now - use '|hm start' to start one", message.ReplyTo));
                        }
                        break;
                }

                return responses;
            }
            return null;
        }

        string VisibleWord(string word, List<char> guesses)
        {
            string visibleWord = "";
            foreach (char c in word)
            {
                visibleWord += guesses.Contains(c) ? c : (c == ' ' ? ' ' : '_');
            }

            if (word == visibleWord)
            {
                Stop();
                return "Congratulations! The " + WordOrPhrase(visibleWord) + " was: " + visibleWord;
            }

            string guessString = "";
            foreach (char guess in guesses)
                guessString += guess + " ";

            return visibleWord + BadGuessIndicator() + "[ " + guessString + "]";
        }

        string BadGuessIndicator()
        {
            string indicator = " [";
            for (int i = 0; i < maxBadGuesses; ++i)
            {
                indicator += (i - badGuesses == 0 ? '*' : (i - badGuesses < 0 ? '.' : '-'));
            }
            return indicator + (badGuesses != maxBadGuesses ? 'O' : '#') + "] ";
        }

        string Start()
        {
            word = words[rand.Next(words.Count)];
            guesses.Clear();
            playing = true;

            return "Hangman game started!";
        }

        string Stop()
        {
            word = "";
            guesses.Clear();
            playing = false;
            badGuesses = 0;

            return "Hangman game stopped!";
        }

        string Guess(BotMessage message)
        {
            string guess = message.Parameters.ToLowerInvariant();

            if (guess.Length == 1)
                return GuessSingle(guess[0], message.User.Name);
            else
                return GuessWord(guess, message.User.Name);
        }

        string GuessSingle(char guess, string user)
        {
            if (guesses.Contains(guess))
                return "'" + guess + "' has already been guessed";

            guesses.Add(guess);

            if (word.Contains(guess.ToString()))
            {
                SQLiteInterface.Instance.ExecuteNonQuery(
                    "INSERT OR IGNORE INTO hangman VALUES (" + user.ToLowerInvariant() + ", 0, 0, 0);"+
                    "UPDATE hangman SET correct = correct + 1 WHERE user LIKE " + user.ToLowerInvariant() + ";");
                return "'" + guess + "' is in the " + WordOrPhrase(word) + "!"; // correct guess
            }
            else
            {
                SQLiteInterface.Instance.ExecuteNonQuery(
                    "INSERT OR IGNORE INTO hangman VALUES (" + user.ToLowerInvariant() + ", 0, 0, 0);" +
                    "UPDATE hangman SET incorrect = incorrect + 1 WHERE user LIKE " + user.ToLowerInvariant() + ";");
                ++badGuesses;
                return "'" + guess + "' is not in the " + WordOrPhrase(word); // incorrect guess
            }
        }

        string GuessWord(string guess, string user)
        {
            return "Guessing the whole " + WordOrPhrase(word) + " isn't done yet.";
        }

        string WordOrPhrase(string stringToTest)
        {
            return (stringToTest.Contains(" ") ? "phrase" : "word");
        }

        void LoadWords()
        {
            string path = Path.Combine(Settings.Instance.DataPath, "hangman.txt");

            if (File.Exists(path))
                words.AddRange(File.ReadAllLines(path));
            else
                words.Add("add some words or phrases you fool");
        }

        void SaveWords()
        {
            File.WriteAllLines(Path.Combine(Settings.Instance.DataPath, "hangman.txt"), words.ToArray());
        }
    }
}
