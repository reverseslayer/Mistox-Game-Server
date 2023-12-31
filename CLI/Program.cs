﻿using System;
using System.Threading.Tasks;
using MistoxServer;

namespace MistoxHolePunch {
    class Program {

        static IMistoxServer serverObj;
        static bool running = true;

        void onConnected( object sender, EventArgs e ) {
            // sender and e are always null
            // put connected functions in here
        }

        void slowReceive( object obj, EventArgs e ) {
            if ( serverObj is ServerInterface ) {
                // If ServerMode is passive obj is byte[] and Send takes in byte[]
                // If Servermode is Athoritiative obj is the Class type sent and Send takes in any Class type except generic object

                // Check to make sure data is correct before relaying
                // Also perform server specific checks in here
                // IE player didnt teleport or is shooting from 20 feet from his body
                serverObj.Send( obj, SendType.SlowUpdate );
            }
            Console.Write( "Received TCP : " );
            Console.WriteLine( obj );
        }

        void fastReceive( object obj, EventArgs e ) {
            if( serverObj is ServerInterface ) {
                // If ServerMode is passive obj is byte[] and Send takes in byte[]
                // If Servermode is Athoritiative obj is the Class type sent and Send takes in any Class type except generic object

                // Check to make sure data is correct before relaying
                // Also perform server specific checks in here
                // IE player didnt teleport or is shooting from 20 feet from his body
                serverObj.Send( obj, SendType.FastUpdate );
            }
            Console.Write( "Received UDP : " );
            Console.WriteLine( obj );
        }

        void onDisconnected( object sender, EventArgs e ) {
            // sender and e are always null
            // put disconnected functions in here
        }

        static string host = "example.com";
        static int port = 7300;
        static ServerMode mode = ServerMode.Passive;

        async Task RunServer() {
            serverObj = mServer.newServer( Convert.ToInt32( port ), mode );
            serverObj.onConnected += onConnected;
            serverObj.onSlowReceive += slowReceive;
            serverObj.onFastReceive += fastReceive;
            serverObj.onDisconnected += onDisconnected;
            while ( running ) {
                // Stop this thread for 1 second so the CPU isnt 100%
                // All the real work is on different threads
                // While loop needs to exist so our main thread doesnt close
                await Task.Delay( 1000 ); 
            }
        }

        async Task RunClient() {
            serverObj = mServer.newClient( host, Convert.ToInt32( port ) );
            serverObj.onConnected += onConnected;
            serverObj.onSlowReceive += slowReceive;
            serverObj.onFastReceive += fastReceive;
            serverObj.onDisconnected += onDisconnected;
            while( running ) {
                // Stop this thread for 1 second so the CPU isnt 100%
                // All the real work is on different threads
                // While loop needs to exist so our main thread doesnt close
                await Task.Delay( 1000 );
            }
        }

        static async Task Main(string[] args) {
            string Task = args.Length > 0 ? args[0].ToLower() : null;

            for( int i = 0; i < args.Length; i++ ) {
                string cur = args[i].ToLower();
                if( cur == "/h" || cur == "-h" ) {
                    host = args [i + 1];
                } else if( cur == "/p" || cur == "-p" ) {
                    port = Convert.ToInt32( args [i + 1] );
                } else if( cur == "/a" || cur == "-a" ) {
                    mode = ServerMode.Authoritative;
                }
            }

            if (Task == "/?" || Task == "--help") {
                Console.WriteLine(HelpDocumentation.HelpText);
            } else if (Task == "/s" || Task == "-s") {
                Program prog = new Program();
                await prog.RunServer();
            } else if (Task == "/c" || Task == "-c") {
                Program prog = new Program();
                await prog.RunClient();
            } else {
                host = "example.com";
                port = 1500;
                Program prog = new Program();
                //prog.RunClient();
                await prog.RunServer();
            }
        }

    }

}