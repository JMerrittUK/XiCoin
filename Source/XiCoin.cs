using System;
using System.IO;

using System.Collections.Generic;

using System.Text;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.WebSockets;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// WELCOME TO Ξ (XI) COIN!
/// 
/// Before delving into the code, I heavily advise you work on looking into cryptography if you haven't already, alongside the basics of blockchain technology.
/// 
/// There are excellent tutorials online that help you take what I've written and create your own custom implementations super easily! I would prefer if you were
/// to make improvements, you should do it on the main branch on GitHub, to help improve the product for everyone.
/// 
/// This work is licensed under a Creative Commons Attribution 4.0 International License: https://creativecommons.org/licenses/by/4.0/
/// Basically you can do what you want with it, so long as you reference the original GitHub page prominently.
/// 
/// Made an improvement or wallet you're particularly proud of? 
/// Drop me an email at work@jmerritt.uk! 
/// </summary>
namespace XiCoin
{
    /// <summary>
    /// Main class, simply acts as a holder for the main Session. This is where any changes should be made to UI level interactions!
    /// </summary>
    class XiCoin
    {
        // Important to note, that just changing this number alone will do nothing. If everything else doesn't match in the blockchain, your blocks WILL be discarded as void data! 
        // The majority of the blockchain MUST move over to the new version. This should NOT change unless significant changes are made that should render it incompatible with old versions.
        const double MajorVersion = 1.0;

        #region Mining settings

        const bool miner = true;
        // A larger nonce offset between threads means more ground for each one to cover, but also increases the chances of you getting a good nonce.
        const int nonceOffset = 100000;
        /// <summary>  </summary>
        const int targetThreadCount = 1;

        #endregion

        static Session xiSession = new Session();

        static void Main(string[] args)
        {
            // We create a new session. We're now ready to start using the currency!

            xiSession.Start();

            // We prep the cancellation tokens so we can close down all async operations correctly
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            CancellationToken globalCancellation = cancellationTokenSource.Token;

            Task.Run(() => xiSession.StartUpdate(1, globalCancellation));

            xiSession.StartMining(targetThreadCount, xiSession);           
        }

        
    }

    /// <summary>
    /// The core element of XiCoin. 
    /// 
    /// This allows people to build off the top of the code base to write their own algorithms and UI setups - we don't want to hamstring people into our ugly UI!
    /// (this also makes it super easy for XiCoin to be included in existing wallets, they just need to communicate with a Session)
    /// </summary>
    class Session
    {
        /// <summary> How long it should take to create a block in seconds </summary>
        const int BLOCK_GENERATION_INTERVAL = 10;
        /// <summary> How many blocks after which the difficulty should adjust </summary>
        const int DIFFICULTY_ADJUSTMENT_INTERVAL = 5;


        /// <summary> A local server, which will let us receive information from other peers </summary>
        TcpListener tcpServer = new TcpListener(5001);

        /// <summary> All blocks should be loaded into memory here </summary>
        public List<Block> blockchain;
        List<Peer> knownPeers = new List<Peer>();

        // This is ALWAYS the first block, and will be the same no matter what. This prevents the injection of fake content.
        Block genesisBlock = new Block()
        {
            blockNumber = 0,
            timestamp = 1533624300,
            difficulty = 1,
            nonce = 990330,
            transactions = new List<Transaction>(),
            wallets = new List<Wallet>()
        };

        public void Start()
        {
            // Calculate the original blockhash
            genesisBlock.previousBlockHash = "This is an old block hash. Doesn't matter much except it's the beginning point!";

            // Add blank wallets / transactions for now. This will be a default wallet in the future.
            genesisBlock.transactions.Add(new Transaction());
            genesisBlock.wallets.Add(new Wallet());

            genesisBlock.blockHash = Validation.CalculateBlockHash(genesisBlock);

            // Start listening for incoming requests!
            tcpServer.Start();

            // Here we can add a selection of new peers. These are hardcoded IP's which should always be up - if not this can be edited, or extras can be added by file in peers.xi (SHA256 encrypted secured by nonce)
            knownPeers.Add(new Peer() { ipAddress = "127.0.0.1" });

            // Now we need to make some external requests, to find out which of our known peers has the latest block.
            foreach (Peer peer in knownPeers)
            {
                Console.WriteLine("Contacting peer '" + peer.ipAddress + "'...");
            }

            // If we can't find any other client, even on the specified backup IP's, we start our own gensis block.

            // WE HIGHLY RECOMMEND YOU SPECIFY A MANUAL KNOWN WORKING IP! 
            // This will help avoid you starting a whole new blockchain which will just get
            // deleted the moment you come into contact with the much bigger main blockchain.
            blockchain = new List<Block>
            {
                genesisBlock
            };
        }

