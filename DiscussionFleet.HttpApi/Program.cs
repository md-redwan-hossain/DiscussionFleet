using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiscussionFleet.Application;
using DiscussionFleet.HttpApi.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationLayer();


builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(
        namingPolicy: JsonNamingPolicy.CamelCase,
        allowIntegerValues: false)
    );
});

builder.Services.AddSwaggerGen(o =>
{
    o.SupportNonNullableReferenceTypes();
    o.UseAllOfToExtendReferenceSchemas();
    o.SchemaFilter<ApiResponseSchemaFilter>();
});

ValidatorOptions.Global.DisplayNameResolver = (_, member, _) =>
    member is not null ? JsonNamingPolicy.CamelCase.ConvertName(member.Name) : null;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapApiEndpointsFromAssembly(Assembly.GetExecutingAssembly());

app.Run();