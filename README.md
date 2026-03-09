# SMV Telecom — Site Institucional

Site institucional da SMV Telecom, empresa especializada em soluções de videoconferência, áudio profissional, telefonia IP e Microsoft Teams.

---

## Tecnologias

- **ASP.NET Core 9** — Razor Pages
- **Tailwind CSS** — via CDN Play (sem build step)
- **Alpine.js** — via CDN (menu mobile, dropdowns)
- **C#** — lógica de serviços e catálogo

---

## Como rodar

**Pré-requisito:** .NET 9 SDK instalado.

```bash
# Na pasta do projeto
dotnet run
```

Acesse: `http://localhost:5195`

---

## Estrutura do projeto

```
SMVTelecom/
├── Pages/
│   ├── Shared/
│   │   └── _Layout.cshtml       # Layout principal (nav, footer, WhatsApp)
│   ├── Index.cshtml             # Home
│   ├── QuemSomos.cshtml         # Página institucional
│   ├── Solucoes/
│   │   ├── Videoconferencia.cshtml
│   │   ├── Audio.cshtml
│   │   ├── Telefones.cshtml
│   │   └── MicrosoftTeams.cshtml
│   ├── Parceiras/
│   │   ├── Logitech.cshtml
│   │   ├── AudioCodes.cshtml
│   │   └── PolyHP.cshtml
│   ├── Produto/
│   │   └── Index.cshtml         # Página de detalhe de produto (slug via rota)
│   └── Admin/
│       ├── Index.cshtml         # Login do admin
│       └── Painel.cshtml        # Painel de configurações
├── Data/
│   ├── ProdutoCatalog.cs        # Catálogo de produtos (estático em C#)
│   ├── site-config.json         # Configurações editáveis (telefone, e-mail)
│   └── users.json               # Credenciais do painel admin
├── Services/
│   ├── ProdutoService.cs        # Busca e filtragem de produtos
│   ├── SiteConfigService.cs     # Lê/salva site-config.json
│   └── AdminAuthService.cs      # Autenticação do admin via sessão
├── wwwroot/
│   ├── images/
│   │   ├── produtos/            # Imagens dos produtos
│   │   └── logos/               # Logos das marcas parceiras
│   ├── css/
│   │   └── site.css             # Estilos customizados (WhatsApp float, etc.)
│   └── favicon.png
└── Program.cs                   # Configuração da aplicação
```

---

## Catálogo de produtos

Os produtos estão definidos em `Data/ProdutoCatalog.cs` como uma lista estática em C#. Cada produto tem:

| Campo | Descrição |
|---|---|
| `Slug` | Identificador único (usado na URL `/Produto/slug`) |
| `Nome` | Nome do produto |
| `Marca` | Logitech, AudioCodes ou Poly / HP |
| `Categoria` | Videoconferência, Áudio ou Telefones |
| `Img` | Nome do arquivo em `wwwroot/images/produtos/` |
| `Descricao` | Texto curto para cards |
| `DescricaoLonga` | Texto completo para página de detalhe |
| `Features` | Lista de características |
| `Relacionados` | Slugs de produtos relacionados |

Para **adicionar/remover produtos**, edite diretamente o arquivo `ProdutoCatalog.cs` e reinicie o servidor.

---

## Painel Admin

Acesse em `/Admin` com as credenciais definidas em `Data/users.json`.

No painel é possível alterar:
- **Telefone** exibido na barra superior do site
- **E-mail** de contato
- **Número do WhatsApp** (campo reservado — o WhatsApp dos botões está fixo no código)

> O telefone da barra superior é dinâmico (admin). O WhatsApp `+55 19 99757-3053` está fixo em todos os botões e links do site.

---

## Marcas parceiras

| Marca | Página |
|---|---|
| Logitech | `/Parceiras/Logitech` |
| AudioCodes | `/Parceiras/AudioCodes` |
| Poly / HP | `/Parceiras/PolyHP` |

As logos estão em `wwwroot/images/logos/` (SVG).

---

## Contato / WhatsApp

- Número fixo: **+55 19 99757-3053**
- Botão flutuante definido em `_Layout.cshtml`
- Animação CSS em `wwwroot/css/site.css`