        public async Task StartUpdate(int interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                await UpdateAsync();
                await Task.Delay(interval, cancellationToken);

                // We check to see if we need to cancel this task
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
        }

        async Task UpdateAsync()
        {
            Console.WriteLine("Awaiting connection...");

            // We wait for a peer to connect to us (the client) - if they do we need to figure out why they're connecting
            TcpClient client = await tcpServer.AcceptTcpClientAsync();

            Console.WriteLine("Peer connected to TCP server...");

            NetworkStream clientStream = client.GetStream();

            // Prepare some variables to hold the data we need
            Byte[] buffer = new Byte[4096];
            int i;

            // This will hold the data once it has been converted from bytes.
            string data;

            while ((i = clientStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = Encoding.ASCII.GetString(buffer, 0, i);
                Console.WriteLine("Received: {0}", data);

                // Process the data sent by the client.
                data = data.ToUpper();

                byte[] msg = Encoding.ASCII.GetBytes(data);

                // Send back a response.
                clientStream.Write(msg, 0, msg.Length);
                Console.WriteLine("Sent: {0}", data);
            }

            // Once we've reached this point, we need to close the connection as we are done.
            client.Close();
        }

        #region Mining
            List<Thread> miningThreads = new List<Thread>();

            public void StartMining(int requestedThreads, Session xiSession)
            {
                for (int i = 0; i < requestedThreads; i++)
                {                   
                    // We start a new thread
                    ThreadStart threadRef = new ThreadStart(MinerThread);
                    Thread newThread = new Thread(threadRef);

                    newThread.Start();

                    miningThreads.Add(newThread);
                }
            }

            bool blockFound;

            void MinerThread()
            {
                // Got to do this to avoid a StackOverflowException :p
                Random r = new Random();

                while (AttemptMine(r.Next(0, 100000)))
                {

                }
            }

            public bool AttemptMine(int nonce)
            {
                blockFound = AttemptToFindBlock(nonce);

                if (blockFound)
                    nonce = 0;

                nonce++;

                return true;
            }

            bool AttemptToFindBlock(int attemptedNonce)
            {
                Block lastBlock = blockchain[blockchain.Count - 1];

                Block newBlock = new Block()
                {
                    blockNumber = lastBlock.blockNumber + 1,
                    previousBlockHash = lastBlock.blockHash,
                    timestamp = Utility.UTCEpocTime(),
                    difficulty = Validation.GetDifficulty(blockchain),
                    nonce = attemptedNonce
                };

                newBlock.blockHash = Validation.CalculateBlockHash(newBlock);

                if ( Validation.DoesHashMatchDifficulty(newBlock.blockHash.ToString(), newBlock.difficulty) && Validation.ValidateBlockIntegrity(newBlock, lastBlock) )
                {
                    Console.WriteLine("Found a block. Block Number: " + newBlock.blockNumber + " Nonce: " + attemptedNonce + " Difficulty: " + newBlock.difficulty);
                    Broadcast.BroadcastNewBlock(blockchain, knownPeers, newBlock);

                    return true;
                }else
                {
                    //Console.WriteLine("Nonce " + attemptedNonce + " failed at difficulty " + newBlock.difficulty);
                    return false;
                }                                
            }
        #endregion


        static class Broadcast
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="blockchain"></param>
            /// <param name="peerList"></param>
            public static void BroadcastNewBlock(List<Block> blockchain, List<Peer> peerList, Block newBlock)
            {
                Block lastBlock = blockchain[blockchain.Count - 1];

                blockchain.Add(newBlock);
            }

            static Block FindLatestBlock()
            {
                Block latestBlock = new Block();

                return latestBlock;
            }
        }

