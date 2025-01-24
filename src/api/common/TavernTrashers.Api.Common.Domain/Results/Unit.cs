namespace TavernTrashers.Api.Common.Domain.Results;

public struct Unit
{
	public static readonly Result Value = Result.Success();
	public static readonly Task<Result> TaskValue = Task.FromResult(Value);

	public static implicit operator Result(Unit _) => Value;
}