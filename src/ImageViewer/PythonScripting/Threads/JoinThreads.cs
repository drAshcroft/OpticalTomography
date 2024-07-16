using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;
using ImageViewer.Filters;
using System.Threading;

namespace ImageViewer.PythonScripting.Threads
{
    public class JoinThreadsTool : aEffectNoForm
    {
        public override string EffectName { get { return "Join Threads"; } }
        public override string EffectMenu { get { return "Macros"; } }
        public override string EffectSubMenu { get { return "Threads"; } }
        public override int OrderSuggestion
        {
            get
            {
                return 5;
            }
        }

        public override bool PassesPassData
        {
            get
            {
                return false;
            }
        }


        public class JoinThreadsToken : IEffectToken
        {
            public string TokenName()
            {
                return this.ToString();
            }
            public int WaitCount=0;
            public bool Dump = false;
            public Barrier _Barrier;
        }

        public static void ReleaseAllJoins(DataEnvironment dataEnvironment)
        {
            foreach (IEffectToken iet in dataEnvironment.EffectTokens.Values )
            {
                if (iet.GetType()==typeof(JoinThreadsToken ))
                {
                    JoinThreadsToken jtt = (JoinThreadsToken)iet;
                    jtt.WaitCount = 10000;
                   
                    jtt._Barrier.RemoveParticipants(jtt._Barrier.ParticipantsRemaining );
                }
            }
        }

        public static void JoinThreads(DataEnvironment dataEnvironment, string JoinName, int ThreadNumber)
        {

            if (dataEnvironment.ThreadsRunning != null)
            {

                JoinThreadsToken jtt;

                lock (CriticalSectionLock)
                {


                    if (dataEnvironment.EffectTokens.ContainsKey("JoinThreadsToken" + JoinName) == true)
                        jtt = (JoinThreadsToken)dataEnvironment.EffectTokens["JoinThreadsToken" + JoinName];
                    else
                    {
                        jtt = new JoinThreadsToken();
                        dataEnvironment.EffectTokens.Add("JoinThreadsToken" + JoinName, jtt);
                    }
                    if (jtt._Barrier == null)
                        jtt._Barrier = new Barrier(dataEnvironment.ThreadsRunning.Count);
                    if (jtt.Dump == true)
                        jtt.Dump = false;
                }

                jtt.WaitCount ++;

                /*while (jtt.WaitCount < dataEnvironment.ThreadsRunning.Count && jtt.Dump==false )
                {
                    Thread.Sleep(100);
                }*/
                
                jtt._Barrier.SignalAndWait();
                
                ///dump makes sure that we can reset the counter, without locking all the other threads still in the loop
                jtt.Dump = true;
                jtt.WaitCount =0;
            }
        }

        static object CriticalSectionLock = new object();
       
        public override object DoEffect(DataEnvironment dataEnvironment, object SourceImage,
             ReplaceStringDictionary PassData, params object[] Parameters)
        {
            mFilterToken = Parameters;

            int ThreadID = 0;
            try
            {
                ThreadID = (int)Parameters[0];
            }
            catch
            {
            }

            JoinThreads(dataEnvironment, (string)Parameters[1], ThreadID);

            
            return SourceImage;
        }
        public override object[] DefaultProperties
        {
            get { return new object[] { 0 }; }
        }

        public override string[] ParameterList
        {
            get { return new string[] { "ThreadID|int" }; }
        }

    }
}
