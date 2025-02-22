# Base stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["UBS.Risk.Evaluator.Batch.csproj", "./"]
RUN dotnet restore "./UBS.Risk.Evaluator.Batch.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./UBS.Risk.Evaluator.Batch.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Configure time_zone
ENV TZ=America/Sao_Paulo
RUN apt-get update && apt-get install -y tzdata
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UBS.Risk.Evaluator.Batch.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Enabling the use of volume
VOLUME ["/app/data"]

# Adding user for security
USER $APP_UID

# Adding graceful shutdown
STOPSIGNAL SIGINT

# Using CMD to run the application directly
CMD ["sh", "-c", "echo 'Starting UBS Risk Evaluator'; dotnet UBS.Risk.Evaluator.Batch.dll"]
