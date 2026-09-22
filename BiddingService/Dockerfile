FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
EXPOSE 80

# copy all .csproj files and restore as distinct layers.Use of the same COPY command
# for every dockerfile in the project to take advantage of docker caching
COPY Carsties.sln .
COPY AuctionService/AuctionService.csproj AuctionService/
COPY SearchService/SearchService.csproj SearchService/
COPY GateWayService/GateWayService.csproj GateWayService/
COPY BiddingService/BiddingService.csproj BiddingService/
COPY NotificationService/NotificationService.csproj NotificationService/
COPY Contracts/Contracts.csproj Contracts/
COPY src/IdentityService/IdentityService.csproj src/IdentityService/

# Restore package deps
RUN dotnet restore Carsties.sln

# Copy the app folders over
COPY BiddingService BiddingService
COPY Contracts Contracts


WORKDIR BiddingService
RUN dotnet publish -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "BiddingService.dll"]
