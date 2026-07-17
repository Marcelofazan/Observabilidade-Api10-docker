## 📈 Observabilidade-Api10-MongoDB
Exemplo de API observalidade e monitoramento com Grafana, Jaeger, OpenSearch e Prometheus em C# ASP.NET Core 10 com banco de dados MongoDB.

#### 🎨 Aqui está uma demonstração do projeto
<img width="700" height="350" alt="Dashboard_grafana" src="https://github.com/user-attachments/assets/cd94008a-be41-4e7a-856c-34ee8b29f40a" />

#### 📋 O que voçê vai ver nesse Projeto
| Tecnologia | Descrição |
|-----------|-----------|
| **Grafana**  |É o painel centralizador. Ele possui um plugin para OpenSearch que permite consultar e visualizar logs diretamente |
| **Jaeger**  |Coleta e analisa traces (rastreamento de requisições) para diagnosticar gargalos. Ele também pode enviar métricas para o Prometheus. |
| **Prometheus**  | Coleta e armazena métricas em tempo real. O Grafana se conecta à sua API/endpoint para exibi-las em dashboards. |

#### 💬 Requisitos do Projeto
- Necessário **Docker** instalado.

#### 🔄 Executar a aplicação 

- Inicie a aplicação e guarde o numero da porta **[https://localhost:XXXXX/swagger/index.html](https://localhost:XXXXX/swagger/index.html)** ou adicione **/swagger/index.html** no do link localhost para abrir a API. 

| Tecnologia | Descrição | Host |
|-----------|-----------|-----------|
| **Grafana**  | Observabilidade/Logs	| http://localhost:3000/ |
| **Jaeger**  | Rastreamento distribuído | http://localhost:16686/ |
| **Prometheus**  | Monitoramento Metricas | http://localhost:9090/ |		  


#### 🌐 Prometheus 
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

#### 🌐 Grafana 
- Abra o seu navegador de internet e acesse: **http://localhost:3000**
- Para acessar Faça login com as credenciais padrão do Grafana: 
```bash
Usuário: admin Senha: admin
```

- **Passo 1** - Criar novo Data Source 
- Selecione **prometheus** na busca e Informar em Connection **http://host.docker.internal:9090** e clique em Salvar.

- **Passo 2** - Modelo de Gráfico Prometheus

 - **Passo 2.1** - Acessar link Grafana para baixar modelo de Dashboard **https://grafana.com/grafana/dashboards/**, buscar a opção: **Prometheus 2.0 Overview** e Copiar ID do Dashboard **3662**
 - **Passo 2.2** - Clicar no menu em Dashboard , clicar em **New** e escolher a segunda opção **Importar Dashboard**, informe o ID **3662** e clique em **Load**, selecione em DS_THEMIS **prometheus** e clique em Import.

#### ⚙️ Configurações Grafana Dashboard
- **Passo 3** - No canto Superior direito do Dashboard, tem que clicar em **Edit** para aprecer as opções de **Editar**, clique no ícone de Engrenagem (Dashboard Options) -> e clique **View All Settings** em Settings escolha -> Aba **Variables**.
- Edite as Variables da seguinte forma:

- **Passo 3.1**  Variável: job
```bash
Label values: Digite job
Data source: Selecione prometheus.
Alterar Query para **Classic Query** e Digite exatamente isto: label_values(up, job)
Remova a String do campo Regex 
Refresh: opção On time range change
Save & Back to Dashboard
```

- **Passo 3.2** Variável: instance
```bash
Label values: Digite instance
Data source: Selecione prometheus.
Alterar Query para **Classic Query** e Digite exatamente isto: label_values(up{job=~"$job"}, instance)
Remova a String do campo Regex 
Refresh: opção On time range change
Save & Back to Dashboard
```

- **Passo 3.3** Configurar Painel Browser Time, Job e Instance
```bash
No topo do Grafana selecionar em Browser Time de Last 1 hour altere para Last 5 minutes.
No topo do Grafana selecionar em Auto Refresh Interval 5 segundos. 
No topo do Grafana selecione em Job **dashboardtelemetria**
No topo do Grafana selecione em Instance **dashboardtelemetria:8080**
Save
```

- **Passo 4** - Configurar Gráfico **Series Count**
- No painel na Seção **series** no quadro do gráfico **Series Count**, clique nos três pontinhos e selecione Edit.
- Na query do gráfico, altere o conteudo de **Metrics browser** de **up{job=~"$job", instance=~"$instance"}** para :
  
```bash
rate(http_server_request_duration_seconds_count{job=~"$job"}[1m])
```
- Salve e crie ao lado outro gráfico em **series** utilize
```bash
rate(http_server_request_duration_seconds_sum{job=~"$job"}[1m])
```
