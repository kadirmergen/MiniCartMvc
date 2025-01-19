FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Proje dosyalarýný kopyala ve restore et
COPY . .
RUN dotnet restore

# Projeyi yayýnla
RUN dotnet publish MiniCartMvc.csproj -c Release -o /app

# Gerekli dizinleri kopyalamaya gerek yok, çünkü tüm dosyalar zaten proje dizininde
ENV ConnectionStrings__ApplicationDatabase="Server=host.docker.internal;Database=MiniCartMvc;User Id=kadirmergen;Password=YourPassword123.;Encrypt=False;MultipleActiveResultSets=true"

# Uygulamayý çalýþtýr
ENTRYPOINT ["dotnet", "MiniCartMvc.dll"]
