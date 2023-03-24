using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeWatchAnalytics
{
    public class VideoData
    {
        public string URL { get; private set; }
        public string Title { get; private set; }
        public string ChannelName { get; private set; }
        public TimeSpan Duration { get; private set; }

        public VideoData(string uRL, string title, string channelName, TimeSpan time)
        {
            URL = uRL;
            Title = title;
            ChannelName = channelName;
            Duration = time;
        }
    }
}