        static class Validation
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="newBlock"></param>
            /// <param name="lastBlock"></param>
            /// <returns></returns>
            public static bool ValidateBlockIntegrity(Block newBlock, Block lastBlock)
            {
                // We check to see if the block number is correct (i.e. matches our latest block plus one, which makes it the next block)
                if (lastBlock.blockNumber != newBlock.blockNumber - 1)
                {
                    Console.WriteLine("ERROR: Invalid block number. It has been rejected.");
                    return false;
                }

                // Check to make sure that the last blocks hash matches up with 
                if (lastBlock.blockHash != newBlock.previousBlockHash)
                {
                    Console.WriteLine("ERROR: New block's previous hash does not match last local block's hash. It has been rejected.");
                    return false;
                }

                if (newBlock.blockHash != CalculateBlockHash(newBlock))
                {
                    Console.WriteLine("ERROR: New block's hash does not match the data stored within it. It has been rejected.");
                    return false;
                }

                // We allow a variance of up to 60 seconds each way to prevent people using the timestamp to tamper with difficulty
                // If variance > 60 seconds the block is invalid.
                if(newBlock.timestamp - lastBlock.timestamp < -60)
                {
                    Console.WriteLine("ERROR: New block's timestamp is below the allowed 60 seconds variance. It has been rejected.");
                    return false;
                }

                if (Utility.UTCEpocTime() - newBlock.timestamp < -60)
                {
                    Console.WriteLine("ERROR: New block's timestamp is above the allowed 60 seconds variance. It has been rejected.");
                    return false;
                }

                // If we get this far, the block has passed the integrity check - it should be fine!
                return true;
            }

            /// <summary>
            /// Takes in all of the data from the current block to create a hash. This returns what this hash SHOULD have as a hash. This will prevent injection of false data.
            /// </summary>
            /// <returns> Complete hash for the current block </returns>
            public static string CalculateBlockHash(Block targetBlock)
            {
                // TODO: Add wallets and data in here
                return Utility.EncryptString(targetBlock.blockNumber.ToString() + targetBlock.timestamp + targetBlock.previousBlockHash.ToString());
            }

