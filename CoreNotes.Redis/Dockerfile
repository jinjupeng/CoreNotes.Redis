FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["CoreNotes.Redis/CoreNotes.Redis.csproj", "CoreNotes.Redis/"]
RUN dotnet restore "CoreNotes.Redis/CoreNotes.Redis.csproj"
COPY . .
WORKDIR "/src/CoreNotes.Redis"
RUN dotnet build "CoreNotes.Redis.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoreNotes.Redis.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CoreNotes.Redis.dll"]