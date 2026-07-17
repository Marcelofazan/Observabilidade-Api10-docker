using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics;

namespace dashboardTelemetria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutosController : ControllerBase
    {
        private readonly IMongoCollection<Produto> _produtos;

        public ProdutosController(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDb:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDb:Database"]);
            _produtos = database.GetCollection<Produto>("produtos");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lista = await _produtos.Find(_ => true).ToListAsync();
            return Ok(lista);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Produto produto)
        {
            var activity = Activity.Current;
            activity?.SetTag("http.action", "CreateProduct");

            // Garante que o ID comece nulo para o MongoDB gerar o ObjectId automaticamente
            produto.Id = null;

            await _produtos.InsertOneAsync(produto);
            return StatusCode(201, produto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var activity = Activity.Current;
            activity?.SetTag("http.action", "GetProductById");
            activity?.SetTag("product.id", id);

            var produto = await _produtos.Find(p => p.Id == id).FirstOrDefaultAsync();

            if (produto == null)
            {
                activity?.SetTag("http.status_code", 404);
                return NotFound(new { mensagem = "Produto não encontrado." });
            }

            return Ok(produto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] Produto produtoAtualizado)
        {
            var activity = Activity.Current;
            activity?.SetTag("http.action", "UpdateProduct");
            activity?.SetTag("product.id", id);

            produtoAtualizado.Id = id;

            var resultado = await _produtos.ReplaceOneAsync(p => p.Id == id, produtoAtualizado);

            if (resultado.MatchedCount == 0)
            {
                activity?.SetTag("http.status_code", 404);
                return NotFound(new { mensagem = "Produto não encontrado para atualização." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var activity = Activity.Current;
            activity?.SetTag("http.action", "DeleteProduct");
            activity?.SetTag("product.id", id);

            var resultado = await _produtos.DeleteOneAsync(p => p.Id == id);

            if (resultado.DeletedCount == 0)
            {
                activity?.SetTag("http.status_code", 404);
                return NotFound(new { mensagem = "Produto não encontrado para exclusão." });
            }

            return NoContent();
        }
    }

    public class Produto
    {
        // AJUSTE CRÍTICO: Atributos para o MongoDB mapear a string como ObjectId
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Nome { get; set; } = null!;
        public decimal Preco { get; set; }
    }
}