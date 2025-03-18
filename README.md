# DoaMais - Sistema de Gerenciamento de Doação de Sangue

DoaMais é um sistema desenvolvido para gerenciar doações de sangue, controle de estoque e notificações para hospitais e doadores. A solução utiliza arquitetura de microsserviços com RabbitMQ, implementa CQRS para comandos e consultas, e adota boas práticas como logs estruturados com Serilog e armazenamento no Elasticsearch.

## 🛠️ Tecnologias Utilizadas

- **.NET 9** - Backend principal
- **CQRS + MediatR** - Separacão de comandos e consultas
- **RabbitMQ** - Mensageria para eventos assíncronos
- **SQL Server** - Banco de dados principal
- **Serilog + Elasticsearch** - Logging estruturado
- **Docker** - Containeração de serviços
- **JWT Authentication** - Autenticação segura
- **Worker Services** - Para notificação de doadores e hospitais
- **VaultService** - Para armazenamento seguro de segredos
- **Swagger** - Documentação de API

## 🌟 Funcionalidades

### 👥 Cadastro de Doadores
- Registro de novos doadores.
- Validação de idade, peso e requisitos mínimos.
- Restrições para evitar cadastros duplicados.

### 💉 Controle de Estoque de Sangue
- Monitoramento de quantidades por tipo sanguíneo.
- Notificação automática em caso de estoque baixo.

### 📊 Registro de Doações
- Atualização automática do estoque após doações.
- Controle de tempo entre doações para homens (60 dias) e mulheres (90 dias).

### 🔎 Consulta de Doadores
- Histórico de doações de cada doador.
- Relatório de doações nos últimos 30 dias.

## 🔧 Microsserviços

| Serviço | Função |
|----------|----------|
| **DoaMais.API** | API principal para cadastro, consultas e comandos |
| **StockService** | Controle e monitoramento do estoque de sangue |
| **LowStockAlertService** | Notifica administradores sobre estoque baixo |
| **HospitalNotificationService** | Informa hospitais sobre disponibilidade de sangue |
| **DonorNotificationService** | Notifica doadores sobre novas doações |
| **ReportService** | Gera relatórios periódicos sobre doações e estoque |
| **MessageBus (RabbitMQ)** | Orquestra eventos entre microsserviços |

## 🔄 Arquitetura Orientada a Eventos (EDA)

O **DoaMais** segue um modelo de **Arquitetura Orientada a Eventos (EDA - Event-Driven Architecture)**, pois os eventos são o principal mecanismo de comunicação entre os serviços. Isso garante **desacoplamento, escalabilidade e processamento assíncrono**. 

- A API **publica eventos** no **RabbitMQ** (exemplo: doação registrada, alerta de estoque baixo).
- Os **Worker Services** **consomem e processam esses eventos**, gerando novas ações automaticamente.
- Isso permite um **processamento assíncrono**, tornando o sistema mais eficiente e resiliente.

## 🛡️ Regras de Negócio
- **Não permitir cadastro duplicado de doadores pelo e-mail.**
- **Menores de idade podem ser cadastrados, mas não podem doar.**
- **Peso mínimo de 50KG para ser elegível como doador.**
- **Intervalo mínimo entre doações:**
  - Homens: 60 dias
  - Mulheres: 90 dias
- **Volume de sangue permitido por doação: 420ml - 470ml.**

## 🔍 Endpoints Principais

### **Doadores**
- `POST /api/donor` - Cadastrar doador
- `GET /api/donor/{id}` - Consultar doador por ID
- `GET /api/donor/getAll` - Listar todos os doadores

### **Doações**
- `POST /api/donation` - Registrar nova doação
- `GET /api/donation?donorId={id}` - Consultar última doação de um doador

### **Estoque de Sangue**
- `GET /api/stock` - Consultar estoque atual

## 🎯 Próximos Passos
- [ ] Criar interface web para interação com a API
- [ ] Melhorar testes automatizados

---

