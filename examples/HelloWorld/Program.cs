namespace TcpStumps.Examples.HelloWorld
{
    /* TCP-Stumps Hello World API Example
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

    class Program
    {
        static async Task Main(string[] args)
        {
            ConsoleHelper.ApplicationBanner("Hello World API");

            // *************************************************
            // STEP 1: SETUP SERVER
            // *************************************************

            // Create a new TCP Stumps server
            var server = new TcpStumpsServer();

            // Define a protocol which accepts messages based on a LF character.
            var protocol = new SentinelDelimitedProtocolFactory
            {
                SentinelValues = new byte[] { (byte)'\n' }
            };
            server.Protocol = protocol;

            // Another protocol allows only fixed-length messages
            // var protocol = new FixedLengthProtocolFactory()
            // {
            //     MessageLength = 32
            // };

            // Yet another handles clients that first send a header which includes the total message length
            // which in this example is the first two bytes.
            // var protocolX = new HeaderDefinedLengthProtocolFactory()
            // {
            //     HeaderLength = 2,
            //     LengthCalculator = (headerBytes) => BitConverter.ToInt16(headerBytes, 0)
            // };

            // Create a default response returned when the server doesn't understand the request.
            // The default is to simply disconnect.
            var defaultResponse = new TcpResponse
            {
                new TcpMessage
                {
                    Message = "I don't understand.  Choose: animal, sing, quit)\r\n".FromAsciiString()
                }
            };
            server.Protocol.DefaultResponse = defaultResponse;

            // The server will only listen for local connections by default
            // server.AllowExternalConnection = true;

            // An open port will be chosen at random unless specified
            // server.ListeningPort = 9100;

            // *************************************************
            // STEP 2: CREATE SOME STUMPS
            // *************************************************

            // -------------------------------------------------
            // Simple Stump
            // -------------------------------------------------

            // First we will create a new stump for animals, we will return a different animal each time.
            var animalStump = server.AddNewStump("animalStump");

            // Now we create a rule for the stump
            animalStump.AddRule(new ContainsBytesRule("animal".FromAsciiString()));

            // The response factory will choose from one of the possible responses.
            var animalResponses = new StumpResponseFactory();

            animalResponses.AddResponseMessage("Dog\r\n".FromAsciiString());
            animalResponses.AddResponseMessage("Cat\r\n".FromAsciiString());
            animalResponses.AddResponseMessage("Lion\r\n".FromAsciiString());
            animalResponses.AddResponseMessage("Tiger\r\n".FromAsciiString());
            animalResponses.AddResponseMessage("Bear\r\n".FromAsciiString());

            animalStump.Responses = animalResponses;

            // -------------------------------------------------
            // Stump where response sends multiple messages
            // -------------------------------------------------

            // One stump may require multiple messages to be sent as part of a response
            var singStump = server.AddNewStump("sing");
            singStump.AddRule(new ContainsBytesRule("sing".FromAsciiString()));

            var singResponse = new StumpResponseFactory();
            singResponse.Add(new TcpResponse
            {
                new TcpMessage
                {
                    Message = "Twinkle, twinkle little star\r\n".FromAsciiString()
                },
                new TcpMessage
                {
                    Message = "How I wonder what you are\r\n".FromAsciiString()
                },
                new TcpMessage
                {
                    Message = "Up above the world so high\r\n".FromAsciiString(),
                    ResponseDelay = 2000
                },
                new TcpMessage
                {
                    Message = "Like a diamond in the sky\r\n".FromAsciiString()
                },
                new TcpMessage
                {
                    Message = "... you get the idea.\r\n".FromAsciiString()
                }
            });

            singStump.Responses = singResponse;

            // -------------------------------------------------
            // Stump where response closes the connection
            // -------------------------------------------------

            var quitStump = server.AddNewStump("quit");
            quitStump.AddRule(new ContainsBytesRule("quit".FromAsciiString()));

            var quitResponse = new StumpResponseFactory();
            quitResponse.Add(new TcpResponse
            {
                new TcpMessage
                {
                    TerminateConnection = true
                }
            });

            quitStump.Responses = quitResponse;

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
    }
}
