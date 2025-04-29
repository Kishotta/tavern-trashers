namespace TavernTrashers.Api.Common.Domain.Results.Extensions;

public static class ResultDoExtensions
{
	public static Result Do(this Result result, Action action)
	{
		if (result.IsSuccess)
			action();

		return result;
	}
}

public static class ResultTDoExtensions
{
	public static Result<TValue> Do<TValue>(this Result<TValue> result, Action<TValue> action)
	{
		if (result.IsSuccess)
			action(result.Value);

		return result;
	}
}

public static class ResultDoAsyncExtensions
{
	public static async Task<Result> DoAsync(
		this Task<Result> resultTask,
		Action action)
	{
		var result = await resultTask.ConfigureAwait(false);

		if (result.IsSuccess)
			action();

		return result;
	}

	public static async Task<Result> DoAsync(
		this Task<Result> resultTask,
		Action<Result> action)
	{
		var result = await resultTask.ConfigureAwait(false);

		if (result.IsSuccess)
			action(result);

		return result;
	}
}

public static class ResultTDoAsyncExtensions
{
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
		this Result<TValue> result,
		Func<TValue, Task> action)
	{
		if (result.IsSuccess)
			await action(result);

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