# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the rest of the source code
COPY ./src ./src
COPY ./test ./test

# Publish the application
RUN dotnet publish ./src/aspire92.YarpProxy -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app .

# Copy SSL certificate
COPY ./src/certs/yarp-cert.pfx /https/yarp-cert.pfx

# Set environment variables for Kestrel
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/yarp-cert.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=P@ssw0rd!

# Set the entry point
ENTRYPOINT ["dotnet", "aspire92.YarpProxy.dll"]
