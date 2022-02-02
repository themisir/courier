FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["Courier/Courier.csproj", "Courier/"]
COPY ["Courier.PostgresMigrations/Courier.PostgresMigrations.csproj", "Courier.PostgresMigrations/"]
COPY ["Courier.SqliteMigrations/Courier.SqliteMigrations.csproj", "Courier.SqliteMigrations/"]
RUN dotnet restore "Courier/Courier.csproj"
RUN dotnet restore "Courier.PostgresMigrations/Courier.PostgresMigrations.csproj"
RUN dotnet restore "Courier.SqliteMigrations/Courier.SqliteMigrations.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Courier/Courier.csproj" -c Release -o /app/build
RUN dotnet build "Courier.PostgresMigrations/Courier.PostgresMigrations.csproj" -c Release -o /app/build
RUN dotnet build "Courier.SqliteMigrations/Courier.SqliteMigrations.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "Courier/Courier.csproj" -c Release -o /app/publish
RUN cp /app/build/Courier.PostgresMigrations.dll /app/publish
RUN cp /app/build/Courier.SqliteMigrations.dll /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Courier.dll"]
