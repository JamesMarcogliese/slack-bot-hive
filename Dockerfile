FROM microsoft/dotnet:2.2-sdk as build

WORKDIR /publish

COPY nuget.config .
COPY Directory.Build.props .

COPY slack-bot-hive.sln .
COPY src/slack-bot-hive/*.csproj ./src/slack-bot-hive/
COPY test/slack-bot-hive.Tests/*.csproj ./test/slack-bot-hive.Tests/
COPY test/slack-bot-hive.FunctionalTests/*.csproj ./test/slack-bot-hive.FunctionalTests/

RUN dotnet restore --verbosity quiet

COPY test test
COPY src src

RUN dotnet build -c Release --no-restore /clp:ErrorsOnly
RUN dotnet test test/slack-bot-hive.Tests/slack-bot-hive.Tests.csproj -c Release --no-build --no-restore --verbosity minimal 
RUN dotnet test test/slack-bot-hive.FunctionalTests/slack-bot-hive.FunctionalTests.csproj -c Release --no-build --no-restore --verbosity minimal 
RUN dotnet publish src/slack-bot-hive/slack-bot-hive.csproj --output ../../out -c Release --no-restore --verbosity quiet /clp:ErrorsOnly

# Optimised final image
FROM microsoft/dotnet:2.2-aspnetcore-runtime-alpine

EXPOSE 80

ENTRYPOINT ["dotnet", "slack-bot-hive.dll"]

WORKDIR /app/src/slack-bot-hive

COPY --from=build /publish/out .