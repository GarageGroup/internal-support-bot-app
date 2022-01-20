FROM mcr.microsoft.com/dotnet/aspnet:6.0

ARG BOT_BUILD_VERSION
ENV BotBuildVersion $BOT_BUILD_VERSION

ARG BOT_BUILD_DATE_TIME
ENV BotBuildDateTime $BOT_BUILD_DATE_TIME

RUN mkdir /app
WORKDIR /app

COPY ./publish ./

EXPOSE 80
ENTRYPOINT ["dotnet", "GGroupp.Internal.Support.Bot.Application.dll"]
