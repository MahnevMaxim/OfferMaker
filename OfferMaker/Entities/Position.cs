using Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfferMaker
{
    public class Position
    {
        public int Id { get; set; }

        public string PositionName { get; set; }

        /// <summary>
        /// Коллекция включенных разрешений должности.
        /// </summary>
        public ObservableCollection<Permissions> Permissions { get; set; } = new ObservableCollection<Permissions>();

        /// <summary>
        /// Коллекция обёрток для хранения и отображения Permissions.
        /// Содержит все возможные разрешения и состояние - IsEnabled
        /// </summary>
        public ObservableCollection<PermissionWrapper> PermissionWrappers { get; set; } = new ObservableCollection<PermissionWrapper>();

        public Position(string newPositionName)
        {
            PositionName = newPositionName;
            SetPermissions();
        }

        private void SetPermissions()
        {
            foreach (Permissions p in Enum.GetValues(typeof(Permissions)))
            {
                PermissionWrappers.Add(new PermissionWrapper(p));
            }
        }

        public override string ToString() => PositionName;

        internal void SavePermissions()
        {
            Permissions.Clear();
            PermissionWrappers.ToList().ForEach(p =>
            {
                if (p.IsEnabled)
                    Permissions.Add(p.PermissionVal);
            });
        }

        internal void SetWrapperPermission()
        {
            foreach (Permissions perm in Permissions)
            {
                var pw = PermissionWrappers.Where(p => p.PermissionVal == perm).FirstOrDefault();
                if (pw != null)
                    pw.IsEnabled = true;
            }
        }
    }
}
