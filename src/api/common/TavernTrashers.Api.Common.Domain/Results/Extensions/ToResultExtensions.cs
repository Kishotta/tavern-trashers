namespace TavernTrashers.Api.Common.Domain.Results.Extensions;

public static class ToResultExtensions
{
	public static Result<TValue> ToResult<TValue>(this TValue value) =>
		value is not null
			? value
			: Error.NullValue;

	public static Result<TValue> ToResult<TValue>(this TValue? value, Error error) =>
		value is not null
			? value
			: error;

	public static async Task<Result<TResult>> ToResultAsync<TResult>(this Task<TResult?> taskResult, Error error)
	{
		var value = await taskResult.ConfigureAwait(false);
		return value.ToResult(error);
	}
}