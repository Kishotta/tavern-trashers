using System.ComponentModel.DataAnnotations;

namespace TavernTrashers.Web.Models;

public record Campaign(Guid Id, string Name, string Description);

public class CreateCampaignRequest
{
	[Required] 
	public string Title { get; set; } = string.Empty;
	
	[Required]
	public string Description { get; set; } = string.Empty;
}