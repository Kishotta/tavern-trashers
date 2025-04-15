namespace TavernTrashers.Api.Common.Domain.Results.Extensions;

public static class EnsureSuccessExtensions
{
	public static Result<TValue> EnsureSuccess<TValue>(this Result<TValue> result, Func<Error, Exception> exception)
	{
		if (result.IsFailure)
			throw exception(result.Error);

		return result;
	}

	public static async Task<Result> EnsureSuccessAsync(
		this Task<Result> resultTask,
		Func<Error, Exception> exception)
	{
		var result = await resultTask.ConfigureAwait(false);

		if (result.IsFailure)
			throw exception(result.Error);

		return result;
	}

	public static async Task<Result<TValue>> EnsureSuccessAsync<TValue>(
		this Task<Result<TValue>> resultTask,
		Func<Error, Exception> exception)
	{
		var result = await resultTask.ConfigureAwait(false);

		if (result.IsFailure)
			throw exception(result.Error);

		return result;
	}
}