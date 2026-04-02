using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RestoreSlotPoolCommand(Guid CharacterId, Guid PoolId) : ICommand;

internal sealed class RestoreSlotPoolCommandValidator : AbstractValidator<RestoreSlotPoolCommand>
{
	public RestoreSlotPoolCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.PoolId).NotEmpty();
	}
}

internal sealed class RestoreSlotPoolCommandHandler(
	ICharacterRepository characterRepository,
	IHubService hubService,
	IClaimsProvider claimsProvider)
	: ICommandHandler<RestoreSlotPoolCommand>
{
	public async Task<Result> Handle(RestoreSlotPoolCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var pool = character.SpellSlotPools.SingleOrDefault(p => p.Id == command.PoolId);

		var result = character.RestoreSlotPool(command.PoolId);
		if (result.IsFailure) return result.Error;

		if (pool is not null)
		{
			await hubService.PublishAsync(
				$"campaign:{character.CampaignId}",
				"ResourceChanged",
				new ResourceChangedNotification(
					character.Id,
					character.Name,
					character.CampaignId,
					$"{pool.Kind} Spell Slots",
					"used",
					"restored",
					claimsProvider.GetEmail()),
				cancellationToken);
		}

		return Result.Success();
	}
}
