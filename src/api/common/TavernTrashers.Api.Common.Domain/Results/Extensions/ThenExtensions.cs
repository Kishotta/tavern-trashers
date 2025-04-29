namespace TavernTrashers.Api.Common.Domain.Results.Extensions;

public static class ThenExtensions
{
	public static Result<TValue> Then<TValue>(this Result result, Func<TValue> binder) =>
		result.IsSuccess
			? binder()
			: result.Error;

	public static Result<TValue> Then<TValue>(
		this Result<TValue> result,
		Func<TValue, TValue> binder) =>
		result.IsSuccess
			? binder(result)
			: result.Error;

	public static Result<TOut> Then<TIn, TOut>(
		this Result<TIn> result,
		Func<TIn, TOut> binder) =>
		result.IsSuccess
			? binder(result)
			: result.Error;

	public static async Task<Result> ThenAsync<TValue>(
		this Task<Result<TValue>> taskResult,
		Func<TValue, Result> binder)
	{
		var result = await taskResult.ConfigureAwait(false);

		return result.IsSuccess
			? binder(result)
			: result.Error;
	}

	public static async Task<Result<TOut>> ThenAsync<TIn, TOut>(
		this Task<Result<TIn>> taskResult,
		Func<TIn, TOut> binder)
	{
		var result = await taskResult.ConfigureAwait(false);
		return result.IsSuccess
			? binder(result)
			: result.Error;
	}

	public static async Task<Result<TOut>> ThenAsync<TIn, TOut>(
		this Task<Result<TIn>> taskResult,
		Func<TIn, Result<TOut>> binder)
	{
		var result = await taskResult.ConfigureAwait(false);
		return result.IsSuccess
			? binder(result)
			: result.Error;
	}

	public static async Task<Result<TOut>> ThenAsync<TIn, TOut>(
		this Task<Result<TIn>> taskResult,
		Func<TIn, Task<Result<TOut>>> binder)
	{
		var result = await taskResult.ConfigureAwait(false);
		return result.IsSuccess
			? await binder(result)
			: result.Error;
	}
}