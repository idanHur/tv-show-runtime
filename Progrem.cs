using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Home_Test
{
    class Progrem
    {
        public const String SEARCH_SHOW_URL = "https://api.tvmaze.com/search/shows?q=";
        public const String SEARCH_EPISODES_URL = "https://api.tvmaze.com/shows/";

        static async Task Main(string[] args)
        {
            if (args.Length != 0)
            {
                try
                {
                    String showName = "";
                    foreach (String str in args)
                        showName += str + " ";
                    showName = showName.Trim();
                    List<Show> temp = Progrem.getShowListByName(showName).Result;
                    temp.Sort(new ShowComperer());
                    if (temp.Count > 0)
                        Console.WriteLine(temp[0].episodesRuntime.runtime);
                    else
                        System.Environment.Exit(10);
                }
                catch (HttpRequestException e)
                {
                    System.Environment.Exit(1);
                }
            }
            else
                System.Environment.Exit(10);
        }


        public static async Task<List<Show>> getShowListByName(String showName)
        {
            List<Show> showsList = new List<Show>();
            JArray jsonArray = null;
            using (HttpClient client = new HttpClient())
            {
                var responce = await client.GetAsync(SEARCH_SHOW_URL + showName);
                if (!responce.IsSuccessStatusCode)
                    throw new Exception("Got " + responce.StatusCode.ToString() + " when sent a get requeset");

                var responceString = await responce.Content.ReadAsStringAsync();
                jsonArray = JArray.Parse(responceString);//because the respone we get is in an array "[]" and if we use JObject it will raise an exeption for that
            }
            foreach (JToken show in jsonArray)
            {
                if (showName.Equals((String)show["show"]["name"], StringComparison.InvariantCultureIgnoreCase))
                {
                    Show temp = new Show((int)show["show"]["id"], (String)show["show"]["name"], (String)show["show"]["ended"]);
                    Episodes episodes = (Episodes)Progrem.episodesById((int)show["show"]["id"]).Result;
                    temp.episodesRuntime = episodes;
                    showsList.Add(temp);
                }
            }
            return showsList;
        }


        public static async Task<Episodes> episodesById(int showId)
        {
            JArray jsonArray = null;
            using (HttpClient client = new HttpClient())
            {
                var responce = await client.GetAsync(SEARCH_EPISODES_URL + showId.ToString() + "/episodes");
                if (!responce.IsSuccessStatusCode)
                    throw new Exception("Got " + responce.StatusCode.ToString() + " when sent a get requeset");

                var responceString = await responce.Content.ReadAsStringAsync();
                jsonArray = JArray.Parse(responceString);//because the respone we get is in an array "[]" and if we use JObject it will raise an exeption for that
            }

            Episodes episodes = new Episodes(jsonArray);
            return episodes;
        }
    }


}
