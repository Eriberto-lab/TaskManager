# TaskManager API

Sistema de Gestão de Tarefas desenvolvido em .NET 8 utilizando arquitetura limpa (Clean Architecture), com foco em boas práticas de código, testes automatizados e manutenção.

## Sumário
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Arquitetura](#arquitetura)
- [Funcionalidades](#funcionalidades)
- [Validações Adicionais](#validações-adicionais)
- [Como Executar o Projeto](#como-executar-o-projeto)
- [Testes e Cobertura](#testes-e-cobertura)
- [Exemplos de Requisição e Resposta](#exemplos-de-requisição-e-resposta)
- [Considerações Finais](#considerações-finais)

---

## Tecnologias Utilizadas
- .NET 8
- Entity Framework Core (InMemory)
- Swagger (Swashbuckle)
- xUnit + Moq
- Coverlet + ReportGenerator

---

## Arquitetura
O projeto foi desenvolvido seguindo os princípios da **Clean Architecture**, separando responsabilidades em camadas bem definidas:

```
TaskManager
|
|-- TaskManager.API              // Camada de apresentação (Controllers, Validadores, Program.cs)
|-- TaskManager.Application      // Regras de negócio (Services, DTOs)
|-- TaskManager.Domain           // Entidades e contratos (Entities, Interfaces, Enums)
|-- TaskManager.Infrastructure   // Persistência de dados (DbContext, Repositories)
|-- TaskManager.CrossCutting     // Middleware, Responses e Exceptions customizadas
|-- TaskManager.Tests            // Testes unitários (xUnit + Moq)
```

> **Nota:** Não foi utilizado AutoMapper com o objetivo de manter a clareza na transformação entre entidades e DTOs, evitando abstrações desnecessárias para um projeto de pequeno porte.

---

## Funcionalidades
- Criar tarefa
- Listar tarefas
  - Com filtros opcionais de status e intervalo de datas
- Atualizar tarefa
- Excluir tarefa

### Status permitidos
- `Pending`
- `InProgress`
- `Completed`

---

## Validações Adicionais
- **Data de vencimento não pode ser no passado**
- **Filtro por data de vencimento e intervalo de datas**: adicionados para melhorar a usabilidade e organização das tarefas, mesmo não sendo uma exigência do desafio.

---

## Como Executar o Projeto
1. Clone o repositório:
   ```bash
   git clone https://github.com/Eriberto-lab/TaskManager.git
   ```

2. Acesse a pasta da solução:
   ```bash
   cd TaskManager
   ```

3. Execute a API:
   ```bash
   dotnet run --project TaskManager.API
   ```

4. Acesse a documentação Swagger:
   [http://localhost:7085/swagger](https://localhost:7085/swagger/index.html) *(ajustar porta conforme aplicação)*

---

## Testes e Cobertura
1. Execute os testes:
   ```bash
   dotnet test /p:CollectCoverage=true
   ```

2. Gere o relatório de cobertura:
   ```bash
   reportgenerator -reports:TaskManager.Tests/coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html
   ```

3. Abra o arquivo gerado em:
   ```
   coveragereport/index.html
   ```

> 100% de cobertura na camada `Application`.

---

## Exemplos de Requisição e Resposta

### Criar Tarefa (POST /api/tasks)
**Request:**
```json
{
  "title": "Estudar Clean Architecture",
  "description": "Revisar conceitos de camada e responsabilidades",
  "dueDate": "2025-04-10T00:00:00"
  
}
```
**Response:**
```json
{
  201
}
```

### Listar Tarefas com Filtro (GET /api/tasks?status=Pending&startDate=2025-04-01&endDate=2025-04-10)
**Response:**
```json
[
  {
    "id": 1,
    "title": "Estudar Clean Architecture",
    "description": "Revisar conceitos de camada e responsabilidades",
    "dueDate": "2025-04-10T00:00:00",
    "status": "Pending"
  }
]
```

---

## Considerações Finais
- Projeto estruturado para facilitar a manutenção e extensibilidade.
- Testes com alta cobertura garantem maior confiança em futuras mudanças.
- Middleware genérico para tratamento centralizado de exceções.

---

**Autor:** Eriberto Lima

---

Sinta-se livre para contribuir ou sugerir melhorias ✨

