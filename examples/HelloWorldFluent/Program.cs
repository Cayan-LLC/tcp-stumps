namespace TcpStumps.Examples.HelloWorldFluent
{
    /* TCP-Stumps Hello World (Fluent) API Example
     * ==================================
     * 
     * This Example is used to demonstrate using the basic API provided by TCP-Stumps
     * to create a new instance of a Stumps TCP server.
     * 
     * Using telnet:
     * Requesting "animal" will return back a name of an animal.
     * Requesting "sing" will sing a little song.
     * Requesting "quit" will disconnect.
     */
    using System;
    using System.Threading.Tasks;

    public class Program
    {
        static async Task Main(string[] args)
        {
            ConsoleHelper.ApplicationBanner("Hello World (Fluent) API");

            // *************************************************
            // STEP 1: SETUP SERVER
            // *************************************************

            // Create a new Stumps server
            var server = new TcpStumpsServer();

            server.UsesProtocol()
                .WithSentinelsToDeliminateMessages(
                    readPastSentinelLength: 1,
                    sentinels: 13
                ).ReturnsByDefault()
                    .TheMessage("I don't understand.  (Choose \"animal\", \"sing\", \"noop\" or \"quit\")\r\n".FromAsciiString());

            // *************************************************
            // STEP 2: CREATE SOME STUMPS
            // *************************************************

            // First we will create a new stump for animals, we will return a different animal with each request.
            server.HandlesRequest()
                .WhenMessageContainsBytes("animal".FromAsciiString())
                .RespondWith()
                    .TheMessage("Dog\r\n".FromAsciiString())
                    .TheMessage("Cat\r\n".FromAsciiString())
                    .TheMessage("Lion\r\n".FromAsciiString())
                    .TheMessage("Tiger\r\n".FromAsciiString())
                    .TheMessage("Bear\r\n".FromAsciiString());

            // One stump may require multiple messages to be sent as part of a single response.  Each one can have
            // its own delay or simply drop the connection.
            server.HandlesRequest()
                .WhenMessageContainsBytes("sing".FromAsciiString())
                .RespondWith().MultipleMessages()
                    .TheMessage("Twinkle, twinkle little star\r\n".FromAsciiString())
                    .TheMessage("How I wonder what you are\r\n".FromAsciiString())
                    .TheDelayedMessage(
                        message: "Up above the world so high\r\n".FromAsciiString(),
                        delayTime: 2000)
                    .TheMessage("Like a diamond in the sky.\r\n".FromAsciiString())
                    .TheMessage("... you get the idea.\r\n".FromAsciiString());

            // This one doesn't do anything at all.
            server.HandlesRequest()
                .WhenMessageContainsBytes("noop".FromAsciiString())
                .RespondWith().Nothing();

            // Easy to drop the connection when a user calls "quit"
            server.HandlesRequest()
                .WhenMessageContainsBytes("quit".FromAsciiString())
                .RespondWith().DropConnection();

            // *************************************************
            // STEP 3: Some happy extras!
            // *************************************************

            server.OnClientConnection += (o, e) => Console.WriteLine($"Connect => {e.Connection.ConnectionId}");
            server.OnClientDisconnect += (o, e) => Console.WriteLine($"Disconnect => {e.Connection.ConnectionId}");

            // *************************************************
            // STEP 4: RUN THE SERVER!
            // *************************************************

            await server.StartAsync();

            // Show the address to the user
            Console.WriteLine($"You can access the server by running telnet > 127.0.0.1 : {server.ListeningPort}");
            Console.WriteLine();

            ConsoleHelper.WaitForExit();

            await server.ShutdownAsync();
        }

        static void AlternateFluentSevers()
        {
            // Fixed-length messages
            var server2 = new TcpStumpsServer().ListeningOnPort(9045);

            server2.UsesProtocol()
                .WithFixedLengthMessages(messageLength: 32);

            // Using delegate to calculate message size
            var server3 = new TcpStumpsServer().ListeningOnPort(9046);

            server3.UsesProtocol()
                .WithHeadersContainingTheMessageLength(
                    headerLength: 2,
                    messageLengthCalculator: (headerBytes) => BitConverter.ToInt16(headerBytes, 0)
                );
        }
    }
}
