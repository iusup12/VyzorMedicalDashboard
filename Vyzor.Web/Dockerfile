FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore Vyzor.Web/Vyzor.Web.csproj

RUN dotnet publish Vyzor.Web/Vyzor.Web.csproj 
    -c Release 
    -o /app/publish 
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Vyzor.Web.dll"]
