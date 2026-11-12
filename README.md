# 💈 MizinhaDuCorte — Sistema de Agendamento e Gestão para Barbearia

Aplicação web full-stack desenvolvida para modernizar o atendimento e a gestão administrativa da barbearia **MizinhaDuCorte**. O sistema elimina conflitos de horários (*overbooking*) causados por marcações manuais no WhatsApp, automatiza o catálogo de serviços e oferece um painel administrativo completo com controle financeiro e de agenda.

---

## 🚀 Funcionalidades

### 📱 Área do Cliente (Mobile-First)
* **Agendamento Rápido sem Cadastro:** O cliente escolhe o serviço, a data e o horário disponível em menos de 1 minuto.
* **Trava Anti-Overbooking em Tempo Real:** Consulta assíncrona via **Fetch API** que bloqueia automaticamente horários já agendados ou fechados pelo administrador.
* **Confirmação Integrada ao WhatsApp:** Geração automática de mensagem pré-formatada com os dados do agendamento para envio direto à barbearia.
* **Catálogo Dinâmico & Carrossel:** Exibição de serviços (com preços e duração), produtos disponíveis e galeria de cortes.

### 🛠️ Painel Administrativo
* **Autenticação Segura:** Login integrado ao **Supabase Auth** com gerenciamento de sessão por cookies no ASP.NET Core.
* **Dashboard Financeiro:** Cálculo automático do faturamento concluído e total de clientes atendidos com filtros por período (*Hoje*, *Últimos 7 dias*, *Este Mês*, *Este Ano* e *Todo o Período*).
* **Calendário Mensal Interativo:** Navegação entre meses com sinalização visual por cores (*Aberto*, *Fechado* e *Bloqueio Parcial*) e modal detalhado ao clicar em cada dia.
* **Gestão Flexível de Agenda:** Abertura de dias com definição de horário de início/fim e bloqueio de dias inteiros ou horários específicos.
* **CRUD de Serviços e Produtos:** Cadastro, edição e exclusão de serviços e controle de estoque de produtos.
* **Controle de Atendimentos:** Atualização de status dos clientes agendados (*Pendente*, *Concluído* ou *Faltou*).

### 🎨 Interface e Experiência (UI/UX)
* **Light & Dark Mode:** Alternância entre tema claro e escuro em toda a aplicação (cliente e painel administrativo) com persistência no navegador (`localStorage`).

---

## 💻 Tecnologias Utilizadas

* **Back-end:** C#, ASP.NET Core MVC, Entity Framework Core (`Npgsql.EntityFrameworkCore.PostgreSQL`)
* **Banco de Dados & Autenticação:** Supabase (PostgreSQL + Supabase Auth)
* **Front-end:** Razor Views (`.cshtml`), HTML5, Tailwind CSS, JavaScript (Fetch API)
* **Ícones:** Phosphor Icons

---

## 📂 Estrutura do Projeto

```text
MizinhaDuCorte/
├── Controllers/
│   ├── AdminController.cs       # Autenticação, dashboard, agenda, CRUDs e calendário
│   └── HomeController.cs        # Interface do cliente, horários livres e agendamento
├── Data/
│   └── ApplicationDbContext.cs  # Contexto do Entity Framework Core e mapeamento PostgreSQL
├── Models/                      # Entidades (Agendamento, Servico, Produto, DiaAberto, Bloqueio)
├── Views/
│   ├── Admin/                   # Painel administrativo e tela de login
│   ├── Home/                    # Página principal de agendamento do cliente
│   └── Shared/                  # Layouts e componentes compartilhados
├── wwwroot/                     # Arquivos estáticos, imagens e scripts
├── appsettings.json             # Configurações da aplicação e conexão
└── Program.cs                   # Configuração de serviços, rotas e middlewares
```

---

## ⚙️ Como Executar Localmente

### Pré-requisitos
* [.NET SDK 8.0 ou superior](https://dotnet.microsoft.com/download) instalado na máquina.
* Conta e projeto ativo no [Supabase](https://supabase.com) com as tabelas da aplicação criadas.

### Passo a passo

1. **Clone o repositório:**
   ```bash
   git clone [https://github.com/kaykyvieira-dev/BarbeariaMizinhaDuCorte.git](https://github.com/kaykyvieira-dev/BarbeariaMizinhaDuCorte.git)
   cd BarbeariaMizinhaDuCorte
   ```

2. **Restaure os pacotes NuGet:**
   O arquivo `.csproj` já contém todas as dependências necessárias (`Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Tools`, etc.), que são baixadas automaticamente:
   ```bash
   dotnet restore
   ```

3. **Configure as credenciais no `appsettings.json`:**
   Substitua os valores genéricos pelas credenciais reais do seu projeto no **Supabase**:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=SEU_HOST_SUPABASE;Database=postgres;Username=postgres;Password=SUA_SENHA;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
     },
     "Supabase": {
       "Url": "https://SEU_PROJETO.supabase.co",
       "AnonKey": "SUA_CHAVE_ANON_SUPABASE"
     }
   }
   ```

4. **Execute a aplicação:**
   Abra a solução no **Visual Studio** e pressione `F5`, ou rode via terminal:
   ```bash
   dotnet run
   ```

---

## 👨‍💻 Autor

Desenvolvido por **Kayky Vieira** — [GitHub (@kaykyvieira-dev)](https://github.com/kaykyvieira-dev)
