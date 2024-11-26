FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
EXPOSE 8080
EXPOSE 8081

WORKDIR /src

COPY ConsolaBlazor.csproj .
RUN dotnet restore ConsolaBlazor.csproj


COPY . .
RUN dotnet build ConsolaBlazor.csproj -c Release -o /app/build


FROM build AS publish
RUN dotnet publish ConsolaBlazor.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /user/share/nginx/html

# Instalar nano
RUN apk update && apk add nano

COPY --from=publish /app/publish/wwwroot .
COPY  nginx.conf /etc/nginx/nginx.conf