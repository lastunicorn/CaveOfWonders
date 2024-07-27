﻿// Cave of Wonders
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

using CsvParser.Ports.LogAccess;
using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CsvParser.Application.Importing;
using DustInTheWind.CsvParser.Ports.SheetsAccess;

namespace DustInTheWind.CsvParser.Application.UseCases.ImportBt;

public class ImportBtGemsUseCase
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ISheets sheets;
    private readonly ILog log;

    public string SourceFilePath { get; set; }

    public bool Overwrite { get; set; }

    public ImportBtGemsUseCase(IUnitOfWork unitOfWork, ISheets sheets, ILog log)
    {
        this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        this.sheets = sheets ?? throw new ArgumentNullException(nameof(sheets));
        this.log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public async Task<ImportBtResponse> Execute()
    {
        log.WriteInfo(new string('-', 100));
        log.WriteInfo("---> Starting import of BT Sheet.");
        log.WriteInfo($"---> Import from file: {SourceFilePath}");

        string importTypeDescription = Overwrite
            ? "overwrite"
            : "append";

        log.WriteInfo($"---> Import type: {importTypeDescription}");

        GemImport gemImport = new()
        {
            Pots = await CreatePotCollection(),
            Overwrite = Overwrite,
            Log = log
        };

        IEnumerable<SheetValue> sheetsValues = sheets.GetBtRecords(SourceFilePath);
        gemImport.Import(sheetsValues);

        await unitOfWork.SaveChanges();

        log.WriteInfo(new string('-', 100));

        return new ImportBtResponse
        {
            Report = gemImport.Report
        };
    }

    private async Task<PotCollection> CreatePotCollection()
    {
        PotCollection pots = new();

        Pot currentAccountLeiPot = await GetPot(PotIds.Bt.CurrentAccountEuro);
        pots.Add(currentAccountLeiPot, "current-account-euro");

        return pots;
    }

    private async Task<Pot> GetPot(string potId)
    {
        IEnumerable<Pot> pots = await unitOfWork.PotRepository.GetByPartialId(potId);
        return pots.Single();
    }
}