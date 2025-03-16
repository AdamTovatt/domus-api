namespace DomusApi.Models.Devices
{
    public class DomusSubButton
    {
        private static TimeSpan maxTimeSinceInteraction = TimeSpan.FromDays(100);

        public Guid Id { get; set; }
        public DateTimeOffset? LastInteractionTime { get; set; }

        public DomusSubButton(Guid id, DateTimeOffset? lastInteractionTime)
        {
            Id = id;

            if (lastInteractionTime == null || DateTimeOffset.Now - lastInteractionTime < maxTimeSinceInteraction)
                LastInteractionTime = null;
            else
                LastInteractionTime = lastInteractionTime;
        }
    }
}
