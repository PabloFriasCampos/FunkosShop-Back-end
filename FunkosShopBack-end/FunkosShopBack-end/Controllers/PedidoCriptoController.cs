﻿using FunkosShopBack_end.Models;
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
using Microsoft.EntityFrameworkCore;

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
            return _dbContext.Pedidos.Include(pedido => pedido.ListaProductosPedido).ThenInclude(producto => producto.Producto);
        }

        [HttpPost("buy")]
        public async Task<TransactionToSing> BuyAsync([FromBody] JsonElement body)
        {
            ICollection<ProductoPedido> productosPedido = JsonConvert.DeserializeObject<ICollection<ProductoPedido>>(body.GetProperty("productos").GetRawText());
            decimal totalPedido = 0;
            foreach (ProductoPedido producto in productosPedido)
            {
                if (producto.Producto.Stock < producto.CantidadProducto)
                {
                    throw new Exception("No hay stock suficiente para el producto " + producto.Producto.NombreProducto);
                }
                totalPedido += producto.TotalProductoEUR;
            }
            string cuentaMetaMask = body.GetProperty("cuentaMetaMask").GetString();
            int id = int.Parse(body.GetProperty("id").GetString());
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
                PedidoID = _dbContext.Pedidos.Count() + 1,
                UsuarioID = id,
                FechaPedido = DateTime.Now,
                //TotalPedidoETH = priceWei,
                WalletCliente = transactionToSing.From,
                Value = transactionToSing.Value
            };

            _dbContext.Pedidos.Add(pedido);
            _dbContext.SaveChanges();
            foreach (ProductoPedido producto in productosPedido)
            {
                producto.PedidoID = pedido.PedidoID;
                pedido.ListaProductosPedido.Add(producto);
                totalPedido += producto.TotalProductoEUR;
            }
            _dbContext.SaveChanges();
            pedido.TotalPedidoEUR = totalPedido;

            transactionToSing.Id = pedido.PedidoID;

            return transactionToSing;
        }

        [HttpPost("check/{PedidoID}")]
        public async Task<bool> CheckTransactionAsync(int pedidoID, [FromBody] JsonElement body)
        {
            bool success = false;
            Pedido pedido = _dbContext.Pedidos.Find(pedidoID);
            string txHash = body.GetProperty("txHash").GetString();
            int id = int.Parse(body.GetProperty("id").GetString());
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
                    && transactionReceipt.To.Equals(OUR_WALLET, StringComparison.OrdinalIgnoreCase); // El dinero se ingresa en nuestra cuenta
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al esperar la transacción: {ex.Message}");
            }

            pedido.Pagado = success;

            if (success)
            {
                Carrito carrito = _dbContext.Carritos.Find(id);

                foreach(ProductoCarrito producto in carrito.ListaProductosCarrito)
                {
                    producto.Producto.Stock -= producto.CantidadProducto;
                }

                carrito.ListaProductosCarrito = [];
                carrito.TotalCarritoEUR = 0;

                _dbContext.SaveChanges();
            }

            return success;
        }

    }
}
