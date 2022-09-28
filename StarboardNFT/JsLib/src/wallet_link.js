import WalletLink from "walletlink"
import Web3 from "web3"

export const walletLink = new WalletLink({
    appName: "My Awesome DApp",
    appLogoUrl: "https://example.com/logo.png",
    darkMode: false
})

export const ethereum = walletLink.makeWeb3Provider(
    "https://mainnet.infura.io/v3/a58ceb01027546d49881cfbbbf9898ee", 1
)

export const web3 = new Web3(ethereum)