using FluentValidation;

namespace DiscussionFleet.HttpApi.Utils;

public class FluentValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public FluentValidationFilter(IValidator<T> validator) => _validator = validator;


    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.Arguments.FirstOrDefault(x => x?.GetType() == typeof(T)) is not T instance)
        {
            return ApiEndpointResponse.Send(StatusCodes.Status422UnprocessableEntity);
        }

        var validationResult = await _validator.ValidateAsync(instance);
        if (validationResult.IsValid is false)
        {
            return ApiEndpointResponse.Send(StatusCodes.Status400BadRequest,
                FluentValidationUtils.MapErrors(validationResult.Errors));
        }

        return await next(context);
    }
}