using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackBot;
using BlackjackBot.Bot;
using System.Diagnostics;
using System.IO;

namespace BlackjackBot.ConsoleHost
{
    class Program
    {
        static string url = "http://blackjackbotserver.azurewebsites.net/";

        static CustomBot bot = new CustomBot();
        static bool isMultiplayer = false; 

        static void Main(string[] args)
        {
            ListenToDebug();
            Console.ForegroundColor = ConsoleColor.Green; 

            Console.WriteLine(asciiBot);
            Console.WriteLine();
            Console.WriteLine("Would you like to play a game?");
            Console.WriteLine("1. Play 10 games solo against the dealer. Choose this to debug your bot");
            Console.WriteLine("2. Play against others in a tournament (Requires 3 players)");

            ConsoleKeyInfo key =  Console.ReadKey();

            if (key.KeyChar == '1')
            {
                isMultiplayer = false;
                Console.WriteLine("Starting solo game"); 
            }
            else if (key.KeyChar == '2')
            {
                Console.WriteLine("Joining queu to start multiplayer game"); 
                isMultiplayer = true;   
            }
            else
            {
                Console.WriteLine("You must select 1 or 2");
                Console.Read(); 
            }
            Console.WriteLine("*** ***\n***Starting Your Bot*** - Click any key to exit\n*** ***");

            try
            {
                RunAsync(isMultiplayer).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);               
            }           
            
            Console.ReadKey();
        }


