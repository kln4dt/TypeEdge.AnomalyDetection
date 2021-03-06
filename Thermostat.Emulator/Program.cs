﻿using System;
using System.Threading.Tasks;
using TypeEdge.Host;
using Microsoft.Extensions.Configuration;
using Modules;
using ThermostatApplication.Modules;
using TypeEdge.DovEnv;
using System.IO;

namespace ThermostatApplication
{
    internal class Program 
    {
        private static async Task Main(string[] args) 
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddDotenvFile()
                .AddCommandLine(args)
                .Build();

            var host = new TypeEdgeHost(configuration);

            host.RegisterModule<ITemperatureSensor, TemperatureSensor>();
            host.RegisterModule<IOrchestrator, Orchestrator>(); 
            host.RegisterModule<IModelTraining, ModelTraining>();
            host.RegisterModule<IVisualization, Visualization>();
            host.RegisterModule<IAnomalyDetection, AnomalyDetection>();

            host.Upstream.Subscribe(host.GetProxy<IAnomalyDetection>().Anomaly);

            var manifest = host.Build();

            File.WriteAllText("../../../manifest.json", manifest);

            await host.RunAsync();

            Console.WriteLine("Press <ENTER> to exit..");
            Console.ReadLine();
        }
    }
}