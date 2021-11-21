using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace OfferMaker
{
    public class DebugTreeItem : BaseModel
    {
        string propertyName;
        string propertyType;
        string propertyValue;
        ObservableCollection<DebugTreeItem> childs = new ObservableCollection<DebugTreeItem>();

        public string PropertyType
        {
            get => propertyType;
            set
            {
                propertyType = value;
                OnPropertyChanged();
            }
        }

        public string PropertyName
        {
            get=> propertyName;
            set
            {
                propertyName = value;
                OnPropertyChanged();
            }
        }

        public string PropertyValue
        {
            get => propertyValue;
            set
            {
                propertyValue = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DebugTreeItem> Childs
        {
            get => childs;
            set
            {
                childs = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            if (PropertyValue == null) PropertyValue = "null";
            return PropertyType + " " + PropertyName + "=" + PropertyValue;
        }
    }
}
