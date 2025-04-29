namespace TavernTrashers.Api.Common.Domain.Results.Extensions;

public static class EnsureExtensions
{
	public static Result Ensure(this Result result, Func<bool> condition, Error error) =>
		condition() ? result : error;

	public static Result<TValue> Ensure<TValue>(this Result<TValue> result, Func<bool> condition, Error error) =>
		condition() ? result : error;

	public static Result<TValue> Ensure<TValue>(
		this Result<TValue> result,
		Func<TValue, bool> condition,
		Error error) =>
		condition(result) ? result : error;

	public static async Task<Result<TValue>> EnsureAsync<TValue>(
		this Task<TValue> taskValue,
		Func<TValue, bool> predicate,
		Error error)
	{
		var value = await taskValue.ConfigureAwait(false);
		return predicate(value) ? value : error;
	}

	public static async Task<Result<TValue>> EnsureAsync<TValue>(
		this Task<Result<TValue>> taskResult,
		Func<TValue, bool> predicate,
		Error error)
	{
		var result = await taskResult.ConfigureAwait(false);
		return result.IsSuccess && predicate(result.Value)
			? result
			: error;
	}
}