using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace YoutubeWatchAnalytics
{
    public class RawVideo
    {
        public string header { get; set; }
        public string title { get; set; }
        public string titleUrl { get; set; }
        public string[] Subtitles { get; set; }
        public string time { get; set; }
        public string products { get; set; }
        public string activityControls { get; set; }
    }
}
