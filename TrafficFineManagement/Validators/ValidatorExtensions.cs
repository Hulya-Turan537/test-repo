using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TrafficFineManagement.Validators;

public static class ValidatorExtensions
{
    public static async Task<bool> ValidateToModelStateAsync<T>(
        this IValidator<T> validator,
        T instance,
        ModelStateDictionary modelState)
    {
        var result = await validator.ValidateAsync(instance);
        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return result.IsValid;
    }
}
