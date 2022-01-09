using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    /// <summary>
    /// Обёртка для разрешения.
    /// </summary>
    public class PermissionWrapper
    {
        public bool IsEnabled { get; set; }

        public Permissions PermissionVal { get; set; }

        public string PermissionName { get; set; }

        public PermissionWrapper(Permissions permission)
        {
            PermissionVal = permission;
            PermissionName = GetName(permission);
        }

        private string GetName(Permissions permission) => permission switch
        {
            Permissions.CanAll => "Админ",
            Permissions.CanControlPositions => "Управление должностями",
            Permissions.CanControlUsers => "Управление пользователями",
            Permissions.CanEditPhotos => "Разрешение изменять и загружать фотографии",
            Permissions.CanEditCurrencies => "Разрешение изменять валюты",
            Permissions.CanEditProducts => "Разрешение изменять номенклатуру",
            Permissions.CanEditTemplates => "Разрешение изменять шаблоны",
            Permissions.CanSaveTemplate => "Разрешение сохранять шаблоны",
            Permissions.CanUseManager => "Разрешение назначать менеджера",
            Permissions.CanExportOfferWithOldPrice => "Разрешение экспортировать КП с неактуальными ценами",
            Permissions.CanMakeOfferWithCostPrice => "Разрешение формировать КП по себестоимости",
            Permissions.CanSeeAllOffers => "Разрешение видеть все КП",
            _ => throw new NotImplementedException()
        };

        public override string ToString() => PermissionVal.ToString() + " " + IsEnabled.ToString() + " " + PermissionName;
    }
}
