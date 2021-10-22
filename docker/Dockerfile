FROM mcr.microsoft.com/dotnet/aspnet:6.0

RUN mkdir /app
WORKDIR /app
RUN ls
COPY ./publish ./

EXPOSE 80
ENTRYPOINT ["dotnet", "GGroupp.Internal.Support.Bot.Application.dll"]