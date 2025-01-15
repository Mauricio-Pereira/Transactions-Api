# Transactions API

Transactions API é uma aplicação web construída com ASP.NET Core, projetada para gerenciar transações financeiras de maneira
eficiente e segura. Ela oferece funcionalidades de criação, leitura, atualização e exclusão (CRUD) de transações, além
de implementar autenticação baseada em API Key para proteger endpoints sensíveis.

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Funcionalidades](#funcionalidades)
3. [Dependências](#tecnologias-utilizadas)
4. [Arquitetura do Projeto](#arquitetura-do-projeto)
5. [Configuração e Instalação](#configuração-e-instalação)
6. [Uso da API](#uso-da-api)
7. [Autenticação](#autenticação)
8. [Endpoints](#endpoints)
9. [Validação](#validação)
10. [Tratamento de Erros](#tratamento-de-erros)
11. [Testes](#testes)
12. [Contribuição](#contribuição)
13. [Licença](#licença)
14. [Contato](#contato)

## Visão Geral

Transactions API é uma aplicação robusta para gerenciamento de transações financeiras, permitindo aos usuários realizar
operações de forma segura e eficiente. Com uma arquitetura bem definida e práticas de desenvolvimento modernas, a API
garante alta performance, segurança e facilidade de manutenção.

## Funcionalidades

- **CRUD de Transações**: Criação, leitura, atualização e exclusão de transações financeiras.
- **Autenticação por API Key**: Proteção de endpoints sensíveis com validação de chave de API.
- **Validação de Dados**: Utilização do FluentValidation para garantir a integridade dos dados.
- **Tratamento Centralizado de Erros**: Middleware personalizado para tratamento e logging de exceções.
- **Mapeamento de Objetos**: Utilização do AutoMapper para mapeamento eficiente entre entidades e DTOs.
- **Testes Automatizados**: Cobertura abrangente com testes de unidade e integração utilizando XUnit e Moq.

## Dependências

Este projeto utiliza várias bibliotecas e pacotes para implementar funcionalidades específicas e realizar testes. Abaixo
está a lista de dependências e suas respectivas versões:

### Dependências do Projeto Principal (Transactions_API)

| Pacote                                      | Versão  | Descrição                                                                |
|---------------------------------------------|---------|--------------------------------------------------------------------------|
| `AutoMapper`                                | 13.0.1  | Mapeamento de objetos para facilitar a conversão entre entidades e DTOs. |
| `FluentValidation`                          | 11.10.0 | Validação de modelos para garantir integridade dos dados.                |
| `FluentValidation.AspNetCore`               | 11.3.0  | Integração do FluentValidation com o ASP.NET Core.                       |
| `Microsoft.EntityFrameworkCore`             | 8.0.10  | ORM para manipulação de dados no banco de dados.                         |
| `Microsoft.EntityFrameworkCore.Design`      | 8.0.10  | Ferramentas de design para Entity Framework, incluindo migrações.        |
| `Microsoft.Extensions.Logging`              | 8.0.1   | API de logging para ASP.NET Core.                                        |
| `Microsoft.Extensions.Logging.Abstractions` | 8.0.2   | Abstrações de logging.                                                   |
| `Newtonsoft.Json`                           | 13.0.3  | Manipulação de JSON para serialização e desserialização.                 |
| `Pomelo.EntityFrameworkCore.MySql`          | 8.0.2   | Provedor MySQL para Entity Framework Core.                               |
| `Swashbuckle.AspNetCore`                    | 6.9.0   | Integração do Swagger para documentação automática da API.               |

### Dependências do Projeto de Testes (Transactions_Api.Tests)

| Pacote                                    | Versão  | Descrição                                                                  |
|-------------------------------------------|---------|----------------------------------------------------------------------------|
| `coverlet.collector`                      | 6.0.0   | Ferramenta de coleta de cobertura de código durante a execução dos testes. |
| `FluentAssertions`                        | 6.12.1  | Extensões para asserções mais legíveis e expressivas em testes.            |
| `Microsoft.AspNetCore.Mvc.Testing`        | 8.0.10  | Ferramentas para testar aplicações ASP.NET Core.                           |
| `Microsoft.Extensions.Configuration.Json` | 8.0.1   | Configuração de arquivos JSON.                                             |
| `Microsoft.NET.Test.Sdk`                  | 17.8.0  | Ferramentas e infraestrutura para executar testes.                         |
| `Moq`                                     | 4.20.72 | Biblioteca de mocking para simulação de dependências em testes.            |
| `Newtonsoft.Json`                         | 13.0.3  | Manipulação de JSON, para testes que exigem serialização JSON.             |
| `System.IO.Pipelines`                     | 8.0.0   | Manipulação eficiente de dados de fluxo para testes avançados.             |
| `xunit`                                   | 2.5.3   | Framework de testes para execução e organização dos testes.                |
| `xunit.runner.visualstudio`               | 2.5.3   | Ferramentas de execução de testes no Visual Studio.                        |

Essas dependências garantem a funcionalidade completa do projeto Transactions API, desde o mapeamento de objetos e validação
de dados até testes automatizados e documentação com Swagger.

## Arquitetura do Projeto

A aplicação segue uma arquitetura limpa, separando responsabilidades em diferentes camadas:

- **Controllers**: Gerenciam as requisições HTTP e retornam respostas apropriadas.
- **Services**: Contêm a lógica de negócio e interagem com os repositórios.
- **Repositories**: Responsáveis pelo acesso aos dados utilizando o Entity Framework Core.
- **Middleware**: Implementa funcionalidades transversais, como autenticação e tratamento de erros.
- **Validators**: Validam os Data Transfer Objects (DTOs) antes de serem processados.
- **Tests**: Inclui testes de unidade e integração para garantir a qualidade do código.

## Configuração e Instalação

### Pré-requisitos

- .NET 8.0 SDK ou superior. Baixe [aqui](https://dotnet.microsoft.com/download/dotnet/8.0)
- IDE: Visual Studio 2022, Visual Studio Code ou outra de sua preferência.

### Passo a Passo

1. **Clone o Repositório**:
   ```bash
   git clone https://github.com/seu-usuario/transactions-api.git
   cd transactions-api
   ```

2. **Configurar Variáveis de Ambiente**:
   Crie um arquivo `.env` na raiz do projeto (se aplicável) e defina as variáveis necessárias, como chaves de API e
   strings de conexão.

3. **Restaurar Dependências**:
   ```bash
   dotnet restore
   ```

4. **Aplicar Migrations e Configurar o Banco de Dados**:
   Caso esteja utilizando um banco de dados real, configure a string de conexão no `appsettings.json` e execute as
   migrations:
   ```bash
   dotnet ef database update
   ```

5. **Executar a Aplicação**:
   ```bash
   dotnet run --project Transactions-API/Transactions-API.csproj
   ```
   A aplicação estará disponível em `https://localhost:7250` ou `http://localhost:5000` por padrão.

## Uso da API

### Autenticação

A API utiliza autenticação baseada em API Key para proteger endpoints sensíveis. A chave de API deve ser fornecida no
cabeçalho da requisição HTTP com o nome `X-API-KEY`.

**Exemplo de Cabeçalho**:

```makefile
X-API-KEY: sua_chave_api_aqui
```

## Endpoints

### Tabela de Endpoints

**Base URL**: `https://localhost:7250` ou `http://localhost:5089`

| Endpoint                 | Método | Autenticação   | Descrição                                     | Exemplo de Sucesso                                                                                        | Exemplo de Erro                                                                  |  
|--------------------------|--------|----------------|-----------------------------------------------|-----------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------|  
| `/api/Transacoes`        | GET    | Requer API Key | Retorna todas as transações.                  | 200 OK - Lista de transações                                                                              | 404 Not Found - `{"message": "Nenhuma transação encontrada"}`                    |  
| `/api/Transacoes/{txid}` | GET    | Requer API Key | Retorna detalhes de uma transação específica. | 200 OK - Detalhes da transação                                                                            | 404 Not Found - `{"message": "Transação não encontrada"}`                        |  
| `/api/Transacoes/paged`  | GET    | Requer API Key | Retorna uma lista paginada de transações.     | 200 OK - Lista paginada de transações                                                                     | 404 Not Found - `{"message": "Nenhuma transação encontrada"}`                    |
| `/api/Transacoes`        | POST   | Requer API Key | Cria uma nova transação.                      | 201 Created - `{"id": 1, "txid": "string", "valor": 100.00, "dataTransacao": "2024-11-05T18:53:07.101Z"}` | 400 Bad Request - `{"message": "O valor da transação deve ser maior que zero."}` |  
| `/api/Transacoes/{txid}` | PUT    | Requer API Key | Atualiza uma transação existente.             | 200 OK - `{"message": "Transação atualizada com sucesso"}`                                                | 404 Not Found - `{"message": "Transação não encontrada"}`                        |  
| `/api/Transacoes/{txid}` | DELETE | Requer API Key | Deleta uma transação existente.               | 200 OK - Detalhes da transação deletada                                                                   | 404 Not Found - `{"message": "Transação não encontrada"}`                        |  

1. **Obter Todas as Transações**
    - **URL**: `/api/Transacoes`
    - **Método**: GET
    - **Autenticação**: Requer API Key
    - **Descrição**: Retorna uma lista das transações existentes.

   **Exemplo de Resposta**: 200 OK
   ```json
   [
      {
      "id": 1,
      "txid": "string",
      "e2eId": "string",
      "pagadorNome": "string",
      "pagadorCpf": "string",
      "pagadorBanco": "string",
      "pagadorAgencia": "string",
      "pagadorConta": "string",
      "recebedorNome": "string",
      "recebedorCpf": "string",
      "recebedorBanco": "string",
      "recebedorAgencia": "string",
      "recebedorConta": "string",
      "valor": 0,
      "dataTransacao": "2024-11-05T18:53:07.101Z"
      },
   {
      "id": 2,
      "txid": "string",
      "e2eId": "string",
      "pagadorNome": "string",
      "pagadorCpf": "string",
      "pagadorBanco": "string",
      "pagadorAgencia": "string",
      "pagadorConta": "string",
      "recebedorNome": "string",
      "recebedorCpf": "string",
      "recebedorBanco": "string",
      "recebedorAgencia": "string",
      "recebedorConta": "string",
      "valor": 0,
      "dataTransacao": "2024-11-05T18:53:07.101Z"
      }
   ]
   ```
    - **Resposta de Erro**: 404 Not Found - `{"message": "Nenhuma transação encontrada"}`


2. **Obter Transação por Txid**
    - **URL**: `/api/Transacoes/{txid}`
    - **Método**: GET
    - **Autenticação**: Requer API Key
    - **Descrição**: Retorna os detalhes de uma transação específica.

   ```json
   {
      "id": 1,
      "txid": "string",
      "e2eId": "string",
      "pagadorNome": "string",
      "pagadorCpf": "string",
      "pagadorBanco": "string",
      "pagadorAgencia": "string",
      "pagadorConta": "string",
      "recebedorNome": "string",
      "recebedorCpf": "string",
      "recebedorBanco": "string",
      "recebedorAgencia": "string",
      "recebedorConta": "string",
      "valor": 0,
      "dataTransacao": "2024-11-05T18:53:07.101Z"
   }
   ```
    - **Resposta de Erro**: 404 Not Found - `{"message": "Transação não encontrada"}`

3. **Obter Trancações Paginadas**
    - **URL**: `/api/Transacoes/paged`
    - **Método**: GET
    - **Autenticação**: Requer API Key
    - **Descrição**: Retorna uma lista paginada de transações.

    **Exemplo de Requisição**:
     ```json
     {
        "pageNumber": 1,
        "pageSize": 10
     }
     ```

   **Exemplo de Resposta**: 200 OK
   ```json
   [
      {
      "id": 1,
      "txid": "string",
      "e2eId": "string",
      "pagadorNome": "string",
      "pagadorCpf": "string",
      "pagadorBanco": "string",
      "pagadorAgencia": "string",
      "pagadorConta": "string",
      "recebedorNome": "string",
      "recebedorCpf": "string",
      "recebedorBanco": "string",
      "recebedorAgencia": "string",
      "recebedorConta": "string",
      "valor": 0,
      "dataTransacao": "2024-11-05T18:53:07.101Z"
      },
   {
      "id": 2,
      "txid": "string",
      "e2eId": "string",
      "pagadorNome": "string",
      "pagadorCpf": "string",
      "pagadorBanco": "string",
      "pagadorAgencia": "string",
      "pagadorConta": "string",
      "recebedorNome": "string",
      "recebedorCpf": "string",
      "recebedorBanco": "string",
      "recebedorAgencia": "string",
      "recebedorConta": "string",
      "valor": 0,
      "dataTransacao": "2024-11-05T18:53:07.101Z"
      }
   ]
   ```
    - **Resposta de Erro**: 404 Not Found - `{"message": "Nenhuma transação encontrada"}`


3. **Criar Transação**

    - **URL**: `/api/Transacoes`
    - **Método**: POST
    - **Autenticação**: Requer API Key
    - **Descrição**: Cria uma nova transação com o valor fornecido.

   **Exemplo de Requisição**:
    ```json
    {
      "valor": 100.00
    }
    ```
   **Exemplo de Resposta**:
   ```json
   {
      "id": 1,
      "txid": "string",
      "valor": 100.00,
      "dataTransacao": "2024-11-05T18:53:07.101Z"
   }
   ```
    - **Resposta de Erro**: 400 Bad Request - `{"message": "O objeto TransacaoCreateDTO não pode ser nulo."}`
    - **Resposta de Erro**: 400 Bad Request - `{"message": "O valor da transação deve ser maior que zero."}`
    - **Resposta de Erro**: 500 Internal Server Error - `{"message": "Erro ao criar a transação"}`


4. **Atualizar Transação**
    - **URL**: `/api/Transacoes/{txid}`
    - **Método**: PUT
    - **Autenticação**: Requer API Key
    - **Descrição**: Atualiza os detalhes de uma transação existente.

   **Exemplo de Requisição**:
    ```json
   {
      "txid": TROQUE AQUI PELO ID DA TRANSAÇÃO,  
      "pagadorNome": "João da Silva",  
      "pagadorCpf": "39053344705",  
      "pagadorBanco": "001",  
      "pagadorAgencia": "1234",  
      "pagadorConta": "1234567",  
      "recebedorNome": "Maria Oliveira",  
      "recebedorCpf": "84983149022",  
      "recebedorBanco": "237",  
      "recebedorAgencia": "5678",  
      "recebedorConta": "7654321"  
       }
    ```
    - **Resposta de Sucesso**: 200 OK - `{"message": "Transação atualizada com sucesso"}`
    ``` json
   {
      "id": 7,
      "txid": "T45756448202411051908MoHCZGm26Eq",
      "e2eId": "E45756448202411051910IGIlXzooAkG",
      "pagadorNome": "João da Silva",
      "pagadorCpf": "39053344705",
      "pagadorBanco": "001",
      "pagadorAgencia": "1234",
      "pagadorConta": "1234567",
      "recebedorNome": "Maria Oliveira",
      "recebedorCpf": "84983149022",
      "recebedorBanco": "237",
      "recebedorAgencia": "5678",
      "recebedorConta": "7654321",
      "valor": 800,
      "dataTransacao": "2024-11-05T19:08:51.034742"
   }
   
    ```
    - **Resposta de Erro**: 404 Not Found - `{"message": "Transação não encontrada"}`
    - **Resposta de Erro**: 400 Bad Request - `{"message": "(mensagem de erro baseada na validação)"}`
    - **Resposta de Erro**: 500 Internal Server Error - `{"message": "Erro ao atualizar a transação"}`


5. **Deletar Transação**

    - **URL**: `/api/Transacoes/{txid}`
    - **Método**: DELETE
    - **Autenticação**: Requer API Key
    - **Descrição**: Deleta uma transação existente.
    - **Resposta de Sucesso**: 200 OK
    ```json
    {
   "id": 7,
   "txid": "T45756448202411051908MoHCZGm26Eq",
   "e2eId": "E45756448202411051910IGIlXzooAkG",
   "pagadorNome": "João da Silva",
   "pagadorCpf": "39053344705",
   "pagadorBanco": "001",
   "pagadorAgencia": "1234",
   "pagadorConta": "1234567",
   "recebedorNome": "Maria Oliveira",
   "recebedorCpf": "84983149022",
   "recebedorBanco": "237",
   "recebedorAgencia": "5678",
   "recebedorConta": "7654321",
   "valor": 800,
   "dataTransacao": "2024-11-05T19:08:51.034742"
   }
    ```
    - **Resposta de Erro**: 404 Not Found - `{"message": "Transação não encontrada"}`
    - **Resposta de Erro**: 500 Internal Server Error - `{"message": "Erro ao deletar a transação"}`

## Validação

A aplicação utiliza FluentValidation para garantir a integridade dos dados nas requisições.
As validações são aplicadas nos Data Transfer Objects (DTOs) antes de serem processados e retornam mensagens de erro
claras em caso de falha.
Além disso, as classes de validação dentro do diretório `Validators` são reutilizáveis e fáceis de manter.

## Tratamento de Erros

Middleware personalizado trata exceções e fornece respostas claras.
O middleware de tratamento de erros captura exceções não tratadas e retorna uma resposta JSON com o status de erro
apropriado e uma mensagem descritiva.
Além disso, as exceções são registradas no console para fins de depuração e monitoramento.

## Testes

Foi feita uma aplicação de testes unitários e de integração para garantir a qualidade do código.
Os testes de unidade cobrem a lógica de negócio e a validação de dados, enquanto os testes de integração verificam a
integridade dos endpoints da API.

## Licença

MIT License

## Autor

- [Mauricio Vieira Pereira](https://github.com/Mauricio-Pereira)