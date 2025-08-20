namespace ToursApp.Domain.Common;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public string CreatedBy { get; init; } // Changed from 'required' to 'init'
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
    protected AuditableEntity(string createdBy)
    {
        CreatedBy = createdBy;
    }
    
    protected AuditableEntity() { }
}