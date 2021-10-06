using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace GGroupp.Infra;

internal static class AuthorizationAppBuilderExtensions
{
    public static IApplicationBuilder UseAuthorization(
        this IApplicationBuilder app,
        Func<IServiceProvider, AuthorizationOptions> authorizationOptionsResolver)
        =>
        app.UseAuthorization(
            sp => new DefaultAuthorizationPolicyProvider(Options.Create(authorizationOptionsResolver.Invoke(sp))));

    public static IApplicationBuilder UseAuthorization(
        this IApplicationBuilder app,
        Func<IServiceProvider, IAuthorizationPolicyProvider> authorizationPolicyProviderResolver)
        =>
        app.Use(
            next => context => context.GetAuthorizationMiddleware(next, authorizationPolicyProviderResolver).Invoke(context));

    private static AuthorizationMiddleware GetAuthorizationMiddleware(
        this HttpContext context,
        RequestDelegate next,
        Func<IServiceProvider, IAuthorizationPolicyProvider> authorizationPolicyProviderResolver)
        =>
        new(next, authorizationPolicyProviderResolver.Invoke(context.RequestServices));
}
