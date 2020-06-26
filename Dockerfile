FROM mcr.microsoft.com/dotnet/core/runtime:3.1.4-focal AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-focal AS build

WORKDIR /src
COPY *.sln .
COPY ["dockerECS/*.csproj", "dockerECS/"]

RUN dotnet restore

# copy full solution over
COPY . .
RUN dotnet build

FROM build AS release
WORKDIR "/src/dockerECS"
RUN dotnet build "dockerECS.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/dockerECS"
RUN dotnet publish "dockerECS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dockerECS.dll"]