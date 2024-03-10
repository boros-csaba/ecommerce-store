FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["elenora/elenora.csproj", ""]
RUN dotnet restore "./elenora.csproj"
COPY /elenora .
WORKDIR "/src/."
RUN apt-get update && apt-get install -y nodejs
RUN dotnet build "elenora.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "elenora.csproj" -c Release -o /app/publish

FROM base AS final
RUN apt-get update \ 
    && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
RUN cd /usr/lib && ln -s libgdiplus.so gdiplus.dll
WORKDIR /app
COPY --from=publish /app/publish .
RUN chmod 755 /app/Rotativa/Linux/wkhtmltopdf
ENTRYPOINT ["dotnet", "elenora.dll"]