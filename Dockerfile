FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /api

# Copier le fichier sln et les fichiers csproj d'abord (cache Docker)
COPY *.sln ./
COPY IncidentAPI-ISIMM-MP1-GL/*.csproj IncidentAPI-ISIMM-MP1-GL/
COPY AppTests/*.csproj AppTests/

# Récupérer les dépendances
RUN dotnet restore

# Copier tout le reste
COPY . .

# Publier uniquement l'API (important)
RUN dotnet publish IncidentAPI-ISIMM-MP1-GL/IncidentAPI-ISIMM-MP1-GL.csproj -c Release -o /app/publish

# Préparer l'env. d'exécution (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Forcer l'API dans le conteneur d'être accessible depuis la machine hôte 
# et d'écouter sur le port 80
ENV ASPNETCORE_URLS=http://0.0.0.0:80
EXPOSE 80

# Copier les fichiers publiés de l’application depuis l’étape de build (/app/publish) 
# vers le dossier courant du conteneur afin de les exécuter
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "IncidentAPI-ISIMM-MP1-GL.dll"]