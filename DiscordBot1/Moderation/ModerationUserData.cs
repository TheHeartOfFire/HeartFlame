using HeartFlame.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeartFlame.Moderation
{//level 4
    public class ModerationUserData
    {
        public string MuteTimer { get; set; }

        public ModerationUserData()
        {
            MuteTimer = DateTimeOffset.UtcNow.ToString();
        }

        public bool isMuted()
        {
            if (DateTimeOffset.Parse(MuteTimer) > DateTimeOffset.UtcNow)
                return true;
            return false;
        }

        public void Mute(TimeSpan Duration)
        {
            MuteTimer = DateTimeOffset.UtcNow.Add(Duration).ToString();
            PersistentData.SaveChangesToJson();
        }
        public void UnMute()
        {
            MuteTimer = DateTimeOffset.UtcNow.ToString();
            PersistentData.SaveChangesToJson();
        }
    }
}
