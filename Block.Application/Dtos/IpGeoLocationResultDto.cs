namespace Block.Application.Dtos;

public class IpGeoLocationResultDto
{
    public string Ip { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string Isp { get; set; } = string.Empty;
}
