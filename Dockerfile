# Use a imagem oficial do .NET SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Define o diretório de trabalho
WORKDIR /app

# Copia os arquivos de projeto (.csproj e .sln)
COPY . .

# Restaura as dependências
RUN dotnet restore

# Copia todo o código fonte
COPY . ./

# Compila a aplicação
RUN dotnet publish AuthService/AuthService.csproj -c Release -o out

# Use a imagem runtime para execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Define o diretório de trabalho
WORKDIR /app

# Copia os arquivos compilados do estágio anterior
COPY --from=build /app/out ./

# Expõe a porta que a aplicação irá usar
EXPOSE 80
EXPOSE 443

# Define o ponto de entrada da aplicação
ENTRYPOINT ["dotnet", "AuthService.dll"]