namespace EventProcessor.Worker.Configuration;

using System.ComponentModel.DataAnnotations;
public class BlobStorageOptions
{
    public const string SectionName = "BlobStorage";
    public string? AccountUri {get;set;}
    public string? ConnectionString {get;set;} = string.Empty;

    [Required]
    public string ContainerName {get;set;} = "batch-summaries";

   public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(string.IsNullOrWhiteSpace(ConnectionString) && string.IsNullOrWhiteSpace(AccountUri))
        {
            yield return new ValidationResult("Either Cocnnection String or AccountUri must be provided.");
        }
    }
}