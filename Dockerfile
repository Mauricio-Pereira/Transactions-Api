FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Transactions-Api.Web/Transactions-Api.Web.csproj", "Transactions-Api.Web/"]
COPY ["Transactions-Api.Infrastructure/Transactions-Api.Infrastructure.csproj", "Transactions-Api.Infrastructure/"]
COPY ["Transactions-Api.Domain/Transactions-Api.Domain.csproj", "Transactions-Api.Domain/"]
COPY ["Transactions-Api.Shared/Transactions-Api.Shared.csproj", "Transactions-Api.Shared/"]
COPY ["Transactions-Api.Application/Transactions-Api.Application.csproj", "Transactions-Api.Application/"]
RUN dotnet restore "Transactions-Api.Web/Transactions-Api.Web.csproj"
COPY . .
WORKDIR "/src/Transactions-Api.Web"
RUN dotnet build "Transactions-Api.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Transactions-Api.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Transactions-Api.Web.dll"]