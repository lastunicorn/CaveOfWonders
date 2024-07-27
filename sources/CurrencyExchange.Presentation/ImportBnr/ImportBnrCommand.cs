﻿// Currency Exchange
// Copyright (C) 2023 Dust in the Wind
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

using DustInTheWind.ConsoleTools.Commando;
using DustInTheWind.CurrencyExchange.Application.ImportFromBnrFile;
using MediatR;

namespace DustInTheWind.CurrencyExchange.Presentation.ImportBnr;

[NamedCommand("import-bnr", Description = "Import exchange rates from a local xml file downloaded from BNR website.")]
internal class ImportBnrCommand : IConsoleCommand<ImportFromBnrResponse>
{
    private readonly IMediator mediator;

    [NamedParameter("source-file", ShortName = 's')]
    public string SourceFilePath { get; set; }

    public ImportBnrCommand(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ImportFromBnrResponse> Execute()
    {
        ImportFromBnrRequest request = new()
        {
            SourceFilePath = SourceFilePath
        };
        ImportFromBnrResponse response = await mediator.Send(request);

        return response;
    }
}