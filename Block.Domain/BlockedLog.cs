﻿namespace Block.Domain;

public class BlockedLog
{
    public string IpAddress { get; set; }
    public DateTime Timestamp { get; set; }
    public string CountryCode { get; set; }
    public bool IsBlocked { get; set; }
    public string UserAgent { get; set; }
}
