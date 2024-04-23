using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using SharpOutcome;

namespace DiscussionFleet.HttpApi.Utils;

public static class ApiEndpointResponse
{
    public static IResult Send(int code, object data, string message,
        JsonSerializerOptions? jsonSerializerOptions = null,
        string contentType = "application/json"
    )
    {
        return MakeResponse(code, data, message, jsonSerializerOptions, contentType);
    }

    public static IResult Send(int code, object data,
        JsonSerializerOptions? jsonSerializerOptions = null,
        string contentType = "application/json")
    {
        return MakeResponse(code, data, null, jsonSerializerOptions, contentType);
    }

    public static IResult Send(int code, string message,
        JsonSerializerOptions? jsonSerializerOptions = null,
        string contentType = "application/json")
    {
        return MakeResponse(code, null, message, jsonSerializerOptions, contentType);
    }


    public static IResult Send(int code, JsonSerializerOptions? jsonSerializerOptions = null,
        string contentType = "application/json")
    {
        return MakeResponse(code, null, null, jsonSerializerOptions, contentType);
    }


    public static IResult Send(IBadOutcome badOutcome,
        JsonSerializerOptions? jsonSerializerOptions = null,
        string contentType = "application/json")
    {
        var code = badOutcome.Tag switch
        {
            BadOutcomeTag.BadRequest => StatusCodes.Status400BadRequest,
            BadOutcomeTag.ValidationFailure => StatusCodes.Status400BadRequest,
            BadOutcomeTag.Unprocessable => StatusCodes.Status422UnprocessableEntity,
            BadOutcomeTag.Conflict => StatusCodes.Status409Conflict,
            BadOutcomeTag.NotFound => StatusCodes.Status404NotFound,
            BadOutcomeTag.Unauthorized => StatusCodes.Status401Unauthorized,
            BadOutcomeTag.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        return MakeResponse(code, null, badOutcome.Reason, jsonSerializerOptions, contentType);
    }


    public static IResult Send(IGoodOutcome goodOutcome,
        JsonSerializerOptions? jsonSerializerOptions = null,
        string contentType = "application/json")
    {
        var code = goodOutcome.Tag switch
        {
            GoodOutcomeTag.Ok => StatusCodes.Status200OK,
            GoodOutcomeTag.Created => StatusCodes.Status201Created,
            GoodOutcomeTag.Deleted => StatusCodes.Status204NoContent,
            _ => StatusCodes.Status200OK,
        };

        return MakeResponse(code, null, goodOutcome.Reason, jsonSerializerOptions, contentType);
    }


    private static IResult MakeResponse(int code, object? data, string? message,
        JsonSerializerOptions? jsonSerializerOptions = null,
        string contentType = "application/json")
    {
        if (code == StatusCodes.Status204NoContent) return TypedResults.NoContent();

        var isSuccess = code is >= 200 and < 300;
        var res = new ApiResponse
        {
            Success = isSuccess,
            Message = message ?? ReasonPhrases.GetReasonPhrase(code),
            Code = code,
            Data = data
        };
        return TypedResults.Json(res, jsonSerializerOptions, contentType, code);
    }
}