namespace HeartFlame.ChatLevels
{
    public class ChatData
    {//level 4
        public int ChatLevel { get; set; }
        public int ChatExp { get; set; }
        public int MessagesSent { get; set; }
        public bool ExpPending { get; set; }
        public bool LevelPending { get; set; }

        public ChatData()
        {
            ChatLevel = 1;
        }

    }
}
