import Fortmatic from 'fortmatic';
// Works for web3 1.0 and pre-1.0 versions
import Web3 from 'web3';

export const fm = new Fortmatic('pk_live_C13CB1B1BC1480F2');
window.web3 = new Web3(fm.getProvider());

export const fortmatic_web3 = web3;
// Send transactions the way your are used to
//web3.eth.sendTransaction({/* ... */ });