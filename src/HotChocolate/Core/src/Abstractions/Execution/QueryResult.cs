using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotChocolate.Properties;

#nullable enable

namespace HotChocolate.Execution;

/// <summary>
/// Represents a query result object.
/// </summary>
public sealed class QueryResult : ExecutionResult, IQueryResult
{
    internal QueryResult(
        IReadOnlyDictionary<string, object?>? data,
        IReadOnlyList<IError>? errors,
        IReadOnlyDictionary<string, object?>? extension,
        IReadOnlyDictionary<string, object?>? contextData,
        string? label,
        Path? path,
        bool? hasNext,
        Func<ValueTask>[] cleanupTasks)
        : base(cleanupTasks)
    {
        if (data is null && errors is null && hasNext is not false)
        {
            throw new ArgumentException(
                AbstractionResources.QueryResult_DataAndResultAreNull,
                nameof(data));
        }

        Data = data;
        Errors = errors;
        Extensions = extension;
        ContextData = contextData;
        Label = label;
        Path = path;
        HasNext = hasNext;
    }

    /// <summary>
    /// Initializes a new <see cref="QueryResult"/>.
    /// </summary>
    public QueryResult(
        IReadOnlyDictionary<string, object?>? data,
        IReadOnlyList<IError>? errors = null,
        IReadOnlyDictionary<string, object?>? extension = null,
        IReadOnlyDictionary<string, object?>? contextData = null,
        string? label = null,
        Path? path = null,
        bool? hasNext = null)
    {
        if (data is null && errors is null && hasNext is not false)
        {
            throw new ArgumentException(
                AbstractionResources.QueryResult_DataAndResultAreNull,
                nameof(data));
        }

        Data = data;
        Errors = errors;
        Extensions = extension;
        ContextData = contextData;
        Label = label;
        Path = path;
        HasNext = hasNext;
    }

    /// <inheritdoc />
    public override ExecutionResultKind Kind => ExecutionResultKind.SingleResult;

    /// <inheritdoc />
    public string? Label { get; }

    /// <inheritdoc />
    public Path? Path { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?>? Data { get; }

    /// <inheritdoc />
    public IReadOnlyList<IError>? Errors { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?>? Extensions { get; }

    /// <inheritdoc />
    public override IReadOnlyDictionary<string, object?>? ContextData { get; }

    /// <inheritdoc />
    public bool? HasNext { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object?> ToDictionary()
        => QueryResultHelper.ToDictionary(this);
}
