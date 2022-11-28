namespace LeanplumSDK
{
    public class MessageDisplayChoice
    {
        public int DelaySeconds { get; set; }
        public DisplayChoice Choice { get; set; }

        public enum DisplayChoice
        {
            SHOW = 0,
            DISCARD,
            DELAY
        }

        private MessageDisplayChoice(DisplayChoice type, int delaySeconds = 0)
        {
            Choice = type;
            DelaySeconds = delaySeconds;
        }

        public static MessageDisplayChoice Show()
        {
            return new MessageDisplayChoice(DisplayChoice.SHOW);
        }

        public static MessageDisplayChoice Discard()
        {
            return new MessageDisplayChoice(DisplayChoice.DISCARD);
        }
        public static MessageDisplayChoice Delay(int delaySeconds)
        {
            return new MessageDisplayChoice(DisplayChoice.DELAY, delaySeconds);
        }
    }
}