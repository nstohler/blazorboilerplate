dotnet ef database drop -f

rm -rf Migrations/

dotnet ef migrations add init 

dotnet ef database update 
