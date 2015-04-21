//20110325  v0.91
using System;
using System.Threading;

namespace TTToolbox
{
   //auto connection
    public abstract class ClientKeeper: IDisposable
    {
        protected delegate object Action();

        private Thread monitor_ = null;
        private object mutex_ = new object();
        private bool has_sent_ = false;
        private bool end_ = false;
        private int endure_ = 0;
        private int ENDURE_LIMIT;

        #region Parameters

        public bool IdleResetEnabled{get; set;}

        //minutes
        public int AllowIdleTime { get; set; }

        //milli-second
        public int AllowInitTime{ get; set; }
        
        //seconds
        public int RetryPeriod { get; set; }

        #endregion

        //connection end
        public bool End
        {
            get{ return end_;}
        }

        //Need overrided Method
        public abstract bool Alive();       //require: no exception
        protected abstract void Reset();    //require: no excepton
        protected abstract void Open();
        protected abstract void Close();


        protected ClientKeeper()
        {
            IdleResetEnabled = true;
            AllowIdleTime = 60;
            AllowInitTime = 3000;
            RetryPeriod = 10;

            monitor_ = new Thread(new ThreadStart(Monitor));
        }

        #region Public Interface

        public void Start()
        {
            ENDURE_LIMIT = AllowIdleTime * 60 / RetryPeriod;

            monitor_.Start();

            int ms = 300;
            for (int times = AllowInitTime / ms; times > 0 && !Alive(); times--)
                Thread.Sleep(ms);
        }

        public void Dispose() { end_ = true; }

        #endregion

        #region Implementation

        private void ResetConn()   //saft close
        {   
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Reset();
        }

        private void DetectIdleReset()
        {
            //Detect
            endure_ = has_sent_ ? 0 : endure_ + 1;
            has_sent_ = false;

            //閒置過久斷線
            if (endure_ > ENDURE_LIMIT)
                ResetConn();
        }

        private void Monitor()
        {
            while (!End) try
            {
                lock (mutex_)
                {
                    //Reset when idle
                    if (IdleResetEnabled)
                        DetectIdleReset();

                    //重連
                    if (!Alive())
                    {
                        endure_ = 0;
                        Open();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("ClientKeeper.Open() Error: " + e.ToString());
            }
            finally
            {
                try
                {
                    for (int i = 0; i < RetryPeriod && !End; ++i)
                        Thread.Sleep(1000);
                }
                catch { }
            }

            //release
            ResetConn();
        }

        protected object OnConnection(Action action){
            lock (mutex_)
            {
                //連線不存在 直接Throw即可
                if(!Alive()) throw new Exception("Connceton Failed");

                has_sent_ = true;
                
                try{
                    return action();
                }
                catch(Exception e)
                {
                    Console.WriteLine("ClientKeeper Error: " + e.ToString());
                    ResetConn();
                    throw;
                }
            }
        }

        #endregion

    }
}


