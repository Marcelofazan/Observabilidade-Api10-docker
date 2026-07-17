## 📈 Observabilidade-Api10-MongoDB
Exemplo de API observalidade e monitoramento com Grafana, Jaeger, OpenSearch e Prometheus em C# ASP.NET Core 10 com banco de dados MongoDB.

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
- Informar **http://localhost:9090/** ou **http://host.docker.internal:9090** na conexão e Salvar.

- **Passo 2** - Acessar link Grafana para baixar modelo de Dashboard **https://grafana.com/grafana/dashboards/**
- Buscar a opção: **Prometheus 2.0 Overview** e Copiar ID do Dashboard **3662**
- Clicar no menu em Dashboard , clicar em New -> escolher a segunda opção Importar Dashboard e informar o ID **3662** , selecionar em DS_THEMIS **prometheus** e clique em Import.

- **Passo 3** - No canto Superior direito do dashboard, tem que estar com **Edit** ativado, clique no ícone de Engrenagem (Dashboard Settings) -> e clique **Variables All Settings** e depois vá na aba **Variables**.
- Edite as Variables da seguinte forma:

- 1ª Variável: job
```bash
Data source: Selecione prometheus.
Label values: Digite job
Alterar para **Classic Query** e Digite exatamente isto: label_values(up, job)
Apague o campo Regex 
Refresh: Mude para On time range change
```

- 2ª Variável: instance
```bash
Data source: Selecione prometheus.
Label values: Digite instance
Alterar para **Classic Query** e Digite exatamente isto: label_values(up{job=~"$job"}, instance)
Refresh: Mude para On time range change
```

- 3ª Configurar  Job e Instance e alterar filtro de Tempo
- Selecione o projeto nos campos Instance e Job 
- No canto superior do Grafana clique em cima e altere de Last 1 hour para Last 5 minutes. 
- Clique em cima dos Segundos e altere o Refresh para 5 segundos. Isso força o Grafana a buscar os dados em tempo real que estão gerados agora.


- **Passo 4** - Configurar Gráfico **Series Count**
Vá no painel na Seção **series** em **Series Count**, clique nos três pontinhos dele e selecione Edit.
Na query do gráfico, você provavelmente verá algo como **up{job=~"$job", instance=~"$instance"}** alterar para : 
```bash
rate(http_server_request_duration_seconds_count{job=~"$job"}[1m])
```


