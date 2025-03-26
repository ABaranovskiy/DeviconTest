FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /src
COPY ["WebAPI/WebAPI.csproj", "WebAPI/"]
RUN dotnet restore "WebAPI/WebAPI.csproj"
COPY . .
RUN dotnet publish "WebAPI/WebAPI.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
COPY --from=build-env /app/publish .
ENTRYPOINT ["dotnet", "WebAPI.dll"]