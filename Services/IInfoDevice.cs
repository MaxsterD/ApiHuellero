namespace ApiConsola.Services
{
    public interface IInfoDevice
    {
        string? ipDevice { get; set; }
        int? portDevice { get; set; }
        int? passwordDevice { get; set; }
    }
}
