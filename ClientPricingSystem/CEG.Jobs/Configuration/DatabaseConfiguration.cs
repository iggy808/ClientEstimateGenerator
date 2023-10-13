namespace CEG.Jobs.Configuration;
public class DatabaseConfiguration
{
    public string? DefaultConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public string? Clients { get; set; }
    public string? Vendors { get; set; }
    public string? Orders { get; set; }
    public string? OrderItems { get; set; }
}