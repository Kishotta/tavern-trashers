namespace TavernTrashers.Api.Common.Domain.Results.Extensions;

public static class EnsureSuccessExtensions
{
	public static async Task<Result<TValue>> EnsureSuccessAsync<TValue>(this Task<Result<TValue>> resultTask, Func<Error, Exception> onFailure)
	{
		var result = await resultTask.ConfigureAwait(false);

		if (result.IsFailure)
			throw onFailure(result.Error);

		return result;
	}
}