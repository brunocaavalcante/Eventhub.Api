## Compose
docker-compose -f compose.debug.yaml up


## Migrations
cd Eventhub.Infra
dotnet ef migrations add InitialCreate
dotnet ef database update

Se for a primeira vez, instale a ferramenta global do EF Core:
dotnet tool install --global dotnet-ef