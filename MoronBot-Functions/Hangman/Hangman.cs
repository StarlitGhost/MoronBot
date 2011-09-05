using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Community.CsharpSqlite.SQLiteClient;

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
            Help = "h(ang)m(an) <command> (<params>) - A game of hangman. Sub-commands are as follows: start, stop, max, score (<user>), <guess>";
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
                            Start();
                            responses.Add(new IRCResponse(ResponseType.Say, "Hangman game started!", message.ReplyTo));
                            responses.Add(new IRCResponse(
                                ResponseType.Say,
                                ProgressString(),
                                message.ReplyTo));
                        }
                        else
                            responses.Add(new IRCResponse(ResponseType.Say, "There is already a game of Hangman in progress!", message.ReplyTo));
                        break;
                    case "stop":
                        if (ChannelList.UserIsAnyOp(message.User.Name, message.ReplyTo))
                        {
                            Stop();
                            responses.Add(new IRCResponse(ResponseType.Say, "Hangman game stopped!", message.ReplyTo));
                        }
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
                                responses.Add(new IRCResponse(ResponseType.Say, "You didn't give a number! |hm max <0-50>", message.ReplyTo));

                            responses.Add(new IRCResponse(ResponseType.Say, "Maximum bad guesses set to: " + maxBadGuesses, message.ReplyTo));
                        }
                        else
                            responses.Add(new IRCResponse(ResponseType.Say, "Cannot change the maximum bad guesses while a game is in progress!", message.ReplyTo));
                        break;
                    case "score":
                        if (message.ParameterList.Count > 1)
                        {
                            responses.Add(new IRCResponse(
                                ResponseType.Say,
                                GetScore(message.ParameterList[1]),
                                message.ReplyTo));
                        }
                        else
                        {
                            responses.Add(new IRCResponse(
                                ResponseType.Say,
                                GetScore(message.User.Name),
                                message.ReplyTo));
                        }
                        break;
                    default:
                        if (playing)
                        {
                            string guessMsg = Guess(message);
                            string visibleWord;

                            if (guessMsg != null)
                            {
                                responses.Add(new IRCResponse(
                                    ResponseType.Say,
                                    guessMsg,
                                    message.ReplyTo));
                                visibleWord = VisibleWord(word, guesses);
                            }
                            else
                                visibleWord = word;

                            if (word == visibleWord)
                            {
                                if (message.Parameters.ToLowerInvariant().Length == 1)
                                    IncrementFinalLetter(message.User.Name, 1);
                                else
                                    IncrementWord(message.User.Name, 1);

                                responses.Add(new IRCResponse(
                                    ResponseType.Say,
                                    "Congratulations " + message.User.Name + "! The " + WordOrPhrase(word) + " was: " + word,
                                    message.ReplyTo));
                                Stop();
                            }
                            else
                            {
                                responses.Add(new IRCResponse(
                                    ResponseType.Say,
                                    ProgressString(),
                                    message.ReplyTo));
                                if (badGuesses == maxBadGuesses)
                                {
                                    responses.Add(new IRCResponse(
                                        ResponseType.Say,
                                        "You have failed! The " + WordOrPhrase(word) + " was: " + word,
                                        message.ReplyTo));
                                    Stop();
                                }
                            }
                        }
                        else
                        {
                            responses.Add(new IRCResponse(
                                ResponseType.Say,
                                "There isn't a Hangman game running right now - use '|hm start' to start one",
                                message.ReplyTo));
                        }
                        break;
                }

                return responses;
            }
            return null;
        }

        string ProgressString()
        {
            return VisibleWord(word, guesses) + " " + WordLength() + " " + BadGuessIndicator() + " " + GuessList();
        }

        string WordLength()
        {
            return "(" + word.Length + ")";
        }

        string VisibleWord(string word, List<char> guesses)
        {
            string visibleWord = "";
            foreach (char c in word)
            {
                visibleWord += guesses.Contains(c) ? c : (c == ' ' ? ' ' : '_');
            }

            return visibleWord;
        }

        string BadGuessIndicator()
        {
            string indicator = "[";
            for (int i = 0; i < maxBadGuesses; ++i)
            {
                indicator += (i - badGuesses == 0 ? '*' : (i - badGuesses < 0 ? '.' : '-'));
            }
            return indicator + (badGuesses != maxBadGuesses ? 'O' : '#') + "]";
        }

        string GuessList()
        {
            string guessString = "[ ";
            foreach (char guess in guesses)
                guessString += guess + " ";

            return guessString + "]";
        }

        void Start()
        {
            word = words[rand.Next(words.Count)];
            guesses.Clear();
            playing = true;
        }

        void Stop()
        {
            word = "";
            guesses.Clear();
            playing = false;
            badGuesses = 0;
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
                IncrementCorrect(user, 1);
                return "'" + guess + "' is in the " + WordOrPhrase(word) + "!"; // correct guess
            }
            else
            {
                IncrementIncorrect(user, 1);
                ++badGuesses;
                return "'" + guess + "' is not in the " + WordOrPhrase(word); // incorrect guess
            }
        }

        string GuessWord(string guess, string user)
        {
            if (word == guess)
            {
                List<char> unknowns = new List<char>();
                int correct = 0;
                foreach (char c in word)
                {
                    if (!guesses.Contains(c) && !unknowns.Contains(c))
                    {
                        unknowns.Add(c);
                        ++correct;
                    }
                }

                IncrementCorrect(user, correct);
                return null;
            }
            else
            {
                IncrementIncorrect(user, 1);
                return "The " + WordOrPhrase(word) + " is not '" + guess + "'";
            }
        }

        string WordOrPhrase(string stringToTest)
        {
            return (stringToTest.Contains(" ") ? "phrase" : "word");
        }

        void IncrementCorrect(string user, int amount)
        {
            try
            {
                using (SqliteCommand cmd = (SqliteCommand)SQLiteInterface.Instance.Connection.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT OR IGNORE INTO hangman VALUES (@user, 0, 0, 0, 0);" +
                        "UPDATE hangman SET correct = correct + @amount WHERE user LIKE @user;";
                    cmd.Parameters.Add("@user", user.ToLowerInvariant());
                    cmd.Parameters.Add("@amount", amount);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
            }
        }

        void IncrementIncorrect(string user, int amount)
        {
            try
            {
                using (SqliteCommand cmd = (SqliteCommand)SQLiteInterface.Instance.Connection.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT OR IGNORE INTO hangman VALUES (@user, 0, 0, 0, 0);" +
                        "UPDATE hangman SET incorrect = incorrect + @amount WHERE user LIKE @user;";
                    cmd.Parameters.Add("@user", user.ToLowerInvariant());
                    cmd.Parameters.Add("@amount", amount);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
            }
        }

        void IncrementWord(string user, int amount)
        {
            try
            {
                using (SqliteCommand cmd = (SqliteCommand)SQLiteInterface.Instance.Connection.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT OR IGNORE INTO hangman VALUES (@user, 0, 0, 0, 0);" +
                        "UPDATE hangman SET word = word + @amount WHERE user LIKE @user;";
                    cmd.Parameters.Add("@user", user.ToLowerInvariant());
                    cmd.Parameters.Add("@amount", amount);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
            }
        }

        void IncrementFinalLetter(string user, int amount)
        {
            try
            {
                using (SqliteCommand cmd = (SqliteCommand)SQLiteInterface.Instance.Connection.CreateCommand())
                {
                    cmd.CommandText =
                        "INSERT OR IGNORE INTO hangman VALUES (@user, 0, 0, 0, 0);" +
                        "UPDATE hangman SET finalLetter = finalLetter + @amount WHERE user LIKE @user;";
                    cmd.Parameters.Add("@user", user.ToLowerInvariant());
                    cmd.Parameters.Add("@amount", amount);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
            }
        }

        string GetScore(string user)
        {
            try
            {
                using (SqliteCommand cmd = (SqliteCommand)SQLiteInterface.Instance.Connection.CreateCommand())
                {
                    cmd.CommandText =
                        "SELECT correct, incorrect, word, finalLetter FROM hangman WHERE user LIKE @user";
                    cmd.Parameters.Add("@user", user.ToLowerInvariant());
                    SqliteDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                        return "No scores held for " + user;

                    reader.Read();

                    int correct = reader.GetInt32(reader.GetOrdinal("correct"));
                    int incorrect = reader.GetInt32(reader.GetOrdinal("incorrect"));
                    int guessedWord = reader.GetInt32(reader.GetOrdinal("word"));
                    int finalLetter = reader.GetInt32(reader.GetOrdinal("finalLetter"));

                    string ratio = (incorrect > 0 ? ((float)correct / (float)incorrect).ToString() : "Infinity!");

                    return "Scores for " + user + " - Correct: " + correct + " Incorrect: " + incorrect + " Ratio: " + ratio + " Final Letter: " + finalLetter + " Whole Word: " + guessedWord;
                }
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
            }
            return "Couldn't open the database, sorry :(";
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
