namespace TavernTrashers.Api.Common.Domain.Results.Extensions;

public static class DoExtensions
{
	public static Result<TValue> Do<TValue>(this Result<TValue> result, Action<TValue> action)
	{
		if (result.IsSuccess)
			action(result.Value);

		return result;
	}

	public static async Task<Result<TValue>> DoAsync<TValue>(this Result<TValue> result, Func<TValue, Task> action)
	{
		if (result.IsSuccess)
			await action(result);

		return result;
	}

	public static async Task<Result<TValue>> DoAsync<TValue>(
		this Task<Result<TValue>> resultTask,
		Action<TValue> action)
	{
		var result = await resultTask.ConfigureAwait(false);

		if (result.IsSuccess)
			action(result);

		return result;
	}

	public static async Task<Result<TValue>> DoAsync<TValue>(
		this Task<Result<TValue>> resultTask,
		Func<TValue, Task> action)
	{
		var result = await resultTask.ConfigureAwait(false);

		if (result.IsSuccess)
			await action(result);

		return result;
	}
}