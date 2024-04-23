using DiscussionFleet.Application.MembershipFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.HttpApi.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.HttpApi.Endpoints;

public class AccountEndpoints : IApiEndpoint
{
    public static void DefineRoutes(IEndpointRouteBuilder routes)
    {
        var accountRoutes = routes.MapGroup("api/account/");

        accountRoutes.MapPost("/registration", Registration)
            .AddEndpointFilter<FluentValidationFilter<MemberRegistrationRequest>>()
            .Produces<ApiResponse<Member>>(statusCode: StatusCodes.Status201Created)
            .Produces<ApiResponse>(statusCode: StatusCodes.Status400BadRequest);
    }


    private async static Task<IResult> Registration([FromBody] MemberRegistrationRequest dto)
    {
        var member = new Member { Id = new MemberId(Guid.NewGuid()), FullName = "Redwan" };
        member.SetCreatedAtUtc(DateTime.UtcNow);

        return TypedResults.Json(member);
    }
}