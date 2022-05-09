#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["K8sMonitorCore/K8sMonitorCore.csproj", "K8sMonitorCore/"]
COPY ["KubernetesSyncronizer/KubernetesSyncronizer.csproj", "KubernetesSyncronizer/"]
COPY ["EndpointPinger/EndpointPinger.csproj", "EndpointPinger/"]
RUN dotnet restore "K8sMonitorCore/K8sMonitorCore.csproj"
COPY . .
WORKDIR "/src/K8sMonitorCore"
RUN dotnet build "K8sMonitorCore.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "K8sMonitorCore.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "K8sMonitorCore.dll"]