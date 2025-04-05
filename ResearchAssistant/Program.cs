using ResearchAssistant.Features.PdfParsing;
using ResearchAssistant.Features.Summarize;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<PdfParser>();
builder.Services.AddScoped<PdfParsingHandler>();
builder.Services.AddScoped<SummarizationService>();
builder.Services.AddScoped<ChunkingService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader(); 
    });
});

var app = builder.Build();
app.UseCors("AllowAll");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Versiune 1");
        c.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
