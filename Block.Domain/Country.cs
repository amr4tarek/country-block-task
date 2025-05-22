namespace Block.Domain;

public class Country
{
    public string CountryCode { get; set; }
    public string CountryName { get; set; }
    public DateTime? TemporaryBlockExpiry { get; set; }

    public bool IsTemporarilyBlocked =>
        TemporaryBlockExpiry.HasValue && TemporaryBlockExpiry.Value > DateTime.UtcNow;
}
