namespace UserManagementAPI.Models;

public class User
{
    public int Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;
}
