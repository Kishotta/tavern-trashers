using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;
using TavernTrashers.Api.Modules.Characters.Domain.Resources;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RestoreGenericResourceCommand(Guid CharacterId, Guid ResourceId) : ICommand;

internal sealed class RestoreGenericResourceCommandValidator : AbstractValidator<RestoreGenericResourceCommand>
{
	public RestoreGenericResourceCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.ResourceId).NotEmpty();
	}
}

internal sealed class RestoreGenericResourceCommandHandler(
	ICharacterRepository characterRepository,
	IHubService hubService,
	IClaimsProvider claimsProvider)
	: ICommandHandler<RestoreGenericResourceCommand>
{
	public async Task<Result> Handle(RestoreGenericResourceCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var resource = character.GenericResources.SingleOrDefault(r => r.Id == command.ResourceId);
		var oldUses = resource?.CurrentUses ?? 0;

		var result = character.RestoreGenericResource(command.ResourceId);
		if (result.IsFailure) return result.Error;

		if (resource is not null)
		{
			await hubService.PublishAsync(
				$"campaign:{character.CampaignId}",
				"ResourceChanged",
				new ResourceChangedNotification(
					character.Id,
					character.Name,
					character.CampaignId,
					resource.Name,
					$"{oldUses}/{resource.MaxUses}",
					$"{resource.CurrentUses}/{resource.MaxUses}",
					claimsProvider.GetEmail()),
				cancellationToken);
		}

		return Result.Success();
	}
}
