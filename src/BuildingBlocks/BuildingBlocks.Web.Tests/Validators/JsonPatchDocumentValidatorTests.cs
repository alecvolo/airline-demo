using BuildingBlocks.Web.Validators;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace BuildingBlocks.Web.Tests.Validators;

public class JsonPatchDocumentValidatorTests
{
    private readonly JsonPatchDocumentValidator<TestModel> _validator;

    public class TestModel
    {
        public string? Name { get; set; }
    }

    public JsonPatchDocumentValidatorTests()
    {
        _validator = new JsonPatchDocumentValidator<TestModel>(new InlineValidator<TestModel>()
        {
            v => v.RuleFor(p => p.Name).NotNull().NotEmpty()
        });
    }
    public static IEnumerable<object[]> VaidTestData => new List<object[]>
    {
        new object[] { new Operation<TestModel> { op = "replace", path = "/name", value = "  " }, "name"},
        new object[] { new Operation<TestModel> { op = "replace", path = "name", value = null }, "name"},
        new object[] { new Operation<TestModel> { op = "remove", path = "/name" }, "name"},
        new object[] { new Operation<TestModel> { op = "replace", path = "/name", value = "value" }, "name"},
        new object[] { new Operation<TestModel> { op = "add", path = "/name", value = "value" }, "name"},
    };
    public static IEnumerable<object[]> InvalidTestData => new List<object[]>
    {
        new object[] { new Operation<TestModel> { op = "add2", path = "/name", value = "Barry" }, "op"},
        new object[] { new Operation<TestModel> { op = "", path = "/name", value = "Barry" }, "op" },
        new object[] { new Operation<TestModel> { op = "add", path = "/1name", value = "Barry" }, "path"},
        new object[] { new Operation<TestModel> { op = "replace", path = "/vname", value = "Barry" }, "path"},
    };
    [Theory]
    [MemberData(nameof(InvalidTestData))]
    public void Should_Raise_Validation_Error(Operation<TestModel> operation, string propertyName)
    {
        var jsonPathDocument = new JsonPatchDocument<TestModel>(new List<Operation<TestModel>>() { operation }, new DefaultContractResolver());
        var result = _validator.TestValidate(jsonPathDocument);
        result.ShouldHaveValidationErrorFor(propertyName);
    }
    [Theory]
    [MemberData(nameof(VaidTestData))]
    public void Should_Pass_Validation(Operation<TestModel> operation, string propertyName)
    {
        var jsonPathDocument = new JsonPatchDocument<TestModel>(new List<Operation<TestModel>>() { operation }, new DefaultContractResolver());
        var result = _validator.TestValidate(jsonPathDocument);
        result.ShouldNotHaveValidationErrorFor(propertyName);
    }
}