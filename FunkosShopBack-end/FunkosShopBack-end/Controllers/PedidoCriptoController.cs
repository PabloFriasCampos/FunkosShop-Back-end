using FunkosShopBack_end.Models;
using FunkosShopBack_end.Models.Entities;
using FunkosShopBack_end.Models.Transaction;
using Microsoft.AspNetCore.Http;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Microsoft.AspNetCore.Mvc;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.TransactionReceipts;
using Nethereum.Web3;
using System.Numerics;
using System.Globalization;
using System.Text.Json;
using Newtonsoft.Json;
using FunkosShopBack_end.Resources;

namespace FunkosShopBack_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoCriptoController : ControllerBase
    {
        private const string OUR_WALLET = "0xb8F1Df8CA072627E02a10879f422bA512C485d0d";
        private const string NETWORK_URL = "https://rpc.sepolia.org";

        private readonly DBContext _dbContext;

        public PedidoCriptoController(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<Pedido> GetPedidos()
        {
            return _dbContext.Pedidos;
        }

        [HttpPost("buy")]
        public async Task<TransactionToSing> BuyAsync([FromBody] JsonElement body)
        {
            ProductoPedido[] productosPedido = JsonConvert.DeserializeObject<ProductoPedido[]>(body.GetProperty("productos").GetRawText());
            decimal totalPedido = 0;
            foreach (ProductoPedido producto in productosPedido)
            {
                producto.PedidoID = _dbContext.Pedidos.Count()+1;
                totalPedido += producto.TotalProductoEUR;
            }
            string cuentaMetaMask = body.GetProperty("cuentaMetaMask").GetString();
            using CoinGeckoApi coinGeckoApi = new CoinGeckoApi();
            decimal ethereumEur = await coinGeckoApi.GetEthereumPriceAsync();
            BigInteger priceWei = Web3.Convert.ToWei(totalPedido / ethereumEur); // Wei

            Web3 web3 = new Web3(NETWORK_URL);

            TransactionToSing transactionToSing = new TransactionToSing()
            {
                From = cuentaMetaMask,
                To = OUR_WALLET,
                Id = _dbContext.Pedidos.Count()+1,
                Value = new HexBigInteger(priceWei).HexValue,
                Gas = new HexBigInteger(300000).HexValue, // Velocidad en en la que se hace la transacción
                GasPrice = (await web3.Eth.GasPrice.SendRequestAsync()).HexValue
            };

            Pedido pedido = new Pedido()
            {
                UsuarioID = 1,
                FechaPedido = DateTime.Now,
                TotalPedidoEUR = totalPedido,
                //TotalPedidoETH = priceWei,
                WalletCliente = transactionToSing.From,
                Value = transactionToSing.Value
            };

            _dbContext.Pedidos.Add(pedido);
            _dbContext.SaveChanges();
            transactionToSing.Id = pedido.PedidoID;

            return transactionToSing;
        }

        [HttpPost("check/{PedidoID}")]
        public async Task<bool> CheckTransactionAsync(int pedidoID, [FromBody] String txHash)
        {
            bool success = false;
            Pedido pedido = _dbContext.Pedidos.Find(pedidoID);
            pedido.HashTransaccion = txHash;

            Web3 web3 = new Web3(NETWORK_URL);
            var receiptPollingService = new TransactionReceiptPollingService(
                web3.TransactionManager, 1000);

            try
            {

                // Esperar a que la transacción se confirme en la cadena de bloques
                var transactionReceipt = await receiptPollingService.PollForReceiptAsync(txHash);

                // Obtener los datos de la transacción
                var transactionEth = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txHash);

                Console.WriteLine(transactionEth.TransactionHash == transactionReceipt.TransactionHash);
                Console.WriteLine(transactionReceipt.Status.Value == 1);
                Console.WriteLine(transactionReceipt.TransactionHash == pedido.HashTransaccion);
                Console.WriteLine(transactionReceipt.From == pedido.WalletCliente);
                Console.WriteLine(transactionReceipt.To.Equals(OUR_WALLET, StringComparison.OrdinalIgnoreCase));

                success = transactionEth.TransactionHash == transactionReceipt.TransactionHash
                    && transactionReceipt.Status.Value == 1 // Transacción realizada con éxito
                    && transactionReceipt.TransactionHash == pedido.HashTransaccion // El hash es el mismo
                    && transactionReceipt.From == pedido.WalletCliente // El dinero viene del cliente
                    && transactionReceipt.To == OUR_WALLET; // El dinero se ingresa en nuestra cuenta
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al esperar la transacción: {ex.Message}");
            }

            pedido.Pagado = success;

               
            return success;
        }

    }
}
