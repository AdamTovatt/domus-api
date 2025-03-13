
namespace DomusApi.Models.Devices
{
    public class DomusContactSensor : DomusControlDevice
    {
        public ContactSensorState CurrentState { get; set; }

        public DomusContactSensor(
            Guid id,
            string name,
            ConnectionStatus connectionStatus,
            DomusDeviceMetadata metadata,
            DateTime? lastInteractionTime,
            ContactSensorState currentState) : base(id, name, connectionStatus, metadata, lastInteractionTime)
        {
            CurrentState = currentState;
        }
    }
}
