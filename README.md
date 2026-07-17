## 📈 Observabilidade-Api10-docker
Exemplo de API Observalidade com Grafana, Jaeger e Prometheus em C# ASP.NET Core 10 com banco de dados MongoDB.

#### 🎨 Aqui está uma demonstração do projeto
<img width="700" height="350" alt="Dashboard_grafana" src="https://github.com/user-attachments/assets/cd94008a-be41-4e7a-856c-34ee8b29f40a" />

#### 📋 O que voçê vai ver nesse Projeto
| Tecnologia | Descrição |
|-----------|-----------|
| **Grafana**  | Plataforma para a observabilidade, monitoramento e visualização de dados gráficos. |
| **Jaeger**  | Ferramenta para rastreamento distribuído de coleta metricas para analisa traces. |
| **Prometheus**  | Ferramentas utilizada para monitoramento e observabilidade de sistemas e infraestruturas em tempo real. |

#### 💬 Requisitos do Projeto
- Necessário **Docker** instalado.

#### 🔄 Executar a aplicação 
- Ao iniciar a aplicação guarde o numero da porta **https://localhost:XXXXX/swagger/index.html**
- Adicione **/swagger/index.html** no link, após localhost para abrir a API. 

| Tecnologia | Descrição | Host |
|-----------|-----------|-----------|
| **Grafana**  | Observabilidade/Logs	| http://localhost:3000/ |
| **Jaeger**  | Rastreamento distribuído | http://localhost:16686/ |
| **Prometheus**  | Monitoramento Metricas | http://localhost:9090/ |		  


#### 🌐 Prometheus Interface Web (UI)
- Clicar na Barra em Status -> Selecionar Targets health, para verificar se o serviço está rodando sem erros.
- Para pesquisar **LOGS da API** de acesse métricas em Endpoint para abrir no navegador **https://localhost:XXXXX/metrics**, com a mesma porta que abriu no navegador, pois está tudo centralizado no Docker.
- Procure metricas por exemplo que começam com **http_server** , ou que terminam com **sum** e **count**
```bash
http_server_request_duration_seconds_count
```

- Para visualizar na própria UI utilizar no topo Query e digite o filtro na busca, para trazer resultados de requisições:
```bash
http_server_request_duration_seconds_count{http_response_status_code="200"}
```

#### 🌐 Grafana Interface Web (UI)
- Abra o seu navegador de internet e acesse: **http://localhost:3000**
- Para acessar Faça login com as credenciais padrão do Grafana: 
```bash
Usuário: admin Senha: admin
```
#### ⚙️ Configurações Grafana Dashboard

- **Passo 1** - Criar novo Data Source 
- Selecione **prometheus** na busca e Informar em Connection **http://host.docker.internal:9090** e clique em Salvar.

- **Passo 2** - Modelo de Gráfico Prometheus
- **Passo 2.1** - Acessar link Grafana para baixar modelo de Dashboard **https://grafana.com/grafana/dashboards/**, buscar a opção: **Prometheus 2.0 Overview** e Copiar ID do Dashboard **3662**
- **Passo 2.2** - Clicar no menu em Dashboard , clicar em **New** e escolher a segunda opção **Importar Dashboard**, informe o ID **3662** e clique em **Load**, selecione em DS_THEMIS **prometheus** e clique em Import.

- **Passo 3** - Editar Váriaveis Padrão
- No canto Superior direito do Dashboard, tem que clicar em **Edit** para aprecer as opções de **Editar**, clique no ícone de Engrenagem (Dashboard Options) -> e clique **View All Settings** em Settings escolha -> Aba **Variables**.

- **Passo 3.1**  Variável: **job** edite da seguinte forma: 
```text
Label values: Digite job
Data source: Selecione prometheus.
Alterar Query para Classic Query e Digite exatamente isto: label_values(up, job)
Remova a String do campo Regex 
Refresh: opção On time range change
Save & Back to Dashboard
```

- **Passo 3.2** Variável: **instance** edite da seguinte forma: 
```text
Label values: Digite instance
Data source: Selecione prometheus.
Alterar Query para Classic Query e Digite exatamente isto: label_values(up{job=~"$job"}, instance)
Remova a String do campo Regex 
Refresh: opção On time range change
Save & Back to Dashboard
```

- **Passo 3.3** Configurar Painel Principal Grafana **Browser Time**, **Job** e **Instance** edite da seguinte forma: 
```text
No topo do Grafana selecionar em Browser Time de Last 1 hour altere para Last 5 minutes.
No topo do Grafana selecionar em Auto Refresh Interval 5 segundos. 
No topo do Grafana selecione em Job dashboardtelemetria
No topo do Grafana selecione em Instance dashboardtelemetria:8080
Save
```

- **Passo 4** - Configurar os Gráficos
-No painel na Seção **series** no quadro do gráfico **Series Count**, clique nos três pontinhos e selecione Edit.
- **Passo 4.1** - Na query do gráfico, altere o conteudo de **Metrics browser** de **up{job=~"$job", instance=~"$instance"}** para :
- Crie e Salve no primeiro gráfico em **series**  
```bash
rate(http_server_request_duration_seconds_count{job=~"$job"}[1m])
```
- Crie e Salve no segundo gráfico em **series** 
```bash
rate(http_server_request_duration_seconds_sum{job=~"$job"}[1m])
```
