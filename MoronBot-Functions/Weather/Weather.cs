using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using CwIRC;
using MBFunctionInterface;
using MBUtilities;

using Newtonsoft.Json;

namespace Internet
{
    /// <summary>
    /// A Function which returns the weather at a specified location
    /// </summary>
    /// <suggester>TsukiakariUsagi (aka Emily)</suggester>
    public class Weather : Function
    {
        public Weather()
        {
            Help = "weather <location> - Uses WorldWeatherOnline.com's free API to find the weather at the specified location. You can specify with 'town/city, country', post/zipcode, latitude & longitude, or IP address.";
            Type = Types.Command;
            AccessLevel = AccessLevels.Anyone;
        }

        public override List<IRCResponse> GetResponse(BotMessage message)
        {
            if (!Regex.IsMatch(message.Command, @"^weather$", RegexOptions.IgnoreCase))
                return null;
            if (message.ParameterList.Count == 0)
                return new List<IRCResponse>() { new IRCResponse(ResponseType.Say, "You didn't give a location!", message.ReplyTo) };

            try
            {
                string query = HttpUtility.UrlEncode(message.Parameters);
                Stream responseStream = URL.SendToServer("http://free.worldweatheronline.com/feed/weather.ashx?format=json&num_of_days=0&key=f31c15f2f7142209122801&q=" + query);
                string jsonResponse = URL.ReceiveFromServer(responseStream);

                WeatherData data = JsonConvert.DeserializeObject<WeatherRoot>(jsonResponse).data;
                Request location = data.request[0];
                CurrentCondition weather = data.current_condition[0];

                return new List<IRCResponse>() { new IRCResponse(
                    ResponseType.Say,
                    String.Format("{0}: {1} | {2} | {3}ºC ({4}ºF) | Humidity: {5}% | Wind: {6}kph ({7}mph) {8}",
                        location.type,
                        location.query,
                        String.Join(", ", weather.weatherDesc),
                        weather.temp_C,
                        weather.temp_F,
                        weather.humidity,
                        weather.windspeedKmph,
                        weather.windspeedMiles,
                        weather.winddir16Point),
                    message.ReplyTo) };
            }
            catch (System.Exception ex)
            {
                Logger.Write(ex.Message, Settings.Instance.ErrorFile);
                return null;
            }
        }
    }
}
