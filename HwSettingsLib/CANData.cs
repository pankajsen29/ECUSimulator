namespace HwSettingsLib
{
    public class CANData
    {
        public string Timestamp { get; set; } = string.Empty;
        public uint Id { get; set; }
        public ushort Channel { get; set; }

        public byte[] Data;
        public string DataBytesHex => BitConverter.ToString(Data).Replace("-", " ");
        public uint Length { get; set; }
        public byte Dlc { get; set; }
        public byte Payload { get; set; }
        public bool IsCanFdFrame { get; set; }
        public bool IsRxData { get; set; }
    }
}
