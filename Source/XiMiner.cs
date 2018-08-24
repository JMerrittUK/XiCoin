using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XiCoin
{
    public class XiMiner
    {
        public Session xiSession;
        public MiningStats miningStats = new MiningStats();

        List<Thread> miningThreads = new List<Thread>();

        private System.Windows.Forms.Timer statTimer;
        bool blockFound;

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

            InitStatTimer();
        }

        public void InitStatTimer()
        {
            statTimer = new System.Windows.Forms.Timer();
            statTimer.Tick += new EventHandler(StatTimer_Tick);
            statTimer.Interval = 2000; // in miliseconds
            statTimer.Start();
        }

        private void StatTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(miningStats.ToString());
        }


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
            miningStats.hashes++;

            if (blockFound)
            {
                Random r = new Random();
                nonce = r.Next(0, 100000);

                miningStats.blocksFound++;
            }

            nonce++;

            return true;
        }

        bool AttemptToFindBlock(int attemptedNonce)
        {
            Block lastBlock = xiSession.blockchain.Last().Value;

            Block newBlock = new Block()
            {
                blockNumber = lastBlock.blockNumber + 1,
                previousBlockHash = lastBlock.blockHash,
                timestamp = Utility.UTCEpocTime(),
                difficulty = Session.Validation.GetDifficulty(xiSession.blockchain),
                nonce = attemptedNonce
            };

            newBlock.blockHash = Session.Validation.CalculateBlockHash(newBlock);

            if (Session.Validation.DoesHashMatchDifficulty(newBlock.blockHash, newBlock.difficulty))
            {
                if(Session.Broadcast.BroadcastNewBlock(xiSession.blockchain, xiSession.knownPeers, newBlock))
                {
                    Console.WriteLine("Found a block. Block Number: " + newBlock.blockNumber + " Nonce: " + attemptedNonce + " Difficulty: " + newBlock.difficulty);
                    xiSession.SaveData();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //Console.WriteLine("Nonce " + attemptedNonce + " failed at difficulty " + newBlock.difficulty);
                return false;
            }
        }
    }

    public struct MiningStats
    {
        public int hashes;
        public int blocksFound;
        public float secondsSpentMining;

        override public string ToString()
        {
            return "Hash rate: " + (hashes / secondsSpentMining) + "/s | Blocks found: " + blocksFound;
        }
    }
}
