var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
var beers = new Beer[]
{
    new Beer("Pikantus","Erdinger"),
    new Beer("Tremens", "Deliriym"),
    new Beer("IPA", "Minerva")
};


app.MapGet("/beers/{quantity}", (int quantity) => 
{
    return beers.Take(quantity);
}
).AddEndpointFilter(async (context, next) => 
{
    int quantity = context.GetArgument<int>(0);
    if(quantity < 0 )
    { return Results.Problem("Debe ser mayor a 0"); }
    
    return await next(context);
}).AddEndpointFilter<MyFilter>();

app.Run();

internal record Beer(string name, string brand);

public class MyFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        int quantity = context.GetArgument<int>(0);
        if (quantity > 20)
        { 
            return Results.Problem("Debe ser menor a 20"); 
        }

        return await next(context);
    }
}
