FROM mcr.microsoft.com/dotnet/aspnet:6.0

RUN mkdir /app
WORKDIR /app

COPY /home/runner/work/internal-support-bot-app/internal-support-bot-app/docker/publish/ ./

EXPOSE 80
ENTRYPOINT ["dotnet", "GGroupp.Internal.Support.Bot.Application.dll"]