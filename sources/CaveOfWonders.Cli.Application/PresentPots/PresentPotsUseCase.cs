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

using DustInTheWind.CaveOfWonders.Domain;
using DustInTheWind.CaveOfWonders.Ports.DataAccess;
using DustInTheWind.CaveOfWonders.Ports.SystemAccess;
using MediatR;

namespace DustInTheWind.CaveOfWonders.Cli.Application.PresentPots;

internal class PresentPotsUseCase : IRequestHandler<PresentPotsRequest, PresentPotsResponse>
{
    private readonly IPotRepository potRepository;
    private readonly ISystemClock systemClock;

    public PresentPotsUseCase(IPotRepository potRepository, ISystemClock systemClock)
    {
        this.potRepository = potRepository ?? throw new ArgumentNullException(nameof(potRepository));
        this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
    }

    public async Task<PresentPotsResponse> Handle(PresentPotsRequest request, CancellationToken cancellationToken)
    {
        return new PresentPotsResponse
        {
            Date = systemClock.Today,
            Pots = await RetrievePots(request.IncludeInactivePots)
        };
    }

    private async Task<List<PotInfo>> RetrievePots(bool includeInactivePots)
    {
        IEnumerable<Pot> pots = await potRepository.GetAll();

        IEnumerable<PotInfo> potInfos = pots
            .Select(ToPotInfo);

        if (!includeInactivePots)
        {
            potInfos = potInfos
                .Where(x => x.IsActive);
        }

        return potInfos.ToList();
    }

    private PotInfo ToPotInfo(Pot pot)
    {
        bool isActive = pot.IsActive(systemClock.Today);

        Gem lastGem = pot.GetLastGem();

        CurrencyValue value = isActive && lastGem != null
            ? new CurrencyValue
            {
                Currency = pot.Currency,
                Value = lastGem.Value
            }
            : null;

        return new PotInfo
        {
            Name = pot.Name,
            Value = value,
            IsActive = isActive
        };
    }
}