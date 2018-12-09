#/bin/bash

rm -rf Database/Meta/Generated
rm -rf Database/Domain/Generated

dotnet restore ../../platform/Repository/Repository.sln
dotnet msbuild ../../platform/Repository/Repository.sln

dotnet restore Repository.sln

cd repository/domain
dotnet ../../../../platform/Repository/Generate/bin/Debug/netcoreapp2.2/Generate.dll repository.csproj ../../../core/repository/templates/meta.cs.stg ../../database/meta/generated
cd ../..

dotnet restore Database.sln
dotnet msbuild Database.sln /target:Clean /verbosity:minimal
dotnet msbuild Database.sln /target:Database\\Generate:Rebuild /p:Configuration="Debug" /verbosity:minimal

dotnet Database/Generate/bin/Debug/netcoreapp2.2/Generate.dll
# dotnet msbuild Database\Resources/Merge.proj /verbosity:minimal

