﻿using System;
using System.Collections.Generic;
using System.Text;

using CwIRC;
using MBFunctionInterface;

namespace Automatic
{
    public class Apathy : Function
    {
        Random rand = new Random();

        struct Response
        {
            public Response(string text, ResponseType rType)
            {
                Text = text;
                RType = rType;
            }
            public string Text;
            public ResponseType RType;
        }

        List<List<Response>> insults = new List<List<Response>>();

        public Apathy()
        {
            Help = "Meh, I can't be bothered to fill this out.";
            Type = Types.Command;
            AccessLevel = AccessLevels.UserList;

            FuncInterface.CommandFormatMessageReceived += commandReceived;

            AccessList.Add("sirgir");
            AccessList.Add("aeltrius");
            AccessList.Add("trahsi");
            AccessList.Add("xelareko");
            AccessList.Add("xelareco");
            AccessList.Add("aerocmdr");
            AccessList.Add("knishimura");
            AccessList.Add("kiraisl");
            AccessList.Add("pikachaos");
            AccessList.Add("tyranic-moron");
            AccessList.Add("maerarde");
            AccessList.Add("dragoon");

            InitInsults();
        }

        void commandReceived(object sender, BotMessage message)
        {
            if (rand.Next(0, 100) == 0)
            {
                List<BotResponse> responses = new List<BotResponse>();
                foreach (Response response in GetInsult(message.User.Name))
                {
                    responses.Add(new BotResponse(response.RType, response.Text, message.ReplyTo));
                }
                responses.Add(null);
                FuncInterface.SendResponses(responses);
            }
        }

        void InitInsults()
        {
            AddInsult("I'm a bit busy at the moment NICK, can it wait?", ResponseType.Say);
            AddInsult("I don't feel like it right now, NICK", ResponseType.Say);
            AddInsult("It's always 'do this', 'do that' with you NICK :|", ResponseType.Say);
            AddInsult("That command NICK? Really?", ResponseType.Say);
            AddInsult("There's a time and place for everything, NICK, but not now.", ResponseType.Say);
            AddInsult("Why don't you go do it yourself, NICK?", ResponseType.Say);
            AddInsult("I really want to do that, NICK, but I don't take orders from idiots", ResponseType.Say); // XelaReko
            AddInsult("I'm in the middle of some calibrations, NICK, can it wait?", ResponseType.Say); // XelaReko
            AddInsult("Sometimes I wonder if my life would be better if I was the bot for a channel of more interesting things. Like snails. That is correct. I would rather talk to snails than you, NICK", ResponseType.Say); // XelaReko
            AddInsult("Ugh, what now NICK? Can't you wait a little bit longer? I have things to do, you know!", ResponseType.Say); // Dragoon

            AddInsult(new List<Response>() // XelaReko
            {
                new Response("yawns", ResponseType.Do),
                new Response("Call me when you have something worthwhile to say, NICK", ResponseType.Say)
            });
        }

        void AddInsult(string text, ResponseType rType)
        {
            insults.Add(new List<Response>() { new Response(text, rType) });
        }

        void AddInsult(List<Response> responses)
        {
            insults.Add(responses);
        }

        List<Response> GetInsult(string userName)
        {
            List<Response> responses = new List<Response>();
            foreach (Response response in insults[rand.Next(insults.Count)])
            {
                Response parsedResponse = response;
                parsedResponse.Text = parsedResponse.Text.Replace("NICK", userName);
                responses.Add(parsedResponse);
            }
            return responses;
        }
    }
}
