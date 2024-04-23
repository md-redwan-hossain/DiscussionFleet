using FluentValidation.Results;

namespace DiscussionFleet.HttpApi.Utils;

public static class FluentValidationUtils
{
    public static IEnumerable<Dictionary<string, object>> MapErrors(IEnumerable<ValidationFailure> errors)
    {
        return errors.Select(error =>
        {
            var errorInfo = new Dictionary<string, object>
            {
                { "propertyName", error.FormattedMessagePlaceholderValues["PropertyName"] },
                { "errorMessage", error.ErrorMessage },
                { "attemptedValue", error.AttemptedValue }
            };

            if (error.FormattedMessagePlaceholderValues.TryGetValue("CollectionIndex", out var index))
            {
                errorInfo["collectionIndex"] = index;
            }

            return errorInfo;
        });
    }
}