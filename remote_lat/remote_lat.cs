
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

class remote_lat
{

    static unsafe int Main(string[] args)
    {
        if (args.Length != 3)
        {
            Console.Out.WriteLine("usage: remote_lat <address> " +
                "<message-size> <roundtrip-count>\n");
            return 1;
        }

        String address = args[0];
        uint messageSize = Convert.ToUInt32(args[1]);
        int roundtripCount = Convert.ToInt32(args[2]);

        //  Initialise 0MQ infrastructure
        ZMQ.Context ctx = new ZMQ.Context(1, 1, 0);
        ZMQ.Socket s = ctx.Socket(ZMQ.REQ);
        s.Connect(address);

        //  Create a message to send.
        byte[] msg = new byte[messageSize];

        //  Start measuring the time.
        System.Diagnostics.Stopwatch watch;
        watch = new Stopwatch();
        watch.Start();

        //  Start sending messages.
        for (int i = 0; i < roundtripCount; i++)
        {
            s.Send(msg);
            s.Recv(out msg);
            Debug.Assert(msg.Length == messageSize);
        }

        //  Stop measuring the time.
        watch.Stop();
        Int64 elapsedTime = watch.ElapsedTicks;

        //  Print out the test parameters.
        Console.Out.WriteLine("message size: " + messageSize + " [B]");
        Console.Out.WriteLine("roundtrip count: " + roundtripCount);

        //  Compute and print out the latency.
        double latency = (double)(elapsedTime) / roundtripCount / 2 *
            1000000 / Stopwatch.Frequency;
        Console.Out.WriteLine("Your average latency is {0} [us]",
            latency.ToString("f2"));

        return 0;
    }
}