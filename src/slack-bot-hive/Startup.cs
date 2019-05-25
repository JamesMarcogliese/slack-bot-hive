using System;
using MediatR;
using System.Reflection;
using slack_bot_hive.Services.Elasticsearch;
using slack_bot_hive.Services.SlackApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using slack_bot_hive.Configuration;
using slack_bot_hive.Services;
using slack_bot_hive.Services.AttachmentGenerators;
using slack_bot_hive.Services.Commands;
using slack_bot_hive.Services.Filters;
using slack_bot_hive.Services.Hangfire;
using Hangfire;
using Hangfire.MemoryStorage;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace slack_bot_hive
{
    public class Startup
    {
        private const string NotSet = "NOT SET";

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _instanceId = Guid.NewGuid().ToString();
        private readonly string _gitSha;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;

            Configuration = configuration;

            var gitSha = Configuration.GetSection("Environment")["GitSha"];
            //_gitSha = gitSha.IsPresent() ? gitSha : NotSet;

            ConfigureSerilogLogger();

        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services.AddDiagnostics(dc =>
            //{
            //    dc.WithEnvironment(_hostingEnvironment.EnvironmentName);
            //    dc.WithInstanceId(_instanceId);
            //    dc.WithMachineName(Environment.MachineName);

            //    if (_gitSha != NotSet)
            //    {
            //        dc.WithGitCommitSha(_gitSha);
            //    }
            //})
            //.AddDataDog();

            services.AddHangfire(c => c.UseMemoryStorage());
            services.AddMemoryCache();
            // AppConfiguration Manager
            services.AddOptions<AppConfiguration>()
                .Bind(Configuration.GetSection("ConfigurationManager"))
                .ValidateDataAnnotations();
            // Mediator
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
            // Factories & Providers
            services.AddScoped<IElasticsearchInitService, ElasticsearchInitService>();
            services.AddSingleton<ISlackWebApiProvider, SlackWebApiProvider>();
            services.AddSingleton<IElasticsearchClientProvider, ElasticsearchClientProvider>();
            services.AddSingleton<IDisappearingSlackMessageProvider, DisappearingSlackMessageProvider>();
            // Event Filters
            services.AddSingleton<IEventFilterBuilder, EventFilterBuilder>();
            // Services
            services.AddSingleton<ISlackVerificationService, SlackVerificationService>();
            services.AddSingleton<FindNoteAttachmentGenerator>();
            services.AddSingleton<ReviewNotesAttachmentGenerator>();
            // Commands
            services.AddScoped<ICommand, FindNoteCommand>();
            services.AddScoped<ICommand, HelpCommand>();
            services.AddScoped<ICommand, ReviewNotesCommand>();
            services.AddScoped<ICommand, SaveNoteCommand>();
            services.AddScoped<ICommand, SetTeamCommand>();
            services.AddScoped<ICommand, FallbackCommand>();    // FallbackCommand must be last of commands.
            services.AddScoped<ICommandStrategy, CommandStrategy>();

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IElasticsearchInitService elasticsearchInitService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Map("/healthcheck", a => a.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                await context.Response.WriteAsync("OK");
            }));

            app.UseHangfireServer();
            app.UseMvc();

            elasticsearchInitService.InitElasticsearchIndices();
        }
        
        private void ConfigureSerilogLogger()
        {
            var esLoggingServer = Configuration.GetValue<string>("Logging:ElasticSearchLoggingServer");

            var logConfig = new LoggerConfiguration()
                .MinimumLevel.Warning()
                .MinimumLevel.Override("Madgex", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine:l}{Message:lj}{NewLine}{Exception}{NewLine:l}")
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(esLoggingServer))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv5,
                    IndexFormat = "todo-index-prefix-{0:yyyy.MM.dd}", // must be lower case!
                    MinimumLogEventLevel = LogEventLevel.Information
                })
                .Enrich.WithProperty("ApplicationName", _hostingEnvironment.ApplicationName)
                .Enrich.WithProperty("ApplicationInstanceId", _instanceId)
                .Enrich.WithProperty("EnvironmentName", _hostingEnvironment.EnvironmentName)
                .Enrich.WithProperty("MachineName", Environment.MachineName);

            if (_gitSha != NotSet)
            {
                logConfig.Enrich.WithProperty("GitCommitSha", _gitSha);
            }

            Log.Logger = logConfig.CreateLogger();
        }
    }
}