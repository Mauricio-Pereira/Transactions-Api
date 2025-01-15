FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FastCash-Api.Web/FastCash-Api.Web.csproj", "FastCash-Api.Web/"]
COPY ["FastCash-Api.Infrastructure/FastCash-Api.Infrastructure.csproj", "FastCash-Api.Infrastructure/"]
COPY ["FastCash-Api.Domain/FastCash-Api.Domain.csproj", "FastCash-Api.Domain/"]
COPY ["FastCash-Api.Shared/FastCash-Api.Shared.csproj", "FastCash-Api.Shared/"]
COPY ["FastCash-Api.Application/FastCash-Api.Application.csproj", "FastCash-Api.Application/"]
RUN dotnet restore "FastCash-Api.Web/FastCash-Api.Web.csproj"
COPY . .
WORKDIR "/src/FastCash-Api.Web"
RUN dotnet build "FastCash-Api.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FastCash-Api.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FastCash-Api.Web.dll"]