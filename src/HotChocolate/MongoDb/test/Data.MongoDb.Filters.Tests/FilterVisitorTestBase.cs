using System;
using HotChocolate.Data.Filters;
using HotChocolate.Execution;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Squadron;

namespace HotChocolate.Data.MongoDb.Filters;

public class FilterVisitorTestBase
{
    private Func<IResolverContext, IExecutable<TResult>> BuildResolver<TResult>(
        MongoResource mongoResource,
        params TResult[] results)
        where TResult : class
    {
        IMongoCollection<TResult> collection =
            mongoResource.CreateCollection<TResult>("data_" + Guid.NewGuid().ToString("N"));

        collection.InsertMany(results);

        return ctx => collection.AsExecutable();
    }

    protected IRequestExecutor CreateSchema<TEntity, T>(
        TEntity[] entities,
        MongoResource mongoResource,
        bool withPaging = false)
        where TEntity : class
        where T : FilterInputType<TEntity>
    {
        Func<IResolverContext, IExecutable<TEntity>> resolver = BuildResolver(
            mongoResource,
            entities);

        return new ServiceCollection()
            .AddGraphQL()
            .AddObjectIdConverters()
            .AddFiltering(x => x.BindRuntimeType<TEntity, T>().AddMongoDbDefaults())
            .AddQueryType(
                c => c
                    .Name("Query")
                    .Field("root")
                    .Resolve(resolver)
                    .Use(
                        next => async context =>
                        {
                            await next(context);
                            if (context.Result is IExecutable executable)
                            {
                                context.ContextData["query"] = executable.Print();
                            }
                        })
                    .UseFiltering<T>())
            .AddType(new TimeSpanType(TimeSpanFormat.DotNet))
            .UseRequest(
                next => async context =>
                {
                    await next(context);
                    if (context.ContextData.TryGetValue("query", out var queryString))
                    {
                        context.Result =
                            QueryResultBuilder
                                .FromResult(context.Result!.ExpectQueryResult())
                                .SetContextData("query", queryString)
                                .Create();
                    }
                })
            .UseDefaultPipeline()
            .Services
            .BuildServiceProvider()
            .GetRequiredService<IRequestExecutorResolver>()
            .GetRequestExecutorAsync()
            .GetAwaiter()
            .GetResult();
    }
}
