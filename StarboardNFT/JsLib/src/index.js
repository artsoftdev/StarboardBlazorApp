import { ethereum } from './wallet_link';
import { walletLinkWeb3 } from './wallet_link'
import { bitski, bitski_web3, authentication_status } from './bitski_wallet';
import { fm, fortmatic_web3 } from './fortmatic';
import { Bitski, AuthenticationStatus } from 'bitski';
import Fortmatic from 'fortmatic';
import WalletConnectProvider from "@walletconnect/web3-provider";
import Web3 from 'web3';
import { Starboard721Abi } from './abis';

var web3 = new Web3(Web3.givenProvider);

const contractAddr = '0xEd2A52d61e745aa54e739BC6b6AcFC22e68C95C7';
const Starboard721NFTContract = new web3.eth.Contract(Starboard721Abi, contractAddr);

export function GetCurrentTime() {
    return getCurrentTime();
}

export function Walletlink_Request()
{
    const walletLink = new WalletLink({
        appName: "My Awesome DApp",
        appLogoUrl: "https://example.com/logo.png",
        darkMode: false
    })

    const provider = walletLink.makeWeb3Provider(
        "https://mainnet.infura.io/v3/a58ceb01027546d49881cfbbbf9898ee", 1
    )

    web3 = new Web3(provider);

    // Use eth_RequestAccoun
    return provider.send('eth_requestAccounts').then((accounts) => {
        console.log(`User's address is ${accounts[0]}`);
        // Optionally, have the default account set for web3.
        //web3.eth.defaultAccount = accounts[0];
        return accounts[0];
    }).catch((err) => { // if user closes the modal without logging in
        console.log(err); // { message: 'Fortmatic: User denied account access.', code: '4001' }
    });
}
    
export async function BitskiSignIn() {

    //await bitski.signIn();

    const bitski = new Bitski('69917632-2aa8-4554-8b2b-58e97eb9ab71', 'https://nft.starboard.org/bitski_callback.html');

    await bitski.signIn();
    const provider = bitski.getProvider();

    web3 = new Web3(provider);

    // now you can get accounts
    const accounts = await web3.eth.getAccounts();
    console.log(accounts[0]);

    return accounts[0];
}

export function BitskiCallback() {
    bitski.callback();
}

export function BitskiSignOut() {
    bitski.signOut().then(() => {
        //signed out!
        console.log('sign out');
    });
}

//continueToApp();
//export function BitskiCallback() {
//    // call signInRedirect from some click handler
//    bitski.signInRedirect('https://3b093327cd61.ngrok.io/');
//}

export async function handleLoginWithMagicLink() {
    var real;
    try {
        let fm = new Fortmatic('pk_test_D4DB4846C58F54B7', 'kovan');
        const provider = fm.getProvider();

        web3 = new Web3(provider);

        const accounts = await web3.eth.getAccounts();
        real = accounts[0];
    }
    catch (err) { // if user closes the modal without logging in
        console.log(err); // { message: 'Fortmatic: User denied account access.', code: '4001' }
        return null;
    }
    //return fm.configure({ primaryLoginOption: 'email' }).then(() => {
    //    return fm.user.login().then(() => {
    //        return fortmatic_web3.eth.getAccounts().then((address) => {
    //            console.log(address);
    //            return address[0];
    //        }); // ['0x...']
    //    }).catch((err) => {
    //        console.log(err);
    //    });
    //});
    return real;
}

export function handleLogoutWithFortmatic() {
    fm.user.logout();
}

export async function SignInWalletConnect(Num, TokenURI) {
    console.log('wallet ---- connect');
    var selectedAccount = "";
    try {
        const provider = new WalletConnectProvider({
            infuraId: 'c4f79cc821944d9680842e34466bfbd',
        });

        await provider.enable();

        web3 = new Web3(provider);
        var accounts = await web3.eth.getAccounts();

        selectedAccount = accounts[0];

        return selectedAccount;
    } catch (error) {
        console.log(error);
    }

    return selectedAccount;
}

export function MintStarboard721(web3, Num, TokenURI, selectedAccount) {

    if (Starboard721NFTContract == null) Starboard721NFTContract = new web3.eth.Contract(Starboard721Abi, contractAddr);

    const result = Starboard721NFTContract.methods.mintStarboard721(Num, TokenURI).send({
        from: selectedAccount,
        to: contractAddr,
        gas: 400000
    })

    console.log(result);
}

//export function Eth_requestAccount() {
//    ethereum.send('eth_requestAccounts').then((accounts) => {
//        console.log(`User's address is ${accounts[0]}`);

//        // Optionally, have the default account set for web3.js
//        web3.eth.defaultAccount = accounts[0];
//    });
//}