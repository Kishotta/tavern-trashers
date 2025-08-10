using TavernTrashers.Api.Common.Domain.Results;

namespace TavernTrashers.Api.Common.Application.Data;

public static class IUnitOfWorkExtensions
{
	public static async Task<Result> SaveChangesAsync(
		this Result result,
		IUnitOfWork unitOfWork,
		CancellationToken cancellationToken = default)
	{
		if (result.IsSuccess)
		{
			await unitOfWork.SaveChangesAsync(cancellationToken);
		}

		return result;
	}
	
	public static async Task<Result<TValue>> SaveChangesAsync<TValue>(
		this Result<TValue> result,
		IUnitOfWork unitOfWork,
		CancellationToken cancellationToken = default)
	{
		if (result.IsSuccess)
		{
			await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		}

		return result;
	}
	
	public static async Task<Result> SaveChangesAsync(
		this Task<Result> resultTask,
		IUnitOfWork unitOfWork,
		CancellationToken cancellationToken = default)
	{
		var result = await resultTask.ConfigureAwait(false);

		if (result.IsSuccess)
		{
			await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		}

		return result;
	}
	
	public static async Task<Result<TValue>> SaveChangesAsync<TValue>(
		this Task<Result<TValue>> resultTask,
		IUnitOfWork unitOfWork,
		CancellationToken cancellationToken = default)
	{
		var result = await resultTask.ConfigureAwait(false);

		if (result.IsSuccess)
		{
			await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
		}

		return result;
	}
}