using System.Diagnostics.CodeAnalysis;

namespace TavernTrashers.Api.Common.Domain.Results;

/// <summary>
/// Represents the result of an operation, which can be either a success or a failure.
/// </summary>
/// <remarks>
/// The Result pattern provides a way to handle errors without throwing exceptions,
/// making error handling explicit and forcing callers to handle both success and failure cases.
/// This pattern is particularly useful for expected failures (validation, business rule violations)
/// rather than unexpected exceptions (system failures, bugs).
/// </remarks>
public class Result
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Result"/> class.
	/// </summary>
	/// <param name="isSuccess">Indicates whether the operation was successful.</param>
	/// <param name="error">The error that occurred, or <see cref="Error.None"/> if successful.</param>
	/// <exception cref="ArgumentException">
	/// Thrown when isSuccess is true but error is not Error.None, or when isSuccess is false but error is Error.None.
	/// </exception>
	protected Result(bool isSuccess, Error error)
	{
		if ((isSuccess && error != Error.None) ||
		    (!isSuccess && error == Error.None))
			throw new ArgumentException("Invalid error", nameof(error));

		IsSuccess = isSuccess;
		Error     = error;
	}

	/// <summary>
	/// Gets a value indicating whether the operation was successful.
	/// </summary>
	public bool IsSuccess { get; }
	
	/// <summary>
	/// Gets a value indicating whether the operation failed.
	/// </summary>
	public bool IsFailure => !IsSuccess;

	/// <summary>
	/// Gets the error that occurred during the operation, or <see cref="Error.None"/> if successful.
	/// </summary>
	public Error Error { get; }

	/// <summary>
	/// Creates a successful result.
	/// </summary>
	/// <returns>A successful <see cref="Result"/>.</returns>
	public static Result Success() => new(true, Error.None);

	/// <summary>
	/// Creates a successful result with a value.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="value">The value to include in the result.</param>
	/// <returns>A successful <see cref="Result{TValue}"/> containing the specified value.</returns>
	public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

	/// <summary>
	/// Creates a failed result with an error.
	/// </summary>
	/// <param name="error">The error that occurred.</param>
	/// <returns>A failed <see cref="Result"/>.</returns>
	public static Result Failure(Error error) => new(false, error);

	/// <summary>
	/// Creates a failed result with an error and no value.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="error">The error that occurred.</param>
	/// <returns>A failed <see cref="Result{TValue}"/>.</returns>
	public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

	/// <summary>
	/// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result"/>.
	/// </summary>
	/// <param name="error">The error to convert.</param>
	public static implicit operator Result(Error error) => Failure(error);
}

/// <summary>
/// Represents the result of an operation that returns a value, which can be either a success or a failure.
/// </summary>
/// <typeparam name="TValue">The type of the value returned on success.</typeparam>
/// <param name="value">The value returned if the operation was successful.</param>
/// <param name="isSuccess">Indicates whether the operation was successful.</param>
/// <param name="error">The error that occurred, or <see cref="Error.None"/> if successful.</param>
public class Result<TValue>(TValue? value, bool isSuccess, Error error) : Result(isSuccess, error)
{
	/// <summary>
	/// Gets the value of a successful result.
	/// </summary>
	/// <exception cref="InvalidOperationException">
	/// Thrown when attempting to access the value of a failed result.
	/// </exception>
	/// <remarks>
	/// Always check <see cref="Result.IsSuccess"/> before accessing this property
	/// to avoid exceptions.
	/// </remarks>
	[NotNull]
	public TValue Value => IsSuccess
		? value!
		: throw new InvalidOperationException("The value of a failed result cannot be accessed");

	/// <summary>
	/// Implicitly converts a <see cref="Result{TValue}"/> to its underlying value.
	/// </summary>
	/// <param name="result">The result to convert.</param>
	public static implicit operator TValue(Result<TValue> result) => result.Value;

	/// <summary>
	/// Implicitly converts a value to a successful <see cref="Result{TValue}"/>.
	/// </summary>
	/// <param name="value">The value to convert.</param>
	/// <remarks>
	/// If the value is null, returns a failure result with <see cref="Error.NullValue"/>.
	/// </remarks>
	public static implicit operator Result<TValue>(TValue value) =>
		value is not null
			? Success(value)
			: Error.NullValue;

	/// <summary>
	/// Implicitly converts an <see cref="Error"/> to a failed <see cref="Result{TValue}"/>.
	/// </summary>
	/// <param name="error">The error to convert.</param>
	public static implicit operator Result<TValue>(Error error) =>
		Failure<TValue>(error);

	/// <summary>
	/// Creates a validation failure result.
	/// </summary>
	/// <param name="error">The validation error that occurred.</param>
	/// <returns>A failed <see cref="Result{TValue}"/> with a validation error.</returns>
	public static Result<TValue> ValidationFailure(Error error) =>
		new(default, false, error);
}