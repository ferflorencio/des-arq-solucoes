using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using SolutionArchitect.CashFlow.Api.Endpoints;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Builder;

namespace SolutionArchitect.CashFlow.Api.Tests.Endpoints;

public sealed class DefaultEndpointsTests
{
    private sealed class TestEndpointRouteBuilder : IEndpointRouteBuilder
    {
        public TestEndpointRouteBuilder()
        {
            DataSources = new List<EndpointDataSource>();
            ServiceProvider = new ServiceCollection().BuildServiceProvider();
        }

        public ICollection<EndpointDataSource> DataSources { get; }

        public IServiceProvider ServiceProvider { get; }

        public IApplicationBuilder CreateApplicationBuilder()
            => new ApplicationBuilder(ServiceProvider);
    }

    [Fact]
    public void MapAppEndpoints_Registers_PostEndpoint_WithExpectedRouteAndMetadata()
    {
        // Arrange
        var builder = new TestEndpointRouteBuilder();

        // Act
        DefaultEndpoints.MapAppEndpoints(builder);

        // Assert
        var endpoints = builder.DataSources.SelectMany(ds => ds.Endpoints).ToList();
        Assert.NotEmpty(endpoints);

        var routeEndpoint = endpoints.OfType<RouteEndpoint>().FirstOrDefault();
        Assert.NotNull(routeEndpoint);

        Assert.Equal("CashFlow", routeEndpoint!.DisplayName);

        var pattern = routeEndpoint.RoutePattern?.RawText ?? routeEndpoint.Metadata.GetMetadata<RoutePattern>()?.RawText;
        Assert.NotNull(pattern);
        Assert.Contains("api/v1/cashflow", pattern);

        var httpMeta = routeEndpoint.Metadata.GetMetadata<IHttpMethodMetadata>();
        Assert.NotNull(httpMeta);
        Assert.Contains("POST", httpMeta!.HttpMethods);
    }
}