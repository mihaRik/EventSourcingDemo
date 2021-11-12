#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["../EventSourcingDemo/EventSourcingDemo.csproj", "EventSourcingDemo/"]
COPY ["../EventSourcingDemo.Logic/EventSourcingDemo.Logic.csproj", "EventSourcingDemo.Logic/"]
COPY ["../EventSourcingDemo.ReadModel/EventSourcingDemo.ReadModel.csproj", "EventSourcingDemo.ReadModel/"]
COPY ["../EventSourcingDemo.EventStore/EventSourcingDemo.EventStore.csproj", "EventSourcingDemo.EventStore/"]
COPY ["../EventSourcingDemo.Domain/EventSourcingDemo.Domain.csproj", "EventSourcingDemo.Domain/"]
RUN dotnet restore "EventSourcingDemo/EventSourcingDemo.csproj"
COPY . .
WORKDIR "/src/EventSourcingDemo"
RUN dotnet build "EventSourcingDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EventSourcingDemo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventSourcingDemo.dll"]