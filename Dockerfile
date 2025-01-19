FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Proje dosyalar�n� kopyala ve restore et
COPY . .
RUN dotnet restore

# Projeyi yay�nla
RUN dotnet publish MiniCartMvc.csproj -c Release -o /app

# Gerekli dizinleri kopyalamaya gerek yok, ��nk� t�m dosyalar zaten proje dizininde
ENV ConnectionStrings__ApplicationDatabase="Server=host.docker.internal;Database=MiniCartMvc;User Id=kadirmergen;Password=YourPassword123.;Encrypt=False;MultipleActiveResultSets=true"

# Uygulamay� �al��t�r
ENTRYPOINT ["dotnet", "MiniCartMvc.dll"]
