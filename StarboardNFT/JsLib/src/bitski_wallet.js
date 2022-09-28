import { Bitski, AuthenticationStatus } from 'bitski';
import Web3 from 'web3';

export const bitski = new Bitski('69917632-2aa8-4554-8b2b-58e97eb9ab71', 'https://nft.starboard.org/bitski_callback.html');
export const authentication_status = AuthenticationStatus;

const provider = bitski.getProvider();
const web3 = new Web3(provider);

export const bitski_web3 = web3;