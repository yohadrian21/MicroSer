#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tes/Tes.csproj", "Tes/"]
RUN dotnet restore "Tes/Tes.csproj"
COPY . .
WORKDIR "/src/Tes"
RUN dotnet build "Tes.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tes.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tes.dll"]