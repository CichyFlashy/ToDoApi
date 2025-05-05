FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["ToDoApi/ToDoApi.csproj", "ToDoApi/"]

RUN dotnet restore "ToDoApi/ToDoApi.csproj"

COPY . .
RUN dotnet publish "ToDoApi/ToDoApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "ToDoApi.dll"]