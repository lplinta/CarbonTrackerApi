# CarbonTrackerApi
# Projeto - Cidades ESGInteligentes

## Como executar localmente com Docker

1. Clone o repositório e entre na pasta do projeto:
```bash
git clone https://github.com/lplinta/CarbonTrackerApi.git
cd CarbonTrackerApi
```

2. Suba os containers (reconstruindo a imagem):
```bash
docker-compose up --build -d
```

3. Acesse a API:
- [Swagger](http://localhost:8080/swagger/index.html)

4. Para parar e remover containers:
```bash
docker-compose down
```

## Pipeline CI/CD

A aplicação utiliza o GitHub Actions para CI/CD. O fluxo principal (.github/workflows/deploy.yml) é acionado automaticamente em todo push para a branch main.

### Fluxo da Pipeline
- Build: Compilação da aplicação .NET 9 e restauração de dependências.
- Gitleaks: Escaneamento do repositório em busca de segredos ou credenciais expostas.
- Testes: Execução de todos os testes unitários.
- Deployment Staging (Automático): Se as etapas anteriores forem bem-sucedidas, a imagem Docker da aplicação é construída, publicada no Google Artifact Registry, e o deploy é realizado automaticamente no serviço carbontrackerapi-staging.
- Deployment Produção (Protegido/Manual): Esta etapa é protegida por um GitHub Environment com aprovação obrigatória. Uma vez aprovado, o deploy é realizado no serviço carbontrackerapi, reutilizando a mesma imagem Docker validada em Staging.

## Containerização

Dockerfile (resumo / conteúdo principal usado na pipeline):

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CarbonTrackerApi/CarbonTrackerApi.csproj", "CarbonTrackerApi/"]

RUN dotnet restore "CarbonTrackerApi/CarbonTrackerApi.csproj"

COPY . .


FROM build AS publish
RUN dotnet publish "CarbonTrackerApi/CarbonTrackerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "CarbonTrackerApi.dll"]
```

Estratégias adotadas:
- Multi-stage build para gerar imagens menores (build → publish → runtime).
- Porta exposta 8080 e variável `ASPNETCORE_URLS` configurada para `http://+:8080`.
- Ambiente de runtime definido via `ASPNETCORE_ENVIRONMENT`.

## Prints do funcionamento

### Pipeline
<img width="1919" height="945" alt="image" src="https://github.com/user-attachments/assets/ad8f7089-56bd-437a-8fb7-207cc632a287" />

### Serviço rodando no Cloud Run
<img width="1919" height="944" alt="image" src="https://github.com/user-attachments/assets/3f45e31b-21d0-4d9a-a0b7-b8f7473a7d99" />

### Funcionamento de chamada rodando local
<img width="862" height="322" alt="image" src="https://github.com/user-attachments/assets/218a154c-ef36-487b-8899-acb77d38ba30" />

### Funcionamento de chamada rodando staging
<img width="861" height="289" alt="image" src="https://github.com/user-attachments/assets/a35a2d51-4f60-4244-93ca-bd3f6b6a6a4f" />

### Funcionamento de chamada rodando prod
<img width="862" height="259" alt="image" src="https://github.com/user-attachments/assets/15dec2d8-3ca2-4050-9207-f1934271053b" />

## Tecnologias utilizadas

- C# / .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- Oracle XE (container para desenvolvimento)
- Docker
- GitHub Actions
- Google Cloud Run
- Google Artifact Registry
- Gitleaks

## Checklist de Entrega

- [x] Projeto compactado em .ZIP com estrutura organizada:
  - Dockerfile
  - docker-compose.yml
  - src/
  - README.md
  - .github/workflows/
  - docs/ (prints e PDF)
- [x] Dockerfile funcional
- [x] docker-compose.yml funcional
- [x] Pipeline com etapas de build, teste e deploy
- [x] README.md com instruções e prints
- [x] Documentação técnica (PDF ou PPT) com evidências
- [x] Deploy realizado em staging e produção
