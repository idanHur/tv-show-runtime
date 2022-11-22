using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Home_Test
{
    public class ShowComperer : IComparer<Show>
    {
        public int Compare(Show x, Show y) // custome compare that orders by runtime and in case of same runtime orders by date (by recent) 
        {
            int dateComperasionRes = DateTime.Compare(x.endDate, y.endDate);
            if (x.episodesRuntime.runtime > y.episodesRuntime.runtime)
                return -1;
            else if (x.episodesRuntime.runtime == y.episodesRuntime.runtime)
            {
                if (dateComperasionRes < 0)
                    return 1;
                else if (dateComperasionRes == 0)
                    return 0;
                else
                    return -1;
            }
            else
                return 1;
        }
    }
    public class Show
    {
        public int id { get; set; }
        public string showName { get; set; }
        public Episodes episodesRuntime { get; set; }
        public DateTime endDate { get; set; }

        public Show(int id, String showName, String endDate)
        {
            this.id = id;
            this.showName = showName;
            this.endDate = Convert.ToDateTime(endDate);
            episodesRuntime = null;
        }
    }
    public class Episodes
    {
        public int runtime { get; }
        public Episodes(JArray jsonArray)
        {
            runtime = 0;
            foreach (JToken episode in jsonArray)
            {
                try// if there is no such field of "runtime" then it just skips this episode
                {
                    runtime += (int)episode["runtime"];
                }
                catch
                { }
            }
        }
    }
}