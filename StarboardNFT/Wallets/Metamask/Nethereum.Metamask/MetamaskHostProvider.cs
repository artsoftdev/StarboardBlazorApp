using System;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.UI;

namespace Nethereum.Metamask
{
    public class MetamaskHostProvider: IEthereumHostProvider
    {
        private readonly IMetamaskInterop _metamaskInterop;
        public static MetamaskHostProvider Current { get; private set; }
        public string Name { get; } = "Metamask";
        public bool Available { get; private set; }
        public string SelectedAccount { get; private set; }
        public int SelectedNetwork { get; }
        public bool Enabled { get; private set; }

        private MetamaskInterceptor _metamaskInterceptor;

        public event Func<string, Task> SelectedAccountChanged;
        public event Func<int, Task> NetworkChanged;
        public event Func<bool, Task> AvailabilityChanged;
        public event Func<bool, Task> EnabledChanged;
        public async Task<bool> CheckProviderAvailabilityAsync()
        {
            var result = await _metamaskInterop.CheckMetamaskAvailability();
            await ChangeMetamaskAvailableAsync(result);
            return result;
        }

        // The balance function message definition    
        [Function("balanceOf", "uint256")]
        public class BalanceOfFunction : FunctionMessage
        {
            [Parameter("address", "owner", 1)]
            public string Owner { get; set; }
        }

        // The owner function message definition
        [Function("ownerOf", "address")]
        public class OwnerOfFunction : FunctionMessage
        {
            [Parameter("uint256", "tokenId", 1)]
            public BigInteger TokenId { get; set; }
        }

        public async Task<decimal> GetAccountBalance(string SelectedAccount)
        {
            var web3 = new Nethereum.Web3.Web3("https://mainnet.infura.io/v3/a58ceb01027546d49881cfbbbf9898ee");
            var balance = await web3.Eth.GetBalance.SendRequestAsync(SelectedAccount);
            Console.WriteLine($"Balance in Wei: {balance.Value}");

            var etherAmount = Nethereum.Web3.Web3.Convert.FromWei(balance.Value);
            Console.WriteLine($"Balance in Ether: {etherAmount}");

            return etherAmount;
        }

        public async Task<int> GetERC721Balance(string erc721TokenAddress, string SelectedAccount)
        {
            var web3 = new Nethereum.Web3.Web3("https://mainnet.infura.io/v3/a58ceb01027546d49881cfbbbf9898ee");

            // The ERC721 contract we will be querying
            var erc721TokenContractAddress = erc721TokenAddress;

            // Example 1. Get balance of an account. This is the count of tokens that an account 
            // has from a specified contract (in this case "Gods Unchained").  
            var accountWithSomeTokens = SelectedAccount;
            // You can check the token balance of the above account on etherscan:
            // https://etherscan.io/token/0x6ebeaf8e8e946f0716e6533a6f2cefc83f60e8ab?a=0x5a4d185c590c5815a070ed62c278e665d137a0d9#inventory

            // Send query to contract, to find out how many tokens the owner has
            var balanceOfMessage = new BalanceOfFunction() { Owner = accountWithSomeTokens };
            var queryHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
            var erc721balance = await queryHandler
                .QueryAsync<BigInteger>(erc721TokenContractAddress, balanceOfMessage)
                .ConfigureAwait(false);

            return (int)erc721balance;
        }

        public async Task<decimal> GetTransactions(string SelectedAccount)
        {
            var web3 = new Nethereum.Web3.Web3("https://mainnet.infura.io/v3/a58ceb01027546d49881cfbbbf9898ee");

            var myAddr = "0xbb9bc244d798123fde783fcc1c72d3bb8c189413";
            var blockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            //Getting current block with transactions 

            for (var i = blockNumber.Value; i > 0; --i)
            {
                var block1 = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(i));
                if (block1.Transactions.Length != 0)
                {
                    foreach (var e in block1.Transactions)
                    {
                        // body of foreach loop
                        if (myAddr == e.From)
                        {
                            if (e.From != e.To)
                                Console.WriteLine($"Balance in Wei: From");
                        }
                        if (myAddr == e.To)
                        {
                            if (e.From != e.To)
                                Console.WriteLine($"Balance in Wei: To");
                        }
                    }
                }
            }

            var balance = await web3.Eth.GetBalance.SendRequestAsync(SelectedAccount);
            Console.WriteLine($"Balance in Wei: {balance.Value}");

            var etherAmount = Nethereum.Web3.Web3.Convert.FromWei(balance.Value);
            Console.WriteLine($"Balance in Ether: {etherAmount}");

            return etherAmount;
        }

        public Task<Web3.Web3> GetWeb3Async()
        {
            var web3 = new Nethereum.Web3.Web3 {Client = {OverridingRequestInterceptor = _metamaskInterceptor}};
            return Task.FromResult(web3);
        }

        public async Task<string> EnableProviderAsync()
        {
            var selectedAccount = await _metamaskInterop.EnableEthereumAsync();
            Enabled = !string.IsNullOrEmpty(selectedAccount);

            if (Enabled)
            {
                SelectedAccount = selectedAccount;
                if (SelectedAccountChanged != null)
                {
                    await SelectedAccountChanged.Invoke(selectedAccount);
                }
                return selectedAccount;
            }

            return null;
        }

        public async Task<string> GetProviderSelectedAccountAsync()
        {
            var result = await _metamaskInterop.GetSelectedAddress();
            await ChangeSelectedAccountAsync(result);
            return result;
        }

        public Task<int> GetProviderSelectedNetworkAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string> SignMessageAsync(string message)
        {
            return await _metamaskInterop.SignAsync(message.ToHexUTF8());
        }

        public MetamaskHostProvider(IMetamaskInterop metamaskInterop)
        {
            _metamaskInterop = metamaskInterop;
            _metamaskInterceptor = new MetamaskInterceptor(_metamaskInterop, this);
            Current = this;
        }
        
        public async Task ChangeSelectedAccountAsync(string selectedAccount)
        {
            if (SelectedAccount != selectedAccount)
            {
                SelectedAccount = selectedAccount;
                if (SelectedAccountChanged != null)
                {
                    await SelectedAccountChanged.Invoke(selectedAccount);
                }
            }
        }

        public async Task ChangeMetamaskAvailableAsync(bool available)
        {
            Available = available;
            if (AvailabilityChanged != null)
            {
                await AvailabilityChanged.Invoke(available);
            }
        }

    }
}
