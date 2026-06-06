namespace EventBus.Messages.Events
{
    public class BaseIntegrationEvent
    {
        public string CorelationId { get; set; }
        public DateTime CreationDate { get; private set; }
        public BaseIntegrationEvent()
        {
            CorelationId = Guid.NewGuid().ToString();
            CreationDate = DateTime.UtcNow;
        }
        public BaseIntegrationEvent(Guid corelationId, DateTime creationdate)
        {
            CorelationId = corelationId.ToString();
            CreationDate = creationdate;
        }
    }
}