dotnet build
dotnet ef --startup-project ../Mok.WebApp/ database update --context ApplicationDbContext
pause