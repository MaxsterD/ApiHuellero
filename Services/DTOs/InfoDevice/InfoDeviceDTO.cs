namespace ApiConsola.Services.DTOs.InfoDevice
{
    public class InfoDeviceDTO : IInfoDevice
    {
        public string? ipDevice { get; set; }
        public int? portDevice { get; set; }
        public int? passwordDevice { get; set; }

    }
}
