﻿using SEB.BusinessLogic;
using SEB.DataAccess;
using SEB.DataAccess.Interfaces;
using SEB.HTTP;
using SEB.HTTP.Endpoints;

namespace SEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // initialize connection string for DB connection
            string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=sebdb";
            // initialize http server
            HttpServer? server = null;

            try
            {
                Console.WriteLine("Starting SEB...");

                // Initialize DB / connection
                Console.WriteLine("Cleaning up Database...");
                DB_Manager.CleanupTables(connectionString);

                Console.WriteLine("Initializing Database...");
                DB_Manager.InitializeDatabase(connectionString);

                // initialize repos
                IUserRepo userRepo = new UserRepo(connectionString);
                IExerciseRepo exerciseRepo = new ExerciseRepo(connectionString);
                ITournamentRepo tournamentRepo = new TournamentRepo(connectionString);

                // initialize handlers
                UserHandler userHandler = new UserHandler(userRepo);
                ExerciseHandler exerciseHandler = new ExerciseHandler(exerciseRepo);
                TournamentHandler tournamentHandler = new TournamentHandler(tournamentRepo, exerciseRepo, userRepo);

                // initialize server
                Console.WriteLine("Setting up server...");
                server = new HttpServer();
                var serverThread = new Thread(() => server.Run());

                // initialize endpoints
                server.RegisterEndpoint("users", new UserEP(userHandler));                                                  // register
                server.RegisterEndpoint("sessions", new SessionEP(userHandler));                                            // login
                server.RegisterEndpoint("stats", new StatsEP(userHandler, exerciseHandler));                                // stats
                server.RegisterEndpoint("score", new ScoreEP(userHandler, exerciseHandler));                                                 // scoreboard
                server.RegisterEndpoint("history", new HistoryEP(exerciseHandler, userHandler, tournamentHandler));                            // history
                server.RegisterEndpoint("tournament", new TournamentEP(tournamentHandler, userHandler, exerciseHandler));   // tournament

                // start server
                serverThread.Start();

                // shut down option for HttpServer
                Console.WriteLine("Press 'q' to stop the server...");
                while (Console.ReadKey(true).Key != ConsoleKey.Q)
                {
                    Thread.Sleep(100); // avoids unnecessary resource usage
                }
                Console.WriteLine("Stopping server...");
                server.Stop();          // signal the loop to exit
                serverThread.Join();    // wait for it to clean up and stop

            }
            finally
            {
                // cleanup
                // only for debug mode, not in production/release -> performance-heavy checks, so only for debug
                //#if DEBUG
                Console.WriteLine("Cleaning up Database...");
                DB_Manager.CleanupTables(connectionString);
                //#endif

                Console.WriteLine("Program ended");
            }
        }
    }
}