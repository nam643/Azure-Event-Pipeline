namespace EventProcessor.Worker.Configuration;

using System.ComponentModel.DataAnnotations;
public class BlobStorageOptions
{
    public const string SectionName = "BlobStorage";
    
    [Required]
    public string ConnectionString {get;set;} = string.Empty;

    [Required]
    public string ContainerName {get;set;} = "batch-summaries";
}