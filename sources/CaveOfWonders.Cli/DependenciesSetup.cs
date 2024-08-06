﻿// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Autofac;
using CsvParser.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Adapters.BnrAccess;
using DustInTheWind.CaveOfWonders.Adapters.DataAccess.Json;
using DustInTheWind.CaveOfWonders.Adapters.InsAccess;
using DustInTheWind.CaveOfWonders.Adapters.SystemAccess;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentState;
using DustInTheWind.CaveOfWonders.Ports.BnrAccess;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.InsAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using DustInTheWind.CsvParser.Adapters.LogAccess;
using DustInTheWind.CsvParser.Adapters.SheetsAccess;
using DustInTheWind.CsvParser.Ports.SheetsAccess;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;

namespace DustInTheWind.CaveOfWonders.Cli;

internal class DependenciesSetup
{
    public static void Configure(ContainerBuilder containerBuilder)
    {
        MediatRConfiguration mediatRConfiguration = MediatRConfigurationBuilder
            .Create(typeof(PresentStateRequest).Assembly)
            .WithAllOpenGenericHandlerTypesRegistered()
            .Build();

        containerBuilder.RegisterMediatR(mediatRConfiguration);

        containerBuilder
            .Register(context => new Database(@"c:\Projects.pet\finanțe\CaveOfWonders\db"))
            .AsSelf();
        containerBuilder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

        containerBuilder.RegisterType<SystemClock>().As<ISystemClock>();
        containerBuilder.RegisterType<Bnr>().As<IBnr>();
        containerBuilder.RegisterType<Ins>().As<IIns>();
        containerBuilder.RegisterType<Sheets>().As<ISheets>();
        containerBuilder.RegisterType<Log>().As<ILog>().InstancePerLifetimeScope();
    }
}