            /// <summary>
            /// Checks if the new blockchain provided is valid or not, by finding whether it is longer than the current blockchain or not. (will change this to largest total difficulty soon)
            /// If it is equal - the race is on! The longer chain always takes the priority.
            /// </summary>
            /// <param name="localBlockchain"> The local blockchain </param>
            /// <param name="newBlockchain"> The external blockchain to test </param>
            /// <returns></returns>
            public static bool ValidateBlockchain(List<Block> localBlockchain, List<Block> newBlockchain)
            {
                if (localBlockchain.Count < newBlockchain.Count)
                    return true;
                else
                    return false;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="blockchain"></param>
            /// <param name="difficultyAdjustmentInterval"></param>
            /// <returns></returns>
            public static long GetDifficulty(List<Block> blockchain)
            {
                Block lastBlock = blockchain[blockchain.Count - 1];

                // We check to see if it's not the first block, and if it's not, we see if it's time for the difficulty to be updated (determined by difficultyAdjustmentInterval
                if(lastBlock.blockNumber % DIFFICULTY_ADJUSTMENT_INTERVAL == 0 && lastBlock.blockNumber != 0)
                    return CalculateNewDifficulty(blockchain);
                else
                    return lastBlock.difficulty;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="blockchain"></param>
            /// <param name="difficultyAdjustmentInterval"></param>
            /// <param name="blockGenerationInterval"></param>
            /// <returns></returns>
            public static long CalculateNewDifficulty(List<Block> blockchain)
            {
                Block lastBlock = blockchain[blockchain.Count - 1];
                Block lastAdjustedBlock = blockchain[blockchain.Count - DIFFICULTY_ADJUSTMENT_INTERVAL];

                // We figure out how long it should have taken to complete the last block
                float expectedTime = BLOCK_GENERATION_INTERVAL * DIFFICULTY_ADJUSTMENT_INTERVAL;

                // Now we figure out how long it actually took
                float actualTime = lastBlock.timestamp - lastAdjustedBlock.timestamp;

                // If it took less than 50% of the expected time of the expected time, we need to increase the difficulty.
                if(actualTime < expectedTime * 0.5)
                {
                    return lastAdjustedBlock.difficulty + 1;
                }
                // If it took more than 1.5x the amount of time expected, we need to decrease the difficulty.
                else if(actualTime > expectedTime * 1.5)
                {
                    return lastAdjustedBlock.difficulty - 1;
                }
                // But if it was within the 50% margin of error, then it's okay to keep on working at this difficulty!
                else
                {
                    return lastAdjustedBlock.difficulty;
                }
            }

            /// <summary>
            /// This helps us verify whether the hash is correct in terms of difficulty. Without this, we can't prove that there has actually been work done (mining).
            /// </summary>
            /// <param name="hash"></param>
            /// <param name="difficulty"></param>
            /// <returns></returns>
            public static bool DoesHashMatchDifficulty(string hash, long difficulty)
            {
                string requiredPrefix = "";
                string binaryHash = Utility.ByteToBinary(Utility.StringToByteArray(hash, Encoding.ASCII));

                // We quickly need to iterate through 
                int i = 0;

                while (i < difficulty)
                {
                    requiredPrefix += "0";
                    i++;
                }

                // Now we return whether the binary version of the hash matches the required prefix
                return binaryHash.StartsWith(requiredPrefix);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    class Peer
    {
        public string ipAddress;
        public List<Peer> peers;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    struct Block
    {
        /// <summary> The current blocks index (starts from 0, builds up) </summary>
        public long blockNumber;
        /// <summary> Time this block was finalised in UTC epoch seconds </summary>
        public long timestamp;
        /// <summary> How many zeroes must be in front of this block for it to be considered valid </summary>
        public long difficulty;

        /// <summary> The nonce what we use to iterate through our Proof of Work system. By changing it, we try out a new hash, therefore getting closer to the final hash!  </summary>
        public long nonce;

        /// <summary> SHA512 hash of the current block </summary>
        public string blockHash;
        /// <summary> SHA512 hash of the previous block </summary>
        public string previousBlockHash;

        /// <summary> List of all new transactions in existence at the time of this block (does not include previous block transactions)</summary>
        public List<Transaction> transactions;
        /// <summary> List of all wallets in existence at the time of this block. </summary>
        public List<Wallet> wallets;

        /// <summary>  </summary>
        public List<String> otherData;
    }

    /// <summary>
    /// Contains all of the public information for a wallet.
    /// </summary>
    [Serializable]
    struct Wallet
    {
        /// <summary> The value in the wallet </summary>
        public float value;
        /// <summary> List of all known transactions to do with the wallet </summary>
        public Transaction[] transactions;

        /// <summary> Byte list for the wallet address </summary>
        public byte[] address;
        /// <summary> Byte list for public key </summary>
        public byte[] publicKey;
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    struct Transaction
    {
        /// <summary> Value of the transaction </summary>
        public float value;
        /// <summary> Fee paid to miners </summary>
        public float minerFee;
        /// <summary> The address sending the specified value </summary>
        public byte[] sendingAddress;
        /// <summary> The address receiving the specified value </summary>
        public byte[] targetAddress;
        /// <summary> Reference to the time of the transaction </summary>
        public DateTime transactionTime;
    }    

    /// <summary>
    /// 
    /// </summary>
    class Utility
    {
        /// <summary>
        /// Encrypts a given string with SHA256 algorithm. If no string is specified, then the string is generated at random
        /// </summary>
        /// <param name="stringToEncrypt">String to be encrypted</param>
        /// <returns>Encryped string in form of byte list</returns>
        public static string EncryptString(string stringToEncrypt = "")
        {
            if (stringToEncrypt == "")
                stringToEncrypt = GetRandomString();

            using (SHA256 sha256 = new SHA256Managed())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToEncrypt)).ToString();
            }
        }

        /// <summary>
        /// This is the best way of generating a random string using RNGCryptoServiceProvider (better randomness)
        /// </summary>
        /// <returns>Randomnly generated string</returns>
        public static string GetRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }

        public static byte[] StringToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static String ByteToBinary(Byte[] data)
        {
            return string.Join("", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int UTCEpocTime()
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (int)t.TotalSeconds;
        }
    }
}
