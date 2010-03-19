
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

class local_thr
{

    static unsafe int Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.Out.WriteLine("usage: local_thr <address> " +
                "<message-size> <message-count>\n");
            return 1;
        }

        String address = args[0];
        uint messageSize = Convert.ToUInt32(args[1]);
        int messageCount = Convert.ToInt32(args[2]);

        //  Initialise 0MQ infrastructure
        ZMQ.Context ctx = new ZMQ.Context(1, 1, 0);
        ZMQ.Socket s = ctx.Socket(ZMQ.SUB);
        s.SetSockOpt(ZMQ.SUBSCRIBE, "");
        s.Bind(address);

        //  Wait for the first message.
        byte[] msg;
        s.Recv(out msg);
        Debug.Assert(msg.Length == messageSize);

        //  Start measuring time.
        System.Diagnostics.Stopwatch watch;
        watch = new Stopwatch();
        watch.Start();

        //  Receive all the remaining messages.
        for (int i = 1; i < messageCount; i++)
        {
            s.Recv(out msg);
            Debug.Assert(msg.Length == messageSize);
        }

        //  Stop measuring the time.
        watch.Stop();
        Int64 elapsedTime = watch.ElapsedTicks;

        // Compute and print out the throughput.
        Int64 messageThroughput = (Int64)(messageCount * Stopwatch.Frequency /
            elapsedTime);
        Int64 megabitThroughput = messageThroughput * messageSize * 8 /
            1000000;
        Console.Out.WriteLine("Your average throughput is {0} [msg/s]",
            messageThroughput.ToString());
        Console.Out.WriteLine("Your average throughput is {0} [Mb/s]",
            megabitThroughput.ToString());

        return 0;
    }
}