using FluentValidation;
using TavernTrashers.Api.Common.Application.Messaging;
using TavernTrashers.Api.Common.Domain.Results;
using TavernTrashers.Api.Common.Domain.Results.Extensions;
using TavernTrashers.Api.Modules.Characters.Domain.Characters;

namespace TavernTrashers.Api.Modules.Characters.Application.Characters;

public sealed record SetHitPointFieldsCommand(
	Guid CharacterId,
	int? BaseMaxHitPoints,
	int? CurrentHitPoints,
	int? TemporaryHitPoints,
	int? MaxHitPointReduction) : ICommand<HitPointsResponse>;

internal sealed class SetHitPointFieldsCommandValidator : AbstractValidator<SetHitPointFieldsCommand>
{
	public SetHitPointFieldsCommandValidator()
	{
		RuleFor(x => x.CharacterId).NotEmpty();
		RuleFor(x => x.BaseMaxHitPoints).GreaterThanOrEqualTo(0).When(x => x.BaseMaxHitPoints.HasValue);
		RuleFor(x => x.CurrentHitPoints).GreaterThanOrEqualTo(0).When(x => x.CurrentHitPoints.HasValue);
		RuleFor(x => x.TemporaryHitPoints).GreaterThanOrEqualTo(0).When(x => x.TemporaryHitPoints.HasValue);
		RuleFor(x => x.MaxHitPointReduction).GreaterThanOrEqualTo(0).When(x => x.MaxHitPointReduction.HasValue);
	}
}

internal sealed class SetHitPointFieldsCommandHandler(
	ICharacterRepository characterRepository)
	: ICommandHandler<SetHitPointFieldsCommand, HitPointsResponse>
{
	public async Task<Result<HitPointsResponse>> Handle(SetHitPointFieldsCommand command, CancellationToken cancellationToken)
	{
		var characterResult = await characterRepository.GetAsync(command.CharacterId, cancellationToken);
		if (characterResult.IsFailure) return characterResult.Error;

		var result = characterResult.Value.SetHitPointFields(
			command.BaseMaxHitPoints,
			command.CurrentHitPoints,
			command.TemporaryHitPoints,
			command.MaxHitPointReduction);

		if (result.IsFailure) return result.Error;

		return (HitPointsResponse)characterResult.Value.HitPoints;
	}
}
