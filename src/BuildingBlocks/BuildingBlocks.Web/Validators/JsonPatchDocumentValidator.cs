using System;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace BuildingBlocks.Web.Validators;

public class JsonPatchDocumentValidator<TModel> : AbstractValidator<JsonPatchDocument<TModel>> where TModel : class, new()
{
    public JsonPatchDocumentValidator(IValidator<TModel> validator)
    {
        if (validator == null) throw new ArgumentNullException(nameof(validator));
        var properties = typeof(TModel).GetProperties()
            .ToDictionary(p => p.Name, p => p);
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleFor(p => p.Operations).Custom((operations, context) =>
        {
            foreach (var operation in context.InstanceToValidate.Operations)
            {
                if (operation.OperationType == OperationType.Invalid)
                {
                    context.AddFailure("op", $"Invalid value: {operation.op}");
                }

                var propertyName = GetPropertyName(operation.path);
                if (!properties.ContainsKey(propertyName))
                {
                    context.AddFailure("path", $"Invalid path: {operation.path}");
                }
            }
        });
        //Transform(x => x, to: document => ApplyPath(document)).SetValidator()
        // _validator.CreateDescriptor()
        Transform(x => x, to: document => ApplyPath(document)).Custom((model, context) =>
        {
            var result = validator.Validate(model,
                o => o.IncludeProperties(CollectUpdatedProperties(context.InstanceToValidate)));
            foreach (var validationFailure in result.Errors)
            {
                context.AddFailure(validationFailure);
            }
        });
    }

    private static string GetPropertyName(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }
        var propName = path
            .Split("/", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault();
        if (propName == null)
        {
            return string.Empty;
        }

        if (char.ToUpper(propName[0]) == propName[0])
        {
            return propName;
        }
        var name = propName.ToCharArray();
        name[0] = char.ToUpper(name[0]);
        return new string(name);
    }
    // apply jsonPath to the model
    private static TModel ApplyPath(JsonPatchDocument<TModel> patchDocument)
    {
        var model = new TModel();
        patchDocument.ApplyTo(model);
        return model;
    }
    // returns only updated properties
    private static string[] CollectUpdatedProperties(JsonPatchDocument<TModel> patchDocument)
        => patchDocument.Operations.Select(t => GetPropertyName(t.path)).Distinct().ToArray();

    //public override ValidationResult Validate(ValidationContext<JsonPatchDocument<T>> context)
    //{
    //    var result = base.Validate(context);
    //    if (!result.IsValid)
    //    {
    //        return result;
    //    }
    //    return _validator.Validate(ApplyPath(context.InstanceToValidate),
    //        o => o.IncludeProperties(CollectUpdatedProperties(context.InstanceToValidate)));
    //}

    //public override async Task<ValidationResult> ValidateAsync(ValidationContext<JsonPatchDocument<T>> context, CancellationToken cancellation = new())
    //{
    //    var result = await base.ValidateAsync(context, cancellation);
    //    if (!result.IsValid)
    //    {
    //        return result;
    //    }
    //    return await _validator.ValidateAsync(ApplyPath(context.InstanceToValidate),
    //        o => o.IncludeProperties(CollectUpdatedProperties(context.InstanceToValidate)), cancellation);
    //}
}