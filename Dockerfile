FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY elenora ./
RUN dotnet restore
RUN apt-get update && apt-get install -y nodejs
RUN dotnet publish -c Release -o publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

RUN apt-get update \ 
    && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
RUN cd /usr/lib && ln -s libgdiplus.so gdiplus.dll
WORKDIR /app
COPY --from=build /app/publish .
RUN chmod 755 /app/Rotativa/Linux/wkhtmltopdf
ENTRYPOINT ["dotnet", "elenora.dll"]