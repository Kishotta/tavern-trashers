using FluentValidation;
using TavernTrashers.Api.Common.Application.Authentication;
using TavernTrashers.Api.Common.Application.Hubs;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Application.Hubs;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record RemoveConditionCommand(Guid CharacterId, Conditions Condition) : ICommand<CharacterResponse>;

internal sealed class RemoveConditionCommandValidator : AbstractValidator<RemoveConditionCommand>
{
	public RemoveConditionCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.Condition).IsInEnum();
	}
}

internal sealed class RemoveConditionCommandHandler(
	ICharacterRepository characterRepository,
	IHubService hubService,
	IClaimsProvider claimsProvider)
	: ICommandHandler<RemoveConditionCommand, CharacterResponse>
{
	public async Task<Result<CharacterResponse>> Handle(RemoveConditionCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var character = characterResult.Value;
		var oldConditions = character.Conditions;

		character.RemoveCondition(command.Condition);

		await hubService.PublishAsync(
			$"campaign:{character.CampaignId}",
			"ResourceChanged",
			new ResourceChangedNotification(
				character.Id,
				character.Name,
				character.CampaignId,
				"Conditions",
				oldConditions.ToString(),
				character.Conditions.ToString(),
				claimsProvider.GetEmail()),
			cancellationToken);

		return (CharacterResponse)character;
	}
}
