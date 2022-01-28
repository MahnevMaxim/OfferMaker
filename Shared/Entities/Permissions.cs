using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public enum Permissions
    {
        CanControlPositions,
        CanControlUsers,
        CanControlArchive,
        CanControlTemplates,
        CanEditCurrencies,
        CanEditProducts,
        CanUseManager,
        CanExportOfferWithOldPrice,
        CanMakeOfferWithCostPrice,
        CanSeeAllOffers,
        CanAll
    }
}
