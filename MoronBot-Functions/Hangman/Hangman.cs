using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        int index = 0;
        List<string> words = new List<string>();
        string word = "";
        List<char> guesses = new List<char>();
        bool playing = false;
        int badGuesses = 0;
        int maxBadGuesses = 5;

        class LastRound
        {
            public int correct = 0;
            public int incorrect = 0;
        }
        Dictionary<string, LastRound> lastRound = new Dictionary<string, LastRound>();

        Object dataLock = new Object();
        Object sqlLock = new Object();

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
            if (!Regex.IsMatch(message.Command, "^(h(ang)?m(an)?)$", RegexOptions.IgnoreCase))
                return null;

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
                            maxBadGuesses = (temp < 25 ? (temp >= 1 ? temp : 1) : 25);
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
                                "Congratulations " + message.User.Name + "! The " + WordOrPhrase(word) + " was: " + word + " " + LastRoundSummary(message.User.Name),
                                message.ReplyTo));

                            StoreLastRound();

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

                                StoreLastRound();

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

        string LastRoundSummary(string user)
        {
            int c = lastRound[user.ToLowerInvariant()].correct;
            int i = lastRound[user.ToLowerInvariant()].incorrect;
            string ratio = (i == 0 ? "Infinity!" : ((float)c / (float)i).ToString());
            int s = (c * 2) - i;

            return "(Score Change: " + (s >= 0 ? "+" + s.ToString() : s.ToString()) + " | Ratio: " + ratio + " | C/I: " + c + "/" + i + ")";
        }

        void Start()
        {
            lock (dataLock)
            {
                if (++index == words.Count)
                {
                    index = 0;
                    words.Shuffle();
                }

                word = words[index];
                guesses.Clear();
                playing = true;

                lastRound.Clear();
            }
        }

        void Stop()
        {
            lock (dataLock)
            {
                word = "";
                guesses.Clear();
                playing = false;
                badGuesses = 0;

                lastRound.Clear();
            }
        }

        string Guess(BotMessage message)
        {
            string guess = message.Parameters.ToLowerInvariant();

            lock (dataLock)
            {
                if (guess.Length == 1)
                    return GuessSingle(guess[0], message.User.Name);
                else
                    return GuessWord(guess, message.User.Name);
            }
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
            if (guess.Length != word.Length)
                return "Invalid " + WordOrPhrase(word) + " guess '" + guess + "', " + user;

            string visibleWord = VisibleWord(word, guesses);
            for (int i = 0; i < word.Length; i++)
            {
                if (visibleWord[i] == '_')
                    continue;

                if (guess[i] != visibleWord[i])
                    return "Invalid " + WordOrPhrase(word) + " guess '" + guess + "', " + user;
            }

            List<char> unknowns = new List<char>();
            if (word == guess)
            {
                int correct = 0;
                foreach (char c in word)
                {
                    if (!guesses.Contains(c) && !unknowns.Contains(c))
                    {
                        unknowns.Add(c);
                        ++correct;
                    }
                }

                IncrementCorrect(user, correct + 1);
                return null;
            }
            else
            {
                int incorrect = 0;
                foreach (char c in guess)
                {
                    if (!guesses.Contains(c) && !unknowns.Contains(c))
                    {
                        unknowns.Add(c);
                        ++incorrect;
                    }
                }

                IncrementIncorrect(user, incorrect);

                return "The " + WordOrPhrase(word) + " is not '" + guess + "'";
            }
        }

        string WordOrPhrase(string stringToTest)
        {
            return (stringToTest.Contains(" ") ? "phrase" : "word");
        }

        void IncrementCorrect(string user, int amount)
        {
            lock (sqlLock)
            {
                try
                {
                    using (DbCommand cmd = SQLiteInterface.Instance.Command)
                    {
                        cmd.CommandText =
                            "INSERT OR IGNORE INTO hangman VALUES (@user, 0, 0, 0, 0, 'no data');" +
                            "UPDATE hangman SET correct = correct + @amount WHERE user LIKE @user;";

                        DbParameter paramUser = cmd.CreateParameter();
                        paramUser.ParameterName = "@user";
                        paramUser.DbType = DbType.String;
                        paramUser.Direction = ParameterDirection.Input;
                        paramUser.Value = user.ToLowerInvariant();
                        cmd.Parameters.Add(paramUser);

                        DbParameter paramAmount = cmd.CreateParameter();
                        paramAmount.ParameterName = "@amount";
                        paramAmount.DbType = DbType.Int32;
                        paramAmount.Direction = ParameterDirection.Input;
                        paramAmount.Value = amount;
                        cmd.Parameters.Add(paramAmount);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Write("Correct: " + ex.Message + ex.StackTrace, Settings.Instance.ErrorFile);
                }
            }

            lock (dataLock)
            {
                if (!lastRound.ContainsKey(user.ToLowerInvariant()))
                    lastRound.Add(user.ToLowerInvariant(), new LastRound());

                lastRound[user.ToLowerInvariant()].correct += amount;
            }
        }

        void IncrementIncorrect(string user, int amount)
        {
            lock (sqlLock)
            {
                try
                {
                    using (DbCommand cmd = SQLiteInterface.Instance.Command)
                    {
                        cmd.CommandText =
                            "INSERT OR IGNORE INTO hangman VALUES (@user, 0, 0, 0, 0, 'no data');" +
                            "UPDATE hangman SET incorrect = incorrect + @amount WHERE user LIKE @user;";

                        DbParameter paramUser = cmd.CreateParameter();
                        paramUser.ParameterName = "@user";
                        paramUser.DbType = DbType.String;
                        paramUser.Direction = ParameterDirection.Input;
                        paramUser.Value = user.ToLowerInvariant();
                        cmd.Parameters.Add(paramUser);

                        DbParameter paramAmount = cmd.CreateParameter();
                        paramAmount.ParameterName = "@amount";
                        paramAmount.DbType = DbType.Int32;
                        paramAmount.Direction = ParameterDirection.Input;
                        paramAmount.Value = amount;
                        cmd.Parameters.Add(paramAmount);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Write("Incorrect: " + ex.Message + ex.StackTrace, Settings.Instance.ErrorFile);
                }
            }

            lock (dataLock)
            {
                if (!lastRound.ContainsKey(user.ToLowerInvariant()))
                    lastRound.Add(user.ToLowerInvariant(), new LastRound());

                lastRound[user.ToLowerInvariant()].incorrect++;
            }
        }

        void IncrementWord(string user, int amount)
        {
            lock (sqlLock)
            {
                try
                {
                    using (DbCommand cmd = SQLiteInterface.Instance.Command)
                    {
                        cmd.CommandText =
                            "INSERT OR IGNORE INTO hangman VALUES (@user, 0, 0, 0, 0, 'no data');" +
                            "UPDATE hangman SET word = word + @amount WHERE user LIKE @user;";

                        DbParameter paramUser = cmd.CreateParameter();
                        paramUser.ParameterName = "@user";
                        paramUser.DbType = DbType.String;
                        paramUser.Direction = ParameterDirection.Input;
                        paramUser.Value = user.ToLowerInvariant();
                        cmd.Parameters.Add(paramUser);

                        DbParameter paramAmount = cmd.CreateParameter();
                        paramAmount.ParameterName = "@amount";
                        paramAmount.DbType = DbType.Int32;
                        paramAmount.Direction = ParameterDirection.Input;
                        paramAmount.Value = amount;
                        cmd.Parameters.Add(paramAmount);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Write("Word: " + ex.Message + ex.StackTrace, Settings.Instance.ErrorFile);
                }
            }
        }

        void IncrementFinalLetter(string user, int amount)
        {
            lock (sqlLock)
            {
                try
                {
                    using (DbCommand cmd = SQLiteInterface.Instance.Command)
                    {
                        cmd.CommandText =
                            "INSERT OR IGNORE INTO hangman VALUES (@user, 0, 0, 0, 0, 'no data');" +
                            "UPDATE hangman SET finalLetter = finalLetter + @amount WHERE user LIKE @user;";

                        DbParameter paramUser = cmd.CreateParameter();
                        paramUser.ParameterName = "@user";
                        paramUser.DbType = DbType.String;
                        paramUser.Direction = ParameterDirection.Input;
                        paramUser.Value = user.ToLowerInvariant();
                        cmd.Parameters.Add(paramUser);

                        DbParameter paramAmount = cmd.CreateParameter();
                        paramAmount.ParameterName = "@amount";
                        paramAmount.DbType = DbType.Int32;
                        paramAmount.Direction = ParameterDirection.Input;
                        paramAmount.Value = amount;
                        cmd.Parameters.Add(paramAmount);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Write("Final Letter: " + ex.Message + ex.StackTrace, Settings.Instance.ErrorFile);
                }
            }
        }

        void StoreLastRound()
        {
            lock (sqlLock)
            {
                try
                {
                    foreach (var user in lastRound)
                    {
                        using (DbCommand cmd = SQLiteInterface.Instance.Command)
                        {
                            cmd.CommandText =
                                "UPDATE hangman SET lastRound = @lastRound WHERE user LIKE @user;";

                            DbParameter paramUser = cmd.CreateParameter();
                            paramUser.ParameterName = "@user";
                            paramUser.DbType = DbType.String;
                            paramUser.Direction = ParameterDirection.Input;
                            paramUser.Value = user.Key;
                            cmd.Parameters.Add(paramUser);

                            DbParameter paramLastRound = cmd.CreateParameter();
                            paramLastRound.ParameterName = "@lastRound";
                            paramLastRound.DbType = DbType.String;
                            paramLastRound.Direction = ParameterDirection.Input;
                            paramLastRound.Value = LastRoundSummary(user.Key);
                            cmd.Parameters.Add(paramLastRound);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.Write("Last Round: " + ex.Message + ex.StackTrace, Settings.Instance.ErrorFile);
                }
            }
        }

        string GetScore(string user)
        {
            try
            {
                int correct, incorrect, guessedWord, finalLetter;
                string lastRoundMessage;

                lock (sqlLock)
                {
                    using (DbCommand cmd = SQLiteInterface.Instance.Command)
                    {
                        cmd.CommandText =
                            "SELECT correct, incorrect, word, finalLetter, lastRound FROM hangman WHERE user LIKE @user";

                        DbParameter param = SQLiteInterface.Instance.Parameter;

                        param.ParameterName = "@user";
                        param.DbType = DbType.String;
                        param.Direction = ParameterDirection.Input;
                        param.Value = user.ToLowerInvariant();
                        cmd.Parameters.Add(param);

                        DbDataReader reader = cmd.ExecuteReader();

                        if (!reader.HasRows)
                            return "No scores held for " + user;

                        reader.Read();

                        correct = reader.GetInt32(reader.GetOrdinal("correct"));
                        incorrect = reader.GetInt32(reader.GetOrdinal("incorrect"));
                        guessedWord = reader.GetInt32(reader.GetOrdinal("word"));
                        finalLetter = reader.GetInt32(reader.GetOrdinal("finalLetter"));

                        if (reader.IsDBNull(reader.GetOrdinal("lastRound")))
                            lastRoundMessage = "no data";
                        else
                            lastRoundMessage = reader.GetString(reader.GetOrdinal("lastRound"));
                    }
                }

                string ratio = (incorrect > 0 ? ((float)correct / (float)incorrect).ToString() : "Infinity!");
                string score = ((correct * 2) - incorrect).ToString();

                return user + " - Score: " + score + " | Ratio: " + ratio + " | Correct/Incorrect: " + correct + "/" + incorrect + " | Last Letter: " + finalLetter + " | Whole Word: " + guessedWord + " | Last Round: " + lastRoundMessage;

            }
            catch (System.Exception ex)
            {
                Logger.Write("Get Score: " + ex.Message + ex.StackTrace, Settings.Instance.ErrorFile);
            }

            return "Couldn't open the database, sorry :(";
        }

        void LoadWords()
        {
            string path = Path.Combine(Settings.Instance.DataPath, "hangman.txt");

            lock (dataLock)
            {
                if (File.Exists(path))
                    words.AddRange(File.ReadAllLines(path));
                else
                    words.Add("add some words or phrases you fool");

                words.Shuffle();
            }
        }

        void SaveWords()
        {
            lock (dataLock)
            {
                File.WriteAllLines(Path.Combine(Settings.Instance.DataPath, "hangman.txt"), words.ToArray());
            }
        }
    }
}
