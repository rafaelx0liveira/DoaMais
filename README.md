# DoaMais - Blood Donation Management System

**DoaMais** is a system developed to manage blood donations, stock control, and notifications for hospitals and donors. The solution uses a microservices architecture with RabbitMQ, implements CQRS for commands and queries, and follows best practices such as structured logging with Serilog and storage in Elasticsearch.

## 🛠️ Technologies Used

- **.NET 9** – Main backend
- **CQRS + MediatR** – Separation of commands and queries
- **RabbitMQ** – Messaging for asynchronous events
- **SQL Server** – Primary database
- **Serilog + Elasticsearch** – Structured logging
- **Docker** – Containerization of services
- **JWT Authentication** – Secure authentication
- **Worker Services** – For donor and hospital notifications
- **VaultService** – Secure storage of secrets
- **Swagger** – API documentation

## 🌟 Features

### 👥 Donor Registration
- Register new donors.
- Validation of age, weight, and minimum requirements.
- Restrictions to prevent duplicate registrations.

### 💉 Blood Stock Control
- Monitor quantities by blood type.
- Automatic notification when stock is low.

### 📊 Donation Registration
- Automatically updates stock after donations.
- Enforces time intervals between donations for men (60 days) and women (90 days).

### 🔎 Donor Search
- Donation history for each donor.
- Report of donations in the last 30 days.

## 🔧 Microservices

| Service | Function |
|---------|----------|
| **DoaMais.API** | Main API for registration, queries, and commands |
| **StockService** | Manages and monitors blood stock |
| **LowStockAlertService** | Notifies administrators about low stock |
| **HospitalNotificationService** | Informs hospitals about blood availability |
| **DonorNotificationService** | Notifies donors about new donation opportunities |
| **ReportService** | Generates periodic reports on donations and stock |
| **MessageBus (RabbitMQ)** | Orchestrates events between microservices |

## 🔄 Event-Driven Architecture (EDA)

**DoaMais** follows an **Event-Driven Architecture (EDA)** model, where events are the main communication mechanism between services. This ensures **loose coupling, scalability, and asynchronous processing**.

- The **API publishes events** to **RabbitMQ** (e.g., donation registered, low stock alert).
- **Worker Services consume and process these events**, triggering new actions automatically.
- This enables **asynchronous processing**, making the system more efficient and resilient.

## 🛡️ Business Rules
- **Duplicate donor registration by email is not allowed.**
- **Minors can be registered but cannot donate.**
- **Minimum weight of 50KG required to be eligible as a donor.**
- **Minimum interval between donations:**
  - Men: 60 days  
  - Women: 90 days
- **Allowed blood volume per donation: 420ml – 470ml.**

## 🔍 Key Endpoints

### **Donors**
- `POST /api/donor` – Register donor
- `GET /api/donor/{id}` – Get donor by ID
- `GET /api/donor/getAll` – List all donors

### **Donations**
- `POST /api/donation` – Register new donation
- `GET /api/donation?donorId={id}` – Get last donation from a donor

### **Blood Stock**
- `GET /api/stock` – Get current stock

## 🎯 Next Steps
- [ ] Create a web interface to interact with the API  
- [ ] Improve automated testing
