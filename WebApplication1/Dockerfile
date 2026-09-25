FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["WebApplication1.csproj", "./"]
RUN dotnet restore "WebApplication1.csproj"
COPY . .
RUN dotnet publish "WebApplication1.csproj" \
 -c Release \
 -o /app/publish \
 /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update \
 && apt-get install -y \
 --no-install-recommends \
 curl \
 && rm -rf /var/lib/apt/lists/*
COPY --from=build \
 /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "WebApplication1.dll"]