        public static async Task RunAsync(bool playTournament)
        {
            try
            {
                await bot.InitializeAsync(url);

                Debug.WriteLine("Bots initialized");

                Debug.WriteLine("Calling Hello World");
                await bot.CallEchoTest("Hello World!");

                if (playTournament)
                {
                    //play multiplayer
                    await bot.JoinMultiPlayerGameQueueAsync();
                }
                else
                {
                    //play solo
                    await bot.StartSoloGameSeriesAsync(10);
                }               
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

           //NOTE: To display more data, add more Debug.WriteLine messages from inside the bots
        }

        private static void ListenToDebug()
        {            
            Trace.AutoFlush = true;
            ConsoleTraceListener console = new ConsoleTraceListener();
            Debug.Listeners.Add(console);

            //writes the log to the bin folder of the console exe. 
            string path = DateTime.Now.ToFileTime() + ".txt"; 
            TextWriterTraceListener file = new TextWriterTraceListener(path);
            Debug.Listeners.Add(file);
        }

        private static string asciiBot =
@"GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGL1fGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG
GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGL11LCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC
CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCGfLGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG
GGGGGGGGGGGGGGGGGGGGGGGGGGG0CfttttfL0000000000000000000000000000
0000000000000000000000Gt::,,::::::::;ii1tGCCCCCCCCCCCCCCCCCCCCCC
CCCCCCCCCCCCCCCGiGGGGG;:,,......,,,,:::::iCGGGGiGCCCCCCCCCCCCCCC
CCCCCCGGGGGG0Lt:,:;;;t:,,,,,,,,,,,::::::::1t;;::;tL0GGGGGGCCCCCC
CCG00CCCCGLtLi,::;;it:::::,,,,,,::::::::::iti;;;:,iLtLGCCCC00GGG
G0CCGCLffC;Lf1ii;;;;t::::::;iiiiiiiii:::::;1i;;;ii1fL;CffLCCLC00
0CC....iCftt;,::;;;;t:::fLCLLftttfLCCCLf;::it;;;::,;ttfCi....CC0
0Gt,,,,;C1f1:.,,818i1:::LCi,,;i1ttffifCf1::it818:,.:1f1C:,,,,tG0
00CCf1LCL1f1:.,,:;;i1::,CCt1t.iffftiCCGf1::it;;;:,,:1f1LCL1fCC00
0C0GftttLttt;:::;;;;t::,LCLLLLt1ffLGGGGf1::it;;;:::itttLtttfG0CC
CCCCGLGCCGGCf1tffffff;::tfLG0GGGGGGGGCL1:::1Lfffft1fC0GCCGLGGGGG
GGGGGGGGGGG0Ci.,,:;;ti::::,,,,,,,,,,,,,:::if;;;:,,iC000000000000
000000000000C0GLLft11f;,,,,,,,,,,,::::::::1f1tfLLG0CCCCCCCCCCCCC
CCCCCCCCCCCCCCCCCCCCC01:,,......,,,,:::::1CCCCCCCCCCCCCCCCCCCCCC
CCCCCCCCCCCCCCCCCCCCC@0000Gft111i11ttC000GGGGGGGGGGGGGGGGGGGGGGG
GGGGGGGGGGGGGGGGGGGGGG0@@08@80CffLG8@@00000000000000000000000000
0000000000000000000000000@G0G0GLfL000G88888888888888888888888888
888888888888888888888@CCCfLtCC1i1tfCftL@@@@000000000000000000000
000000000Ci1C000000880LLft, ;iiiiii1 .tLLCL0888888L0f11GGGGGGGGG
GGGGGGG1f;:,:;Lf0CftCttC11...LCCCCL,..;fGf1LLfC0Gft::::iffGGGGGG
GGGGGCL1,...,::ftf,.::,,..,,,1LLLft,,,,.,,::..1Lt1::,,..:tCCCCCC
CCCCC8G;:::::::081,,L1;;1L:,,;CCCCt,,,L1;;1L:,iG8f:::::::t00LLLL
LLLL8L1L1::::t00G1,,tit1;f:::;G000t:::titt;t;,iLCG0;:::;Lt1G@@@@
@@@GCfLttfGGLLLLG1:;LfttfLi::1LCLLf:::LftttLi:iCCCCCCGCftfCtGGGG
GGG0f11tCGGGGGGGGt:::::::::;1t::::1t;:::::::::1LCCCCCCL0Ltt1CCCC
CCCC11fC88888888GGfftii;;ifGCGGCCGGCGtt;;ii1LtGGGGGGGGG@8ft1t000
0C8GCCC88CCCCCCCCCCGfGLCCCGLCCCCCCCCLGCCCLGLGGGGGGGGGGGC@0CCL088
888Cttf008888888888CG00i,:fCLLGGGGLLLC;,i0GGGGGGGGGGGGG@G0ttt080
0G::fi;:i00000000000000GC;;GLfG888ftGi;CGGGGGGGGGGGGGG@G;;;1t:10
CGi1t:i;1888888888888C8G,,iGCGGGGGGCGt,;088888888888888G;i;it1tG
0ffCt;;f088888888888GGtfi;i10CfttfL0ti;1fL000000000000800i;if01G
Gi;;ti;iC0GGGGGGGG08Gi1CG0L0GGLCCCC0GL0GL1188GGGGGGGGG00f;;1i;;f
CL101t;1t:LLLLLLLG8G1tCCCfLGGGGGGGGGGfLCGftfG88888888G1;ti;tfCiG
G0fi:ti;;fGGGGGGGf1tfftfLCGGGGGGGGGGGGCLffff11CCCCCCC0G1:;ti;;GC
CGGGGLttGCC00000tiit,::;00000000000000LG;:::ti;LLLLLG0L0LtfCGGGG
GGGGGCG00C0GGGGf;tLLLf;;GGGGGGGGGGGGGGGf;iLLLL1;CGGG0GCG0GGGGGGG
GGGGGG0C0LGGGGCiL;ii1tLtCCCCCCCCCCCCCC0fLft1iiiLiGGGGGCCGG0LLLLL
LLLLLLLLLLLLLLG0ftttttfCGGGGGGGGGGGGGGCLCttttttCGGGGGGGGGGGGGGGG
GGGGGGGGGGGGGGGG0GLLCCC0000000000000000G0CLCCLG0CCCCCCCCCCCCCCCC

           ***********                  ***********
           ***********  BLACKJACK BOTS  ***********
           ***********                  ***********
"; 
    }
}
