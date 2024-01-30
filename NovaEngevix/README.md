# NovaEngevix

Projeto para registrar documentos de clientes, upload, delete e edição.


Seguir os passos abaixo para realizar o migration no banco de dados


# Instalar o .NET 6.0
https://dotnet.microsoft.com/pt-br/download/dotnet/6.0

# Instalar dotnet-ef para executar o migration
dotnet tool install --global dotnet-ef --version 6.*

# Executar o comando para gerar o migration
dotnet ef migrations add InitialCreate

# Executar o comando para gerar os dados na base
dotnet ef database update

# Connection String
Alterar a connection string de acordo com a configuração do banco de dados a ser utilizado