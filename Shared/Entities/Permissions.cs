using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public enum Permissions
    {
        CanChangePermissions,
        CanEditUsers,
        CanEditPhotos,
        CanEditCurrencies,
        CanEditProducts,
        CanEditTemplates,
        CanSaveTemplate,
        CanUseManager,
        CanExportOfferWithOldPrice,
        CanMakeOfferWithCostPrice,
        CanSeeAllOffers,
        CanAll
    }
}
