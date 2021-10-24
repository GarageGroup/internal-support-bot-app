FROM mcr.microsoft.com/dotnet/aspnet:6.0

RUN mkdir /app
WORKDIR /app

COPY ./publish ./
RUN ls

EXPOSE 80
ENTRYPOINT ["dotnet", "GGroupp.Internal.Support.Bot.Application.dll"